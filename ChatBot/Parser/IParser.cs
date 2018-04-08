using ChatBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Parser
{
    public interface IParser
    {
        /// <summary>
        /// 此方法用于从资料中获取对话文本
        /// </summary>
        /// <returns></returns>
        List<IModel> Start();

        /// <summary>
        /// 此方法用于载入已经学习好的对话文本
        /// 此方法没有获得数据时会尝试调用Start
        /// </summary>
        /// <returns></returns>
        List<IModel> ReStart();

        /// <summary>
        /// 此方法用于将用户输入转为HashCode数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        int[] Parse(string text);

        /// <summary>
        /// 此方法用于新增用户自定义输入
        /// </summary>
        /// <param name="list"></param>
        void Save(IModel model);
    }
}
