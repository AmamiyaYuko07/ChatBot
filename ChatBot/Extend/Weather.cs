using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatBot.Parser;
using Newtonsoft.Json.Linq;

namespace ChatBot.Extend
{
    public sealed class Weather : IExtend
    {
        public override string[] ResponseString()
        {
            return new string[] { "北京的天气怎么样" };
        }

        public override bool Run(string inputStr, IParser parser, List<string> history, out string outputStr)
        {
            Regex reg = new Regex(@"(.+?)的天气怎么样");
            if (reg.IsMatch(inputStr))
            {
                var m = reg.Match(inputStr);
                var city = m.Groups[1].Value;
                var url = "https://www.sojson.com/open/api/weather/json.shtml?city={0}";
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                var jsonResult = client.DownloadString(string.Format(url, city));
                var json = JObject.Parse(jsonResult);
                if(json.Value<string>("status") == "200")
                {
                    var data = json["data"];
                    var wendu = data.Value<string>("wendu");
                    var pm25 = data.Value<string>("pm25");
                    var quality = data.Value<string>("quality");
                    outputStr = string.Format($"今天温度{wendu}度，pm2.5指数为{pm25},{quality}");
                    return true;
                }
            }
            outputStr = inputStr;
            return false;
        }

        public override string RunAfterResult(string inputStr, string outputStr, IParser parser, List<string> history)
        {
            return null;
        }

        public override double Similarity()
        {
            return 0.7;
        }
    }
}
