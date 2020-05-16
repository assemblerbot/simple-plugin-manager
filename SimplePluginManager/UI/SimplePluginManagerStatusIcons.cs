#if UNITY_EDITOR
using UnityEngine;

namespace SimplePluginManager
{
    public class SimplePluginManagerStatusIcons
    {
        private Texture2D m_IconUpToDate;
        private Texture2D m_IconCanUpgrade;
        private Texture2D m_IconCanDowngradeOrExport;
        private Texture2D m_IconNotInstalled;
        private Texture2D m_IconError;

        public SimplePluginManagerStatusIcons()
        {
            InitIcons();
        }

        private void InitIcons()
        {
            m_IconUpToDate             = SimplePluginManagerUnityUtils.LoadIcon("SimplePluginManagerUpToDate");
            m_IconCanUpgrade           = SimplePluginManagerUnityUtils.LoadIcon("SimplePluginManagerCanUpgrade");
            m_IconCanDowngradeOrExport = SimplePluginManagerUnityUtils.LoadIcon("SimplePluginManagerCanDowngradeOrExport");
            m_IconNotInstalled         = SimplePluginManagerUnityUtils.LoadIcon("SimplePluginManagerNotInstalled");
            m_IconError                = SimplePluginManagerUnityUtils.LoadIcon("SimplePluginManagerError");
        }

        public Texture2D GetPluginStatusIcon(SimplePluginManagerPlugin repository_plugin, SimplePluginManagerPlugin project_plugin)
        {
            if (project_plugin == null)
            {
                return m_IconNotInstalled;
            }
            
            if (repository_plugin == null)
            {
                return m_IconCanDowngradeOrExport;
            }
            
            SimplePluginManagerVersion repository_version_number = repository_plugin.ParseVersion;
            SimplePluginManagerVersion project_version_number    = project_plugin.ParseVersion;

            if (repository_version_number == project_version_number)
            {
                return m_IconUpToDate;
            }
            
            if (repository_version_number > project_version_number)
            {
                return m_IconCanUpgrade;
            }

            return m_IconCanDowngradeOrExport;
        }

        public Texture2D GetDependencyStatusIcon(bool project_contains_dependency, bool repository_contains_dependency)
        {
            if (project_contains_dependency)
            {
                return m_IconUpToDate;
            }

            if (repository_contains_dependency)
            {
                return m_IconNotInstalled;
            }

            return m_IconError;
        }
    }
}
#endif