using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using Valutech.Files;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using Valutech.Wtm;
using System.Windows.Forms;

namespace Valutech.Wtm
{
    public class ChannelsFile:FileRepresentation
    {
        /// <summary>
        /// Default filename for Channels
        /// </summary>
        public static string FILE_NAME = "Channels.xml";

        private const string CHANNEL_TAG = "channel";

        private ArrayList channels = new ArrayList();

        public ChannelsFile()
            : base(Application.StartupPath + @"\" + FILE_NAME) { }

        public bool Load()
        {
            try
            {
                channels = new ArrayList();
                Regex regexp = new Regex(".xml$");
                if (regexp.IsMatch(path))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(path);
                    XmlNodeList list = xml.GetElementsByTagName(CHANNEL_TAG);
                    foreach (XmlNode node in list)
                    {
                        XmlNode rxFrequencyNode = ((XmlElement) node).GetElementsByTagName("rxfrequency")[0];
                        XmlNode txFrequencyNode = ((XmlElement)node).GetElementsByTagName("txfrequency")[0];
                        string name = node.Attributes["name"].Value;
                        string tech;
                        try
                        {
                            if (node.Attributes["tech"] != null && (node.Attributes["tech"].Value == CHANNEL_TECH.CDMA.ToString()) || (node.Attributes["tech"].Value == CHANNEL_TECH.GSM.ToString()) || (node.Attributes["tech"].Value == CHANNEL_TECH.WCDMA.ToString()))
                            {
                                tech = node.Attributes["tech"].Value;
                            } else {
                                tech = CHANNEL_TECH.CDMA.ToString();
                            }
                        }
                        catch
                        {
                            tech = CHANNEL_TECH.CDMA.ToString();
                        }
                        channels.Add(new Channel(node.Attributes["name"].Value,rxFrequencyNode.InnerText,txFrequencyNode.InnerText,tech));
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Channel GetChannel(string frequency)
        {
            foreach (Channel channel in channels)
            {
                if (channel.RxFrequencyExp == frequency || channel.TxFrequencyExp == frequency)
                {
                    return channel;
                }
            }
            return null;
        }

        public ArrayList Channels
        {
            get
            {
                return channels;
            }
        }
    }
}
