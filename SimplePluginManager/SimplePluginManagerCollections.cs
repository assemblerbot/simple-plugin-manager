#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SimplePluginManager
{
    public class SimplePluginManagerCollections
    {
        public class CPluginsPair
        {
            public SimplePluginManagerPlugin RepositoryPlugin;
            public SimplePluginManagerPlugin ProjectPlugin;

            public string Id   => RepositoryPlugin?.Id   ?? ProjectPlugin.Id;
            public string Name => RepositoryPlugin?.Name ?? ProjectPlugin.Name;
        }

        private SimplePluginManagerCollection m_Repository;
        private SimplePluginManagerCollection m_Project;

        public bool IsInitialized => m_Repository != null && m_Project != null;
        
        public string Update(SimplePluginManagerSettings settings)
        {
            if (settings == null)
            {
                return "Missing settings asset!";
            }

            string repository_path = settings.GetValidRepositoryPath(out var error_message);
            if (string.IsNullOrEmpty(repository_path))
            {
                return error_message;
            }

            m_Repository = new SimplePluginManagerCollection();
            m_Repository.Init(repository_path);

            string project_path = settings.GetValidProjectPath(out error_message);
            if (string.IsNullOrEmpty(project_path))
            {
                return error_message;
            }

            m_Project = new SimplePluginManagerCollection();
            m_Project.Init(Path.Combine(Application.dataPath, project_path));

            return null;
        }
        
        public List<CPluginsPair> BuildListOfPlugins()
        {
            List<CPluginsPair> list_of_plugins = new List<CPluginsPair>();

            foreach (SimplePluginManagerPlugin repository_plugin in m_Repository.Plugins.Values)
            {
                SimplePluginManagerPlugin project_plugin = null;
                m_Project.Plugins?.TryGetValue(repository_plugin.Id, out project_plugin);
                list_of_plugins.Add(new CPluginsPair {RepositoryPlugin = repository_plugin, ProjectPlugin = project_plugin});
            }

            foreach (SimplePluginManagerPlugin project_plugin in m_Project.Plugins.Values)
            {
                SimplePluginManagerPlugin repository_plugin = null;
                m_Repository.Plugins?.TryGetValue(project_plugin.Id, out repository_plugin);

                if (repository_plugin == null)
                {
                    // this plugin exists in project only
                    list_of_plugins.Add(new CPluginsPair {RepositoryPlugin = null, ProjectPlugin = project_plugin});
                }
            }

            list_of_plugins.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCulture));
            return list_of_plugins;
        }

        public bool IsAnyProjectPluginDependentOn(string plugin_id)
        {
            return m_Project.Plugins.Any(x => x.Value.Dependencies != null && x.Value.Dependencies.Contains(plugin_id));
        }

        public bool IsInProject(string plugin_id)
        {
            return m_Project.Plugins.ContainsKey(plugin_id);
        }

        public bool IsInRepository(string plugin_id)
        {
            return m_Repository.Plugins.ContainsKey(plugin_id);
        }

        public bool IsDependencyError(SimplePluginManagerPlugin plugin)
        {
            foreach (string dependency in plugin.Dependencies)
            {
                if (m_Project.Plugins.ContainsKey(dependency))
                {
                    continue;
                }
                
                if(m_Repository.Plugins.ContainsKey(dependency))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public string GetPluginNameAndVersion(string plugin_id)
        {
            if (IsInProject(plugin_id))
            {
                return $"{m_Project.Plugins[plugin_id].Name} ({m_Project.Plugins[plugin_id].Version})";
            }

            if (IsInRepository(plugin_id))
            {
                return $"{m_Repository.Plugins[plugin_id].Name} ({m_Repository.Plugins[plugin_id].Version})";
            }

            return plugin_id;
        }

        public void CopyDependenciesFromRepositoryToProject(SimplePluginManagerPlugin plugin, SimplePluginManagerSettings settings)
        {
            List<string> installed_dependencies = new List<string>();
	        
            foreach (string dependency in plugin.Dependencies)
            {
                if (IsInProject(dependency))
                {
                    continue; // valid
                }

                if (IsInRepository(dependency))
                {
                    // can be installed
                    m_Repository.Plugins[dependency].CopyFromRepositoryToProject(settings);
                    installed_dependencies.Add(dependency);
                }
            }

            // refresh arrays of plugins
            Update(settings);
	        
            // recursively install dependencies of new installed dependencies
            foreach (string installed_dependency in installed_dependencies)
            {
                CopyDependenciesFromRepositoryToProject(m_Repository.Plugins[installed_dependency], settings);
            }
        }
    }
}
#endif