#if UNITY_EDITOR
using System.IO;
using Scaffold;
using UnityEditor;
using UnityEngine;

public static class SaveSettingsUtility
{
    private const string DefaultAssetPath = "Assets/Settings/SaveSettings.asset";

    public static SaveSettings LoadOrCreateSettingsAsset(string assetPath = DefaultAssetPath)
    {
        var settings = AssetDatabase.LoadAssetAtPath<SaveSettings>(assetPath);
        if (settings != null) return settings;

        string folder = Path.GetDirectoryName(assetPath);
        if (!AssetDatabase.IsValidFolder(folder))
        {
            CreateFoldersRecursively(folder);
        }

        settings = ScriptableObject.CreateInstance<SaveSettings>();
        AssetDatabase.CreateAsset(settings, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return settings;
    }

    [MenuItem("Tools/Save Settings/Open Asset")]
    public static void OpenSettingsAsset()
    {
        var settings = LoadOrCreateSettingsAsset();
        Selection.activeObject = settings;
        EditorGUIUtility.PingObject(settings);
    }

    private static void CreateFoldersRecursively(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath)) return;
        var parts = folderPath.Replace('\\', '/').Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}
#endif