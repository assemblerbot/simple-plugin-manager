#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimplePluginManager
{
    public static class SimplePluginManagerFileUtils
    {
        public static void CopyPlugin(string source_path_to_descriptor_file, string source_root_directory, string destination_root_directory)
        {
            Assert.IsTrue(source_path_to_descriptor_file.StartsWith(source_root_directory));
            
            string source_plugin_directory = Path.GetDirectoryName(source_path_to_descriptor_file);

            string relative_source_plugin_directory = source_plugin_directory.Substring(source_root_directory.Length);
            if (relative_source_plugin_directory.First() == '\\' || relative_source_plugin_directory.First() == '/')
            {
                relative_source_plugin_directory = relative_source_plugin_directory.Substring(1);
            }

            string destination_plugin_directory = Path.Combine(destination_root_directory, relative_source_plugin_directory);

            Debug.Log("======= Copying files =======");
            Debug.Log("From: " + source_plugin_directory);
            Debug.Log("To: " + destination_plugin_directory);

            if (Directory.Exists(destination_plugin_directory))
            {
                Directory.Delete(destination_plugin_directory, true);
            }

            CreateDirectoryStructure(destination_plugin_directory);
            CopyRecursive(source_plugin_directory, destination_plugin_directory);
            Debug.Log("DONE!");
        }

        public static void DeletePlugin(string source_path_to_descriptor_file)
        {
            string source_plugin_directory = Path.GetDirectoryName(source_path_to_descriptor_file);

            Debug.Log("======= Deleting directory =======");
            if (Directory.Exists(source_plugin_directory))
            {
                Debug.Log("Deleting: " + source_plugin_directory);
                Directory.Delete(source_plugin_directory, true);
            }

            Debug.Log("DONE!");
        }

        public static void CreateDirectoryStructure(string directory_path)
        {
            if (Directory.Exists(directory_path))
            {
                return;
            }

            CreateDirectoryStructure(Path.GetDirectoryName(directory_path));
            
            Debug.Log($"Creating: {directory_path}");
            Directory.CreateDirectory(directory_path);
        }

        public static void CopyRecursive(string from_directory, string to_directory)
        {
            if (!Directory.Exists(to_directory))
            {
                Debug.Log($"Creating: {to_directory}");
                Directory.CreateDirectory(to_directory);
            }

            // files
            foreach (string from_file_path in Directory.GetFiles(from_directory))
            {
                string to_file_path = Path.Combine(to_directory, Path.GetFileName(from_file_path));
                Debug.Log($"Copying: {from_file_path} -> {to_file_path}");
                File.Copy(from_file_path, to_file_path);
            }
 
            // directories
            foreach (string from_dir_path in Directory.GetDirectories(from_directory))
            {
                string to_dir_path = Path.Combine(to_directory, Path.GetFileName(from_dir_path));
                CopyRecursive(from_dir_path, to_dir_path);
            }
        }
    }
}
#endif