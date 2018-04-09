using ChatBot.Model;
using ChatBot.Parser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBot.Bot
{
    /// <summary>
    /// 在此类中实现取优算法
    /// </summary>
    public abstract class IBot
    {
        /// <summary>
        /// 对话的历史记录
        /// 0为最近的对话
        /// </summary>
        protected List<string> history = new List<string>();
        /// <summary>
        /// 所有的扩展
        /// </summary>
        protected List<PlusModel> extends = new List<PlusModel>();

        /// <summary>
        /// 在子类中重写时，此方法用于将转换器添加进来
        /// </summary>
        /// <param name="parser">转换器</param>
        /// <returns></returns>
        public abstract Task Attach(IParser parser);

        /// <summary>
        /// 在子类中重写时，此方法用于根据输入的文本获取响应
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public abstract string GetResponse(string inputStr);

        /// <summary>
        /// 获取最后一条历史记录
        /// </summary>
        /// <returns></returns>
        protected abstract string GetLastHistory();

        /// <summary>
        /// 添加一条历史记录
        /// </summary>
        /// <param name="inputStr"></param>
        protected abstract void AddHistory(string inputStr);

        /// <summary>
        /// 在子类中重写时，此方法用于获取所有的扩展信息
        /// </summary>
        /// <returns></returns>
        protected abstract List<PlusModel> GetExtends();
    }
}
