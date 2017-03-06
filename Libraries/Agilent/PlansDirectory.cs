using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.IO;

namespace Valutech.Agilent
{
    /// <summary>
    /// Plan structure
    /// </summary>
    public class Plan
    {
        public string FileName;
        public string Version;
        
        public Plan(string filename, string version)
        {
            this.FileName = filename;
            this.Version = version;
        }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Plans Directory
    /// Class that can handle the structure of a PlansDirectory xml file
    /// </summary>
    public class PlansDirectory
    {
        /// <summary>
        /// Stores the unique instance of the object
        /// </summary>
        private static PlansDirectory instance;

        #region XML Node Names

        private string xmlRootName = "PlansDirectory";
        private string xmlPlanNodeName = "Plan";
        private string xmlPlanFilename = "Filename";
        private string xmlPlanVersion = "Version";

        #endregion

        private ArrayList plans = new ArrayList();

        /// <summary>
        /// Constructor
        /// </summary>
        private PlansDirectory()
        {
            instance = this;            
        }

        /// <summary>
        /// Gets the instance of the unique object as its going to keep the data updated by itself
        /// </summary>
        /// <returns>Instance of the PlansDirectoy object</returns>
        public static PlansDirectory GetInstance()
        {
            if (instance == null) instance = new PlansDirectory();
            return instance;
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateDataFromFile(string localFile)
        {
            XmlDocument plansDirectoryXmlFile = new XmlDocument();
            plansDirectoryXmlFile.Load(localFile);
            XmlNodeList plansNodeList = plansDirectoryXmlFile.GetElementsByTagName(xmlPlanNodeName);
            plans = new ArrayList();
            foreach (XmlNode planNode in plansNodeList)
            {
                plans.Add(new Plan(planNode.Attributes[xmlPlanFilename].Value, planNode.Attributes[xmlPlanVersion].Value));
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateFileFromData(string fileToSave)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode plansDirectory = doc.CreateElement(xmlRootName);
            doc.AppendChild(plansDirectory);

            foreach (Plan plan in plans)
            {
                XmlNode node = doc.CreateElement(xmlPlanNodeName);
                XmlAttribute name = doc.CreateAttribute(xmlPlanFilename);
                name.Value = plan.FileName;
                XmlAttribute version = doc.CreateAttribute(xmlPlanVersion);
                version.Value = plan.Version;
                node.Attributes.Append(name);
                node.Attributes.Append(version);
                plansDirectory.AppendChild(node);
            }

            doc.Save(fileToSave);
        }

        /// <summary>
        /// Get the list of plans in the directory
        /// </summary>
        public ArrayList Plans
        {
            get
            {
                return this.plans;
            }
        }

        public void UpdatePlan(string planName)
        {
            if (this.plans.Count > 0)
            {
                for(int i = 0; i< this.plans.Count;i++)
                {
                    if (((Plan) this.plans[i]).FileName == planName)
                    {
                        this.plans[i]= new Plan(planName,planName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                }
            }
        }

        public Plan GetPlanByName(string planName)
        {            
            foreach (Plan iPlan in Plans)
            {
                if (iPlan.Name == planName)
                {
                    return iPlan;
                }
            }            
            return null;
        }
    }
}
