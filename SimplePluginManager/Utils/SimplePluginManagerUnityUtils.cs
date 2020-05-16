#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimplePluginManager
{
    public static class SimplePluginManagerUnityUtils
    {
        public static Texture2D LoadIcon(string icon)
        {
            string[] guids = AssetDatabase.FindAssets(icon);
            if (guids.Length == 1)
            {
                string    assetPath = AssetDatabase.GUIDToAssetPath( guids[0] );
                Texture2D asset     = AssetDatabase.LoadAssetAtPath<Texture2D>( assetPath );
                if (asset != null)
                {
                    return asset;
                }
            }

            return null;
        }

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T>  assets = new List<T>();
            string[] guids  = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T      asset     = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static void AddGlobalDefine(string define)
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

            if (!symbols.Contains(define))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + (string.IsNullOrEmpty(symbols) ? "" : ";") + define);
            }
        }

        public static void RemoveGlobalDefine(string define)
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

            if (symbols.Contains(define))
            {
                symbols = symbols.Replace(";" + define, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
            }
        }
    }
}
#endif