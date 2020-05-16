#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimplePluginManager
{
    public class SimplePluginManagerCollection
    {
        private Dictionary<string, SimplePluginManagerPlugin> m_Plugins;
        public Dictionary<string, SimplePluginManagerPlugin> Plugins => m_Plugins;
        
        public void Init(string path)
        {
            m_Plugins = new Dictionary<string, SimplePluginManagerPlugin>();
            
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] descriptors = Directory.GetFiles(path, "plugin.json", SearchOption.AllDirectories);
            foreach (string descriptor in descriptors)
            {
                SimplePluginManagerPlugin plugin = ReadPlugin(descriptor);
                m_Plugins.Add(plugin.Id, plugin);
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
    }
}
#endif