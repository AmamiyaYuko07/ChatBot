using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChatBot.Util
{
    public class ConfigHelper
    {
        /// <summary>
        /// 属性字典
        /// 用于对话使用，在Bot类中替换输出文字中出现的{name}标记
        /// </summary>
        Dictionary<string, string> propertyDict = new Dictionary<string, string>();
        /// <summary>
        /// 配置字典
        /// 用于程序内部使用，主要是用于保存类似数据库连接之类的配置信息
        /// </summary>
        Dictionary<string, string> configDict = new Dictionary<string, string>();

        private static ConfigHelper _helper = null;
        public static ConfigHelper GetInstance()
        {
            return _helper ?? (_helper = new ConfigHelper());
        }

        /// <summary>
        /// 获取属性字典
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                if (propertyDict.ContainsKey(propertyName))
                    return propertyDict[propertyName];
                return "";
            }
        }

        /// <summary>
        /// 获取配置字典
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public string GetConfigValue(string configName)
        {
            if (configDict.ContainsKey(configName))
                return configDict[configName];
            return "";
        }

        private ConfigHelper(string configPath = "config.xml")
        {
            propertyDict.Clear();
            configDict.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(configPath);
            var root = doc.DocumentElement;
            foreach(XmlElement x in root.ChildNodes)
            {
                if(x.Name == "property")
                {
                    GetPropertyByXml(x);
                }
                else
                {
                    GetConfigByXml(x);
                }
            }
        }

        private void GetConfigByXml(XmlElement node)
        {
            foreach (XmlElement x in node)
            {
                if (x.Name == "item")
                {
                    var name = x.GetAttribute("name");
                    var value = x.GetAttribute("value");
                    if (!configDict.ContainsKey(name))
                        configDict.Add(name, value);
                }
            }
        }

        private void GetPropertyByXml(XmlElement node)
        {
            foreach(XmlElement x in node)
            {
                if(x.Name == "item")
                {
                    var name = x.GetAttribute("name");
                    var value = x.GetAttribute("value");
                    if (!propertyDict.ContainsKey(name))
                        propertyDict.Add(name, value);
                }
            }
        }
    }
}
