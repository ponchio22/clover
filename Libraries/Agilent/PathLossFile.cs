using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Valutech.Files;
using System.Collections;
using System.Diagnostics;

namespace Valutech.Wtm
{
    public delegate void PathLossFileSavedEventHandler();

    public delegate void PathLossFileSaveErrorEventHandler();

    public class PathLossFile:FileRepresentation
    {
        #region Constants Declaration
        /// <summary>
        /// Path Effect Tag
        /// </summary>
        private const string PATH_EFFECT_TABLE = "Path_Effect_Table";

        /// <summary>
        /// Path type tag
        /// </summary>
        private const string PATH_TYPE = "Path";

        /// <summary>
        /// Version property
        /// </summary>
        private const string VERSION = "version";

        /// <summary>
        /// Name property 
        /// </summary>
        private const string NAME = "name";

        /// <summary>
        /// 
        /// </summary>
        private const string VALUE = "value";

        /// <summary>
        /// Default path type
        /// </summary>
        private const string DEFAULT_PATH_TYPE = "RF IN/OUT_SYS1_FIX1";

        private const string SPECTRUM = "Spectrum";

        private const string PATH_EFFECT = "PathEffect";

        private ArrayList channelLosses = new ArrayList();

        public event PathLossFileSaveErrorEventHandler SaveError;

        public event PathLossFileSavedEventHandler Saved;

        #endregion

        /// <summary>
        /// Creates a new Pathloss file object
        /// </summary>
        /// <param name="path"></param>
        public PathLossFile(string path)
            : base(path) { }

        /// <summary>
        /// Validates if its a pathloss file
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            Regex regexp = new Regex(".xml$|.XML$");
            if (regexp.IsMatch(path))
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(path);
                    XmlNodeList list = xml.GetElementsByTagName(PATH_EFFECT_TABLE);
                    if (list.Count > 0) return true;                    
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public void Save()
        {
            try
            {
                XmlWriter writer = XmlWriter.Create(this.path);
                
                writer.WriteStartElement(PATH_EFFECT_TABLE);
                writer.WriteAttributeString(VERSION, "1");

                    writer.WriteStartElement(PATH_TYPE);
                    writer.WriteAttributeString(NAME, DEFAULT_PATH_TYPE);

                    foreach (ChannelLoss channelLoss in channelLosses)
                    {
                        writer.WriteStartElement(SPECTRUM);
                        writer.WriteAttributeString(VALUE, channelLoss.channel.RxFrequencyExp);
                            writer.WriteStartElement(PATH_EFFECT);
                            writer.WriteAttributeString("level", "0");
                            writer.WriteString(channelLoss.RxFrequencyLoss.ToString()); 
                            writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteStartElement(SPECTRUM);
                        writer.WriteAttributeString(VALUE, channelLoss.channel.TxFrequencyExp);
                            writer.WriteStartElement(PATH_EFFECT);
                            writer.WriteAttributeString("level", "0");
                            writer.WriteString(channelLoss.TxFrequencyLoss.ToString());
                            writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();

                writer.WriteEndElement();
                writer.Close();
                if(Saved != null) Saved();
            }
            catch
            {
                if(SaveError != null) SaveError();
            }
        }

        public bool Load()
        {
            try
            {
                channelLosses = new ArrayList();
                ChannelsFile channelsFile = new ChannelsFile();
                channelsFile.Load();
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                XmlNodeList pathList = xml.GetElementsByTagName(PATH_TYPE);
                foreach (XmlNode pathNode in pathList)
                {
                    if (pathNode.Attributes[NAME].Value == DEFAULT_PATH_TYPE)
                    {
                        XmlNodeList lossList = ((XmlElement)pathNode).GetElementsByTagName(SPECTRUM);
                        foreach (XmlNode lossNode in lossList)
                        {
                            string frequency = lossNode.Attributes[VALUE].Value;
                            double loss = Convert.ToDouble(((XmlElement)lossNode).GetElementsByTagName(PATH_EFFECT)[0].InnerText);
                            Channel channel = channelsFile.GetChannel(frequency);
                            if (channel != null)
                            {
                                ChannelLoss channelLoss = null;
                                foreach (ChannelLoss channelLossItem in channelLosses)
                                {
                                    if (channelLossItem.channel.Name == channel.Name) channelLoss = channelLossItem;
                                }
                                if (channelLoss == null)
                                {
                                    channelLoss = new ChannelLoss(channel);
                                    channelLosses.Add(channelLoss);
                                }
                                if (frequency == channel.RxFrequencyExp) channelLoss.RxFrequencyLoss = loss; 
                                else if(frequency == channel.TxFrequencyExp) channelLoss.TxFrequencyLoss = loss;
                            }
                        }
                        
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ArrayList ChannelLosses
        {
            get
            {
                return this.channelLosses;
            }
        }
    }
}
