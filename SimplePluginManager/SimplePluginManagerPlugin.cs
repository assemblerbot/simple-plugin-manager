#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePluginManager
{
    [Serializable]
    public class SimplePluginManagerPlugin
    {
        // JSON serialized data
        [SerializeField] public string       Id;
        [SerializeField] public string       Name;
        [SerializeField] public string       Description;
        [SerializeField] public string       Version;
        [SerializeField] public List<string> Dependencies;
        [SerializeField] public List<string> GlobalDefines;
        
        // non serialized data
        [NonSerialized] public string PathToPlugin;
        [NonSerialized] public string Error;
        
        public SimplePluginManagerVersion ParseVersion => new SimplePluginManagerVersion(Version);

        public void CopyFromRepositoryToProject(SimplePluginManagerSettings settings)
        {
            SimplePluginManagerFileUtils.CopyPlugin(
                PathToPlugin,
                settings.GetValidRepositoryPath(out string repository_error_message),
                settings.GetValidProjectPath(out string project_error_message)
            );
            AddGlobalDefines();
        }

        public void CopyFromProjectToRepository(SimplePluginManagerSettings settings)
        {
            SimplePluginManagerFileUtils.CopyPlugin(
                PathToPlugin,
                settings.GetValidProjectPath(out string project_error_message),
                settings.GetValidRepositoryPath(out string repository_error_message)
            );
        }

        public void RemoveFromProject()
        {
            SimplePluginManagerFileUtils.DeletePlugin(PathToPlugin);
            RemoveGlobalDefines();
        }

        private void AddGlobalDefines()
        {
            foreach (string global_define in GlobalDefines)
            {
                SimplePluginManagerUnityUtils.AddGlobalDefine(global_define);
            }
        }
        
        private void RemoveGlobalDefines()
        {
            foreach (string global_define in GlobalDefines)
            {
                SimplePluginManagerUnityUtils.RemoveGlobalDefine(global_define);
            }
        }
    }
}
#endif