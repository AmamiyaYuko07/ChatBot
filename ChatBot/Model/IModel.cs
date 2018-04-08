using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Model
{
    public class IModel
    {
        /// <summary>
        /// 此模型的标识
        /// 分词之后各词的HashCode
        /// </summary>
        public int[] Hash { set; get; }

        /// <summary>
        /// 原文本
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// 文本所对应的回答
        /// </summary>
        public string[] Answers { set; get; }
    }
}
