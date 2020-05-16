#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SimplePluginManager
{
	public class SimplePluginManagerPluginFoldout
	{
		private bool m_IsExpanded = false;

		public void OnGui(
			SimplePluginManagerSettings                 settings,
			SimplePluginManagerCollections              collections,
			SimplePluginManagerCollections.CPluginsPair plugins_pair,
			SimplePluginManagerStatusIcons              icons,
			Action                                      on_collections_changed
		)
		{
			// gather resources
			SimplePluginManagerPlugin repository_plugin = plugins_pair.RepositoryPlugin;
			SimplePluginManagerPlugin project_plugin    = plugins_pair.ProjectPlugin;
			SimplePluginManagerPlugin plugin            = project_plugin ?? repository_plugin;

			string repository_version = repository_plugin == null ? "not available" : repository_plugin.Version;
			string project_version    = project_plugin    == null ? "not installed" : project_plugin.Version;

			bool is_any_plugin_dependent_on_this = collections.IsAnyProjectPluginDependentOn(plugin.Id);
			bool is_dependency_error             = collections.IsDependencyError(plugin);
			bool is_version_error                = (repository_plugin?.ParseVersion.IsError ?? false) || (project_plugin?.ParseVersion.IsError ?? false);
			bool is_error = is_dependency_error || is_version_error;
	        
			// layout
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(icons.GetPluginStatusIcon(repository_plugin, project_plugin), GUILayout.Width(20), GUILayout.Height(20));
					GUILayout.Label($"{plugin.Name} ({project_version})",                         EditorStyles.boldLabel);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical(GUILayout.Width(768));
				{
					// foldout
					EditorGUILayout.BeginHorizontal();
			        
					m_IsExpanded = EditorGUILayout.Foldout(m_IsExpanded, "") || is_error;
					if (!m_IsExpanded)
					{
						GuiButtons(settings, collections, repository_plugin, project_plugin, is_any_plugin_dependent_on_this, is_error, on_collections_changed);
					}
					EditorGUILayout.EndHorizontal();

					if (m_IsExpanded)
					{
						// description and info
						GUIStyle rich_text_style = new GUIStyle(GUI.skin.label);
						rich_text_style.richText = true;
						GUILayout.Label(plugin.Description, rich_text_style);

						GuiPluginDependencies(plugin, collections, icons);
						GUILayout.Space(8);
						GUILayout.Label("Id:"          + plugin.Id);
						GUILayout.Label("Repository: " + repository_version);

						// errors
						if (repository_plugin != null && !string.IsNullOrEmpty(repository_plugin.Error))
						{
							SimplePluginManagerUIUtils.ErrorLabel(repository_plugin.Error);
						}

						if (project_plugin != null && !string.IsNullOrEmpty(project_plugin.Error))
						{
							SimplePluginManagerUIUtils.ErrorLabel(project_plugin.Error);
						}

						if (is_version_error)
						{
							SimplePluginManagerUIUtils.ErrorLabel("Plugin has invalid version format!");
						}

						if (is_dependency_error)
						{
							SimplePluginManagerUIUtils.ErrorLabel("Plugin is dependent on non existing plugin!");
						}

						// buttons
						if (
							(repository_plugin == null || string.IsNullOrEmpty(repository_plugin.Error))
							&&
							(project_plugin == null || string.IsNullOrEmpty(project_plugin.Error))
						)
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Label(""); //formatting
								GuiButtons(settings, collections, repository_plugin, project_plugin, is_any_plugin_dependent_on_this, is_error, on_collections_changed);
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
	        
			SimplePluginManagerUIUtils.Line();
		}
		
		private void GuiPluginDependencies(
			SimplePluginManagerPlugin      plugin,
			SimplePluginManagerCollections collections, 
			SimplePluginManagerStatusIcons icons
		)
		{
			if (plugin.Dependencies != null && plugin.Dependencies.Count > 0)
			{
				GUILayout.Space(8);
				GUILayout.Label("Required plugins:", EditorStyles.boldLabel);

				foreach (string dependency in plugin.Dependencies)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label(
						icons.GetDependencyStatusIcon(
							collections.IsInProject(dependency),
							collections.IsInRepository(dependency)
						),
						GUILayout.Width(20)
					);
					GUILayout.Label(collections.GetPluginNameAndVersion(dependency));
					GUILayout.EndHorizontal();
				}
			}
		}
		
		private void GuiButtons(
			SimplePluginManagerSettings    settings,
			SimplePluginManagerCollections collections,
			SimplePluginManagerPlugin      repository_plugin,
			SimplePluginManagerPlugin      project_plugin,
			bool                           is_any_plugin_dependent_on_this,
			bool                           is_error,
			Action                         on_collections_changed
		)
		{
			if (repository_plugin != null)
			{
				if (project_plugin == null)
				{
					if (!is_error && GUILayout.Button("Install", GUILayout.Width(256)))
					{
						collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
						repository_plugin.CopyFromRepositoryToProject(settings);
						on_collections_changed();
						AssetDatabase.Refresh();
					}
			        
					GUILayout.Label(" ", GUILayout.Width(256));
				}
				else
				{
					SimplePluginManagerVersion repository_version_number = repository_plugin.ParseVersion;
					SimplePluginManagerVersion project_version_number    = project_plugin.ParseVersion;
					if (project_version_number < repository_version_number)
					{
						if (!is_error && GUILayout.Button("Upgrade", GUILayout.Width(256)))
						{
							collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
							repository_plugin.CopyFromRepositoryToProject(settings);
							on_collections_changed();
							AssetDatabase.Refresh();
						}

						if (is_any_plugin_dependent_on_this)
						{
							GUILayout.Label("Required by other plugin.", GUILayout.Width(256));
						}
						else if(GUILayout.Button("Uninstall", GUILayout.Width(256)))
						{
							project_plugin.RemoveFromProject();
							on_collections_changed();
							AssetDatabase.Refresh();
						}
					}
					else if (project_version_number == repository_version_number)
					{
						GUILayout.Label("Up to date.", EditorStyles.boldLabel, GUILayout.Width(256));

						if (is_any_plugin_dependent_on_this)
						{
							GUILayout.Label("Required by other plugin.", GUILayout.Width(256));
						}
						else if(GUILayout.Button("Uninstall", GUILayout.Width(256)))
						{
							project_plugin.RemoveFromProject();
							on_collections_changed();
							AssetDatabase.Refresh();
						}
					}
					else
					{
						if (!is_error && GUILayout.Button("Downgrade", GUILayout.Width(256)))
						{
							collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
							repository_plugin.CopyFromRepositoryToProject(settings);
							on_collections_changed();
							AssetDatabase.Refresh();
						}

						if (!is_error && GUILayout.Button("Copy to repository", GUILayout.Width(256)))
						{
							project_plugin.CopyFromProjectToRepository(settings);
							on_collections_changed();
							AssetDatabase.Refresh();
						}
					}
				}
			}
			else
			{
				if (project_plugin != null)
				{
					if (!is_error && GUILayout.Button("Copy to repository", GUILayout.Width(256)))
					{
						project_plugin.CopyFromProjectToRepository(settings);
						on_collections_changed();
						AssetDatabase.Refresh();
					}
				}
			}
		}
		
	}
}
#endif