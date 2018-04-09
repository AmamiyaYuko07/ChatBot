using ChatBot.Extend;
using ChatBot.Model;
using ChatBot.Parser;
using ChatBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBot.Bot
{
    public class TextBot:IBot
    {
        private List<IModel> _models = null;
        private IParser _parser = null;
        private bool isInited = false;
        private Regex regex = new Regex(@"\{(.+?)\}");
        private Random ran = new Random();
        private double Threshold = 0.25;
        private double MaxThreshold = 0.5;

        public TextBot()
            : base()
        {
            double.TryParse(ConfigHelper.GetInstance().GetConfigValue("Threshold") ?? "0.25", out Threshold);
            double.TryParse(ConfigHelper.GetInstance().GetConfigValue("MaxThreshold") ?? "0.5", out MaxThreshold);
        }

        public override async Task Attach(IParser parser)
        {
            this._parser = parser;
            await Task.Factory.StartNew(() =>
            {
                extends = GetExtends();
            });
            await Task.Factory.StartNew(() =>
            {
                _models = parser.ReStart();
                if (_models == null || _models.Count == 0)
                {
                    _models = parser.Start();
                }
                if (_models == null || _models.Count == 0)
                {
                    throw new Exception("找不到任何对话数据！");
                }
            });
            isInited = true;
        }

        /// <summary>
        /// 获取对话的回复
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public override string GetResponse(string inputStr)
        {
            if (!isInited) return "我正在读取我之前的记忆，请稍微等待一下";
            if (inputStr.StartsWith("你应该说："))
            {
                return Study(inputStr);
            }
            AddHistory(inputStr);
            int[] InputWords = _parser.Parse(inputStr);

            #region 调用扩展
            var outputStr = "";
            if(RunExtendOnPreInput(inputStr,InputWords,out outputStr))
            {
                return outputStr;
            }
            inputStr = outputStr;
            InputWords = _parser.Parse(inputStr);
            #endregion

            var result = GetResult(InputWords);
            if (result != null && result.Answers.Length > 0)
            {
                var answer = result.Answers[ran.Next(0, result.Answers.Length)];
                if (regex.IsMatch(answer))
                {
                    var ms = regex.Matches(answer);
                    foreach (Match m in ms)
                    {
                        var replaceStr = m.Groups[1].Value;
                        answer = answer.Replace("{" + replaceStr + "}", ConfigHelper.GetInstance()[replaceStr]);
                    }
                }

                #region 调用扩展
                answer = RunExtendAfterAnswer(inputStr, InputWords, answer);
                #endregion

                return answer;
            }
            else
            {
                return "我不知道你在说什么，但是你可以说【你应该说：回复的文字】来告诉我如何回答这个问题！";
            }
        }

        /// <summary>
        /// 学习
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private string Study(string inputStr)
        {
            var hisString = GetLastHistory();
            if (!string.IsNullOrEmpty(hisString))
            {
                var answer = inputStr.Substring(5);
                //查找是否已有此问题
                var m = _models.Where(x => x.Text == hisString).FirstOrDefault();
                if (m == null)
                {
                    m = new IModel()
                    {
                        Text = hisString,
                        Hash = _parser.Parse(hisString),
                        Answers = new string[] { answer }
                    };
                    this._models.Add(m);
                }
                else
                {
                    var temp = m.Answers.ToList();
                    temp.Add(answer);
                    m.Answers = temp.ToArray();
                }
                _parser.Save(m);
                return "好的，我已经记住了这个说法";
            }
            else
            {
                return "我们之前应该没有对话，我不清楚你想我怎么回复你";
            }
        }

        private IModel GetResult(int[] inputWords)
        {
            var max = 0.0;
            List<IModel> resultModels = new List<IModel>();
            var resultModel = new IModel();
            foreach (var x in this._models)
            {
                var result = levaten(inputWords, x.Hash);
                if(result > MaxThreshold)
                {
                    resultModels.Add(x);
                }
                else if (result > max)
                {
                    max = result;
                    resultModel = x;
                }
            }
            //首先查找有没有非常类似的结果，如果有则从结果集中任意挑选一个返回
            if(resultModels.Count > 0)
            {
                return resultModels[ran.Next(0, resultModels.Count)];
            }
            //否则挑选比较类似的结果中最好的一个
            if (max > Threshold)
            {
                return resultModel;
            }
            else
            {
                return null;
            }
        }

        private double levaten(int[] inputWords, int[] targetWords)
        {
            int[,] ints = new int[inputWords.Length + 1, targetWords.Length + 1];
            for (var i = 0; i <= inputWords.Length; i++)
            {
                ints[i, 0] = i;
            }
            for (var i = 0; i <= targetWords.Length; i++)
            {
                ints[0, i] = i;
            }
            var temp = 0;
            for (var i = 1; i <= inputWords.Length; i++)
            {
                var w1 = inputWords[i - 1];
                for (var j = 1; j <= targetWords.Length; j++)
                {
                    var w2 = targetWords[j - 1];
                    if (w1 == w2)
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    ints[i, j] = Math.Min(Math.Min(ints[i - 1, j] + 1, ints[i, j - 1] + 1), ints[i - 1, j - 1] + temp);
                }
            }
            var maxLength = Math.Max(inputWords.Length, targetWords.Length);
            return 1 - ((double)ints[inputWords.Length, targetWords.Length] / maxLength);
        }

        protected override string GetLastHistory()
        {
            if (history.Count > 0)
                return history[0];
            else
                return null;
        }

        protected override void AddHistory(string inputStr)
        {
            history.Insert(0, inputStr);
            if (history.Count > 20)
            {
                history = history.Take(20).ToList();
            }
        }

        private bool RunExtendOnPreInput(string InputStr, int[] InputWords,out string outputStr)
        {
            foreach(var x in extends)
            {
                if(x.Type == 1)
                {
                    foreach(var c in x.HashSet)
                    {
                        if(levaten(InputWords,c) >= x.extend.Similarity())
                        {
                            return x.extend.Run(InputStr, _parser, history, out outputStr);
                        }
                    }
                }
            }
            outputStr = InputStr;
            return false;
        }

        private string RunExtendAfterAnswer(string InputStr, int[] InputWords,string answerStr)
        {
            foreach (var x in extends)
            {
                if (x.Type == 2)
                {
                    foreach (var c in x.HashSet)
                    {
                        if (levaten(InputWords, c) >= x.extend.Similarity())
                        {
                            return x.extend.RunAfterResult(InputStr, answerStr, _parser, history);
                        }
                    }
                }
            }
            return answerStr;
        }

        protected override List<PlusModel> GetExtends()
        {
            var types = Assembly.GetCallingAssembly().GetTypes();
            var extendType = typeof(IExtend);
            List<PlusModel> extends = new List<PlusModel>();
            foreach (Type x in types)
            {
                var baseType = x.BaseType;
                if (baseType != null)
                {
                    if (baseType.Name == extendType.Name)
                    {
                        var obj = Activator.CreateInstance(x) as IExtend;
                        if (obj != null)
                        {
                            PlusModel m = new PlusModel();
                            m.responseString = obj.ResponseString();
                            m.Type = obj.Type();
                            m.extend = obj;
                            m.HashSet = m.responseString.Select(c => _parser.Parse(c)).ToList();
                            extends.Add(m);
                        }
                    }
                }
            }
            return extends;
        }
    }
}
