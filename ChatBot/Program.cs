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
            IBot bot = new IBot();
            IParser parser = new TextParser();
            bot.Attach(parser);
            while (true)
            {
                Console.Write("你：");
                var input = Console.ReadLine();
                if (input == "exit") break;
                Console.WriteLine("机器人：" + bot.GetResponse(input));
            }
        }
    }
}
