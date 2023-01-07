#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

namespace SimplePluginManager
{
    public class SimplePluginManagerCollection
    {
        private const string c_PluginFileName  = "plugin.json";
        private const string c_PackageFileName = "package.json";
        
        private Dictionary<string, SimplePluginManagerPlugin> m_Plugins;
        public Dictionary<string, SimplePluginManagerPlugin> Plugins => m_Plugins;
        
        public void Init(string path)
        {
            m_Plugins = new Dictionary<string, SimplePluginManagerPlugin>();
            
            if (!Directory.Exists(path))
            {
                return;
            }

            // plugins
            {
                string[] descriptors = Directory.GetFiles(path, c_PluginFileName, SearchOption.AllDirectories);
                foreach (string descriptor in descriptors)
                {
                    SimplePluginManagerPlugin plugin = ReadPlugin(descriptor);
                    m_Plugins.Add(plugin.Id, plugin);
                }
            }
            
            // unity packages
            {
                string[] descriptors = Directory.GetFiles(path, c_PackageFileName, SearchOption.AllDirectories);
                foreach (string descriptor in descriptors)
                {
                    SimplePluginManagerPlugin plugin = ReadPackage(descriptor);
                    if (plugin != null) // can be null if plugin.json already exists in the same folder
                    {
                        m_Plugins.Add(plugin.Id, plugin);
                    }
                }
            }
        }

        private SimplePluginManagerPlugin ReadPlugin(string path)
        {
            SimplePluginManagerPlugin plugin;
            
            try
            {
                string json = File.ReadAllText(path);
                plugin = JsonUtility.FromJson<SimplePluginManagerPlugin>(json);
            }
            catch (Exception e)
            {
                plugin = new SimplePluginManagerPlugin{Id = path, Name = path, Error = e.ToString()};
            }

            plugin.PathToPlugin = path;
            return plugin;
        }

        private SimplePluginManagerPlugin ReadPackage(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string plugin_path = Path.Join(directory, c_PluginFileName);
            if (File.Exists(plugin_path))
            {
                return null;
            }

            SimplePluginManagerPlugin plugin;
            
            try
            {
                string json = File.ReadAllText(path);
                
                // convert dependencies dictionary to list on JSON level (to avoid using custom JSON parsers)
                {
                    string pattern = @"""dependencies""\:\s*\{\s*("".+""\s*\:\s*"".*""\s*\,*\s*)*\}";
                    Match   match   = Regex.Match(json, pattern);
                    if (match.Success)
                    {
                        string dependencies_list = "";
                        for (int i = 1; i < match.Groups.Count; ++i)
                        {
                            Group             group      = match.Groups[i];
                            CaptureCollection collection = group.Captures;
                            for (int j = 0; j < collection.Count; ++j)
                            {
                                Capture capture = collection[j];

                                string dependency_pattern     = @"("".+"")\s*\:\s*("".*"")";
                                string dependency_replacement = @"{""dependency"":$1,""version"":$2}";
                                dependencies_list += Regex.Replace(capture.ToString(), dependency_pattern, dependency_replacement);
                            }
                        }
                        
                        string replacement = $"\"dependencies\":[{dependencies_list}]";
                        json = Regex.Replace(json, pattern, replacement);    
                    }
                }

                UnityPackageManifest package = JsonUtility.FromJson<UnityPackageManifest>(json);
                plugin = new SimplePluginManagerPlugin
                         {
                             Id           = package.name,
                             Name         = package.displayName,
                             Description  = package.description,
                             Version      = package.version,
                             Dependencies = package.dependencies != null ? package.dependencies.Select(x => x.dependency).ToList() : new(),
                             GlobalDefines = new(),
                         };
            }
            catch (Exception e)
            {
                plugin = new SimplePluginManagerPlugin{Id = path, Name = path, Error = e.ToString()};
            }

            plugin.PathToPlugin = path;
            return plugin;
        }
    }
}
#endif