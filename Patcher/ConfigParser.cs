using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using YamlDotNet.RepresentationModel;

namespace Patcher
{
    public class ConfigParser
    {
        string _configFilePath = string.Empty;
        public ConfigParser(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public Config readConfig()
        {
            var input = new StringReader(File.ReadAllText(_configFilePath));

            var yaml = new YamlStream();
            yaml.Load(input);

            try
            {
                var config = new Config();
                var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
                var yamlConfig = (YamlMappingNode)mapping.Children[new YamlScalarNode("config")];
                var yamlPatchList = (YamlSequenceNode)yamlConfig.Children[new YamlScalarNode("patch_list")];
                config.patchType = yamlConfig.Children[new YamlScalarNode("patch_type")].ToString();
                foreach (YamlMappingNode patchList in yamlPatchList)
                {
                    config.patchList.Add(new Config.patch_list
                    {
                        patchType = patchList.Children[new YamlScalarNode("type")].ToString(),
                        patchFTPHost = patchList.Children[new YamlScalarNode("patch_ftp_host")].ToString(),
                        patchListURL = patchList.Children[new YamlScalarNode("patch_list_url")].ToString(),
                    });
                }

                return config;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class Config
        {
            public string patchType = string.Empty;
            public List<patch_list> patchList;
            
            public Config()
            {
                patchList = new List<Config.patch_list>();
            }

            public class patch_list
            {
                private string _patchType;
                public string patchType
                {
                    get { return _patchType; }
                    set { _patchType = value; }
                }

                private string _patchListURL;
                public string patchListURL
                {
                    get { return _patchListURL; }
                    set { _patchListURL = value; }
                }

                private string _patchFTPHost;
                public string patchFTPHost
                {
                    get { return _patchFTPHost; }
                    set { _patchFTPHost = value; }
                }
            }
        }
    }
}
