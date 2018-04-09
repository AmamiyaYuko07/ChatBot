using ChatBot.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Extend
{
    /// <summary>
    /// 继承此类以扩展更多的方法
    /// </summary>
    public abstract class IExtend
    {
        /// <summary>
        /// 返回响应的语句
        /// </summary>
        /// <returns></returns>
        public abstract string[] ResponseString();

        /// <summary>
        /// 执行的时机
        /// 1:传入后执行
        /// 2:获得结果后执行
        /// 3:两者都执行一次
        /// </summary>
        /// <returns></returns>
        public virtual int Type()
        {
            return 1;
        }

        /// <summary>
        /// 返回相似程度
        /// 一个0-1的浮点数
        /// 响应语句与输入的相似程度达到此精度则执行方法
        /// </summary>
        /// <returns></returns>
        public abstract double Similarity();

        /// <summary>
        /// 具体执行的方法
        /// 此方法会在传入用户输入后立即执行
        /// </summary>
        /// <param name="inputStr">用于输入的语句</param>
        /// <param name="parser">此机器人使用的转换器</param>
        /// <param name="history">之前对话的历史</param>
        /// <param name="outputStr">输出的语句</param>
        /// <returns>如果返回值为True表示不继续执行，outputStr的内容会作为输出返回给用户
        /// 如果返回值为False表示继续执行，output的内容会替换用户输入内容
        /// </returns>
        public abstract bool Run(string inputStr, IParser parser, List<string> history, out string outputStr);

        /// <summary>
        /// 具体执行的方法
        /// 此方法会在得到机器人返回的结果后执行
        /// </summary>
        /// <param name="inputStr">输入的对话</param>
        /// <param name="outputStr">机器人的回复</param>
        /// <param name="parser">此机器人使用的转换器</param>
        /// <param name="history">之前对话的历史</param>
        /// <returns></returns>
        public abstract string RunAfterResult(string inputStr, string outputStr, IParser parser, List<string> history);
    }
}
