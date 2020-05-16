#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimplePluginManager
{
    [CreateAssetMenu(fileName = "SimplePluginManagerSettings", menuName = "Create SimplePluginManager settings")]
    public class SimplePluginManagerSettings : ScriptableObject
    {
        [SerializeField] private List<string> RepositoryPluginsPaths;
        [SerializeField] private string       ProjectPluginsPath = "Plugins";

        public string GetValidRepositoryPath(out string error_message)
        {
            if (RepositoryPluginsPaths == null || RepositoryPluginsPaths.Count == 0)
            {
                error_message = "Repository paths are empty!";
                return null;
            }

            string valid_path = null;
            foreach (string path in RepositoryPluginsPaths)
            {
                if (Directory.Exists(path))
                {
                    if (valid_path == null)
                    {
                        valid_path = path;
                    }
                    else
                    {
                        error_message = "Multiple repository paths are valid, cannot determine which one is correct!";
                        return null;
                    }
                }
            }

            if (valid_path == null)
            {
                error_message = "No valid repository path found. Add one!";
                return null;
            }

            error_message = "";
            return valid_path;
        }

        public string GetValidProjectPath(out string error_message)
        {
            string valid_path = Path.Combine(Application.dataPath, ProjectPluginsPath);
            if (Directory.Exists(valid_path))
            {
                error_message = "";
                return valid_path;
            }

            error_message = $"Project path doesn't exist! ({valid_path})";
            return null;
        }
    }
}
#endif