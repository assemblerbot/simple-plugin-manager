#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimplePluginManager
{
    public class SimplePluginManagerWindow : EditorWindow
    {
	    private SimplePluginManagerSettings    m_Settings;
	    private SimplePluginManagerCollections m_PluginCollections;
	    private SimplePluginManagerStatusIcons m_Icons;

	    private string                   m_ErrorMessage;
	    private Vector2                  m_MainWindowScrolling;
	    private Dictionary<string, SimplePluginManagerPluginFoldout> m_PluginFoldouts;
	    
	    [MenuItem("Tools/Simple Plugin Manager", false)]
        public static void OnMenuClicked()
        {
            EditorWindow.GetWindow(typeof(SimplePluginManagerWindow), false, "Simple Plugin Manager");
        }

        void Awake()
        {
	        InitSettings();
        }

        private void OnInspectorUpdate()
        {
	        UpdatePluginCollections();
        }

        private void OnGUI()
        {
	        GuiSettingsSection();
	        GuiErrorMessageSection();
	        
	        m_MainWindowScrolling = GUILayout.BeginScrollView(m_MainWindowScrolling);
	        {
		        GuiPluginsSection();
	        }
	        GUILayout.EndScrollView();
        }

        private void GuiErrorMessageSection()
        {
	        if (string.IsNullOrEmpty(m_ErrorMessage))
	        {
		        return;
	        }

	        SimplePluginManagerUIUtils.ErrorLabel(m_ErrorMessage);
        }

        private void GuiSettingsSection()
        {
	        GUILayout.Label("Settings", EditorStyles.boldLabel);

	        EditorGUILayout.BeginHorizontal();
	        {
		        m_Settings = EditorGUILayout.ObjectField(m_Settings, typeof(SimplePluginManagerSettings), false) as SimplePluginManagerSettings;
		        if (GUILayout.Button("Refresh!"))
		        {
			        UpdatePluginCollections();
		        }
	        }
	        EditorGUILayout.EndHorizontal();
	        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Height(10));
        }

        private void GuiPluginsSection()
        {
	        if (m_PluginCollections == null || !m_PluginCollections.IsInitialized)
	        {
		        return;
	        }

	        SimplePluginManagerStatusIcons icons = GetOrAllocateIcons();
	        
	        List<SimplePluginManagerCollections.CPluginsPair> list_of_plugins = m_PluginCollections.BuildListOfPlugins();
	        foreach (var plugin_pair in list_of_plugins)
	        {
		        GetOrAllocatePluginFoldout(plugin_pair.Id).OnGui(m_Settings, m_PluginCollections, plugin_pair, icons, UpdatePluginCollections);
	        }
        }

        private void InitSettings()
        {
	        if (m_Settings == null)
	        {
		        List<SimplePluginManagerSettings> settings_files = SimplePluginManagerUnityUtils.FindAssetsByType<SimplePluginManagerSettings>();
		        if (settings_files.Count == 1)
		        {
			        m_Settings = settings_files.First();
			        UpdatePluginCollections();
		        }
		        else if(settings_files.Count == 0)
		        {
			        m_ErrorMessage = "Missing settings asset!";
		        }
		        else
		        {
			        m_ErrorMessage = "Multiple settings assets found, link settings manually!";
		        }
	        }
	        else
	        {
		        UpdatePluginCollections();
	        }
        }

        private void UpdatePluginCollections()
        {
	        if (m_PluginCollections == null)
	        {
		        m_PluginCollections = new SimplePluginManagerCollections();
	        }

	        m_ErrorMessage = m_PluginCollections.Update(m_Settings);
		}

        private SimplePluginManagerPluginFoldout GetOrAllocatePluginFoldout(string plugin_id)
        {
	        if (m_PluginFoldouts == null)
	        {
		        m_PluginFoldouts = new Dictionary<string, SimplePluginManagerPluginFoldout>();
	        }

	        if (!m_PluginFoldouts.ContainsKey(plugin_id))
	        {
		        m_PluginFoldouts.Add(plugin_id, new SimplePluginManagerPluginFoldout());
	        }

	        return m_PluginFoldouts[plugin_id];
        }

        private SimplePluginManagerStatusIcons GetOrAllocateIcons()
        {
	        return m_Icons ?? (m_Icons = new SimplePluginManagerStatusIcons());
        }
    }
}
#endif