using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caro.Services
{
    public class IniFile
    { 
        private IniData     iniData;
        FileIniDataParser   parser;
        public IniFile(string nameFile)
        {
            parser  = new FileIniDataParser();
            iniData = parser.ReadFile(AppDomain.CurrentDomain.BaseDirectory + nameFile);
        }

        /// <summary>
        /// Get List Key From Section
        /// </summary>
        /// <param name="nameSection">Name of section</param>
        /// <returns ListKey></returns>
        public List<string> GetListKeyName(string nameSection)
        {
            List<string> listKey = new List<string>();
            if (iniData.Sections.ContainsSection(nameSection))
            {
                KeyDataCollection keyDatas = iniData.Sections[nameSection];
                foreach (var keyData in keyDatas)
                {
                    listKey.Add(keyData.KeyName);
                }
            }

            return listKey;
        }

        /// <summary>
        /// Get data of key from section
        /// </summary>
        /// <param name="nameSection">Name of section</param>
        /// <param name="nameKey">Name of key</param>
        /// <returns string=keyName>Data of key</returns>
        public string GetDataFromSectionAndKey(string nameSection, string nameKey)
        {
            if (iniData.Sections.ContainsSection(nameSection))
            {
                KeyDataCollection keyDatas = iniData.Sections[nameSection];
                foreach (var keyData in keyDatas)
                {
                    if (keyData.KeyName == nameKey)
                    {
                        return keyData.Value;
                    }
                }
            }

            return String.Empty;
        }

        public void SetDataForKeyFromSection(string nameSection, string nameKey, string value)
        {
            if (iniData.Sections.ContainsSection(nameSection))
            {
                iniData[nameSection][nameKey] = value;
            }

            parser.WriteFile("config.ini", iniData);
            
        }
    }
}
