using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatBot.Model;
using JiebaNet.Segmenter;
using System.IO;

namespace ChatBot.Parser
{
    /// <summary>
    /// 文本转换器，从文本中获取学习资料
    /// </summary>
    public class TextParser : IParser
    {
        private string txtPath = "";
        private string customPath = "";
        private JiebaSegmenter segmenter = new JiebaSegmenter();

        public TextParser(string txtPath = @"data\txts")
        {
            this.txtPath = txtPath;
            this.customPath = Path.Combine(txtPath, "custom.txt");
        }

        public int[] Parse(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var words = segmenter.Cut(text);
                return words.Select(x => x.GetHashCode()).ToArray();
            }
            else
            {
                return new int[] { };
            }
        }

        public List<IModel> ReStart()
        {
            return Start();
        }

        public void Save(IModel model)
        {
            using (StreamWriter sw = new StreamWriter(this.customPath, true))
            {
                sw.WriteLine(model.Text);
                foreach (var x in model.Answers)
                {
                    sw.WriteLine("    " + x);
                }
            }
        }

        /// <summary>
        /// 从文本文档中读取数据
        /// </summary>
        /// <returns></returns>
        public List<IModel> Start()
        {
            List<IModel> models = new List<IModel>();
            foreach (var path in Directory.GetFiles(this.txtPath, "*.txt"))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        var str = "";
                        List<IModel> theModels = new List<IModel>();
                        List<string> answers = new List<string>();
                        while ((str = sr.ReadLine()) != null)
                        {
                            if ((str.StartsWith("    ") || str.StartsWith("\t")) && !string.IsNullOrEmpty(str.Trim()))
                            {
                                answers.Add(str.TrimStart());
                            }
                            else if (!string.IsNullOrEmpty(str))
                            {
                                if (answers.Count > 0)
                                {
                                    foreach(var x in theModels)
                                    {
                                        x.Answers = answers.ToArray();
                                        models.Add(x);
                                    }
                                    answers.Clear();
                                    theModels.Clear();
                                }
                                var model = new IModel();
                                model = new IModel();
                                model.Text = str;
                                model.Hash = Parse(str);
                                theModels.Add(model);
                            }
                        }
                        if(answers.Count > 0)
                        {
                            foreach (var x in theModels)
                            {
                                x.Answers = answers.ToArray();
                                models.Add(x);
                            }
                        }
                    }
                }
            }
            return models;
        }
    }
}