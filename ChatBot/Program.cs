using ChatBot.Bot;
using ChatBot.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            IBot bot = new TextBot();
            IParser parser = new TextParser();
            bot.Attach(parser).ContinueWith((x) =>
            {
                Console.WriteLine("记忆单元载入完成");
                while (true)
                {
                    Console.Write("你：");
                    var input = Console.ReadLine();
                    if (input == "exit") break;
                    Console.WriteLine("机器人：" + bot.GetResponse(input));
                }
            });
            Console.WriteLine("正在载入记忆单元");
            while (true) { System.Threading.Thread.Sleep(TimeSpan.FromDays(1)); }
        }
    }
}
