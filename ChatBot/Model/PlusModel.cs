using ChatBot.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Model
{
    public class PlusModel
    {
        /// <summary>
        /// 响应语句
        /// </summary>
        public string[] responseString { set; get; }

        /// <summary>
        /// 响应语句的HashCode集合
        /// </summary>
        public List<int[]> HashSet { set; get; }

        /// <summary>
        /// 扩展方法
        /// </summary>
        public IExtend extend { set; get; }

        /// <summary>
        /// 扩展类型
        /// 1：用户输入后触发
        /// 2：机器人返回结果后触发
        /// 3：两者都触发
        /// </summary>
        public int Type { set; get; }
    }
}
