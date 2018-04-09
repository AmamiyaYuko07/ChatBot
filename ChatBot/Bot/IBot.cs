using ChatBot.Model;
using ChatBot.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Bot
{
    /// <summary>
    /// 在此类中实现取优算法
    /// </summary>
    public class IBot
    {
        private List<IModel> _models = null;
        private IParser _parser = null;
        private bool isInited = false;
        private List<string> history = new List<string>();
        private Random ran = new Random();

        public virtual async Task Attach(IParser parser)
        {
            this._parser = parser;
            await Task.Factory.StartNew(() =>
            {
                _models = parser.ReStart();
                if(_models == null || _models.Count == 0)
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
        public virtual string GetResponse(string inputStr)
        {
            if (!isInited) return "我正在读取我之前的记忆，请稍微等待一下";
            if (inputStr.StartsWith("你应该说："))
            {
                var hisString = GetLastHistory();
                if (!string.IsNullOrEmpty(hisString))
                {
                    var answer = inputStr.Substring(5);
                    //查找是否已有此问题
                    var m = _models.Where(x => x.Text == hisString).FirstOrDefault();
                    if(m == null)
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
            history.Add(inputStr);
            int[] InputWords = _parser.Parse(inputStr);
            var result = GetResult(InputWords);
            if(result != null && result.Answers.Length > 0)
            {
                return result.Answers[ran.Next(0, result.Answers.Length)];
            }
            else
            {
                return "我不知道你在说什么，但是你可以说【你应该说：回复的文字】来告诉我如何回答这个问题！";
            }
        }

        private IModel GetResult(int[] inputWords)
        {
            var max = 0.0;
            var resultModel = new IModel();
            foreach(var x in this._models)
            {
                var result = levaten(inputWords, x.Hash);
                if(result > max)
                {
                    max = result;
                    resultModel = x;
                }
            }
            if(max > 0.1)
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
            for(var i = 0; i <= inputWords.Length; i++)
            {
                ints[i, 0] = i;
            }
            for(var i = 0; i <= targetWords.Length; i++)
            {
                ints[0, i] = i;
            }
            var temp = 0;
            for(var i = 1; i <= inputWords.Length; i++)
            {
                var w1 = inputWords[i - 1];
                for(var j = 1; j <= targetWords.Length; j++)
                {
                    var w2 = targetWords[j - 1];
                    if(w1 == w2)
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

        private string GetLastHistory()
        {
            if (history.Count > 0)
                return history[history.Count - 1];
            else
                return null;
        }
    }
}
