using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Valutech.IO;

namespace Valutech.Agilent
{
    public class OEMConfigFile:INIFile
    {

        #region Constants

        public const String NOT_SET = "NOT SET";

        public const String BRAND_INFO_SECTION = "General Info";

        public const String TEST_PLAN_INFO_SECTION = "Test Plan Info";

        public const String BRAND_NAME_VARIABLE = "Name";

        public const String TEST_PLAN_VARIABLE = "Test Plan";

        public const String TEST_PLAN_VERSION_VARIABLE = "Test Plan Version";

        public const String CONFIG_FILE_NAME = "Config.ini";

        #endregion

        #region Old variables

        public const String BRAND_NAME_OLD_VARIABLE = "Brand";

        public const String TEST_PLAN_OLD_VARIABLE = "TestPlan";

        public const String TEST_PLAN_VERSION_OLD_VARIABLE = "TestPlanVersion";

        public const String OLD_CONFIG_FILE_NAME = "config";

        public string oldBrandName;

        public string oldTestPlan;

        public string oldTestPlanVersion;

        #endregion

        public OEMConfigFile(OEM oem)
            : base(Path.Combine(oem.Path,CONFIG_FILE_NAME))
        {
            if (Exists())
            {
                MigrateToLatestFormat();
            }
            else
            {
                CreateBlank();
            }
        }

        public string brand
        {
            set { Write(BRAND_INFO_SECTION, BRAND_NAME_VARIABLE, value); MigrateToLatestFormat(); }
            get {
                string value = Read(BRAND_INFO_SECTION, BRAND_NAME_VARIABLE); 
                return Read(BRAND_INFO_SECTION, BRAND_NAME_VARIABLE); 
            }
        }

        public string testPlan
        {
            set { Write(TEST_PLAN_INFO_SECTION, TEST_PLAN_VARIABLE, value); MigrateToLatestFormat(); }
            get { return Read(TEST_PLAN_INFO_SECTION, TEST_PLAN_VARIABLE); }
        }

        public string testPlanVersion
        {
            set 
            { 
                Write(TEST_PLAN_INFO_SECTION, TEST_PLAN_VERSION_VARIABLE, value); 
                MigrateToLatestFormat(); 
            }
            get { return Read(TEST_PLAN_INFO_SECTION, TEST_PLAN_VERSION_VARIABLE); }
        }

        public void CreateBlank()
        {
            if (Exists()) Delete();
            brand = NOT_SET;
            testPlan = NOT_SET;
            testPlanVersion = NOT_SET;            
        }

        private void MigrateToLatestFormat()
        {
            if (Exists())
            {                
                if (UsesOldFormat)
                {
                    if (Exists()) Delete();
                    Write(BRAND_INFO_SECTION, BRAND_NAME_VARIABLE, oldBrandName);
                    Write(TEST_PLAN_INFO_SECTION, TEST_PLAN_VARIABLE, oldTestPlan);
                    Write(TEST_PLAN_INFO_SECTION, TEST_PLAN_VERSION_VARIABLE, oldTestPlanVersion);
                }
                if (brand == string.Empty) brand = NOT_SET;
                if (testPlan == string.Empty) testPlan = NOT_SET;
                if (testPlanVersion == string.Empty) testPlanVersion = NOT_SET;
            }
        }

        private bool UsesOldFormat
        {
            get
            {
                try
                {
                    if (GetSectionNames().Count == 0)
                    {
                        LoadOldVariables();
                        if (oldBrandName != string.Empty)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        private void LoadOldVariables() {
            oldBrandName = string.Empty;
            oldTestPlanVersion = string.Empty;
            oldTestPlan = string.Empty;
            string sLine = string.Empty;
            StreamReader sr = new StreamReader(this.filePath);
            do
            {
                sLine = sr.ReadLine();
                if (sLine != null)
                {
                    if (Convert.ToBoolean(sLine.Length))
                    {
                        if (sLine.Substring(0, 1) != "[")
                        {
                            if(new Regex(" = ").IsMatch(sLine)) {
                                string[] sLineParts = new Regex(" = ").Split(sLine);
                                if (sLineParts.GetValue(0).ToString() == BRAND_NAME_OLD_VARIABLE)
                                {
                                    oldBrandName = sLineParts.GetValue(1).ToString();
                                }
                                else if (sLineParts.GetValue(0).ToString() == TEST_PLAN_OLD_VARIABLE)
                                {
                                    oldTestPlan = sLineParts.GetValue(1).ToString();
                                }
                                else if (sLineParts.GetValue(0).ToString() == TEST_PLAN_VERSION_OLD_VARIABLE)
                                {
                                    oldTestPlanVersion = sLineParts.GetValue(1).ToString();
                                }
                            }
                        }
                    }
                }
            } while (sLine != null);
            sr.Close();
        }
    }
}
