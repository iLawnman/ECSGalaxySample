using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Scaffold
{
    public enum SavePathMode
    {
        ProjectRelative,
        Absolute,
        PersistentDataPath
    }

    [CreateAssetMenu(fileName = "SaveSettings", menuName = "Scaffold/Save Settings", order = 0)]
    public class SaveSettings : ScriptableObject
    {
        [Header("Location")]
        public SavePathMode pathMode = SavePathMode.ProjectRelative;

        [Tooltip("Folder relative to project root (when PathMode = ProjectRelative), e.g. Assets/Scaffold/Configs")] 
        public string projectRelativeFolder = "Assets/Scaffold/Configs";

        [Tooltip("Absolute folder on disk (when PathMode = Absolute)")]
        public string absoluteFolder = "/tmp";

        [Tooltip("Default file name for scene config JSON")]
        public string defaultFileName = "SceneConfig.json";

        [Header("Components to Save")]
        [Tooltip("MonoScripts whose types derive from UnityEngine.Component and are considered in save process")] 
        public List<MonoScript> componentScripts = new List<MonoScript>();

        public string ResolveFolderAbsolute()
        {
            switch (pathMode)
            {
                case SavePathMode.ProjectRelative:
#if UNITY_EDITOR
                    // Convert project-relative to absolute by combining with project root
                    string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
                    return MakeSureFolderExists(Path.Combine(projectRoot, NormalizeSlashes(projectRelativeFolder)));
#else
                    return Application.dataPath;
#endif
                case SavePathMode.PersistentDataPath:
                    return MakeSureFolderExists(Application.persistentDataPath);
                case SavePathMode.Absolute:
                default:
                    return MakeSureFolderExists(NormalizeSlashes(absoluteFolder));
            }
        }

        public string ResolveDefaultFileAbsolute()
        {
            string folder = ResolveFolderAbsolute();
            return Path.Combine(folder, string.IsNullOrEmpty(defaultFileName) ? "SceneConfig.json" : defaultFileName);
        }

        public IReadOnlyList<Type> GetComponentTypes()
        {
            var result = new List<Type>();
            foreach (var script in componentScripts)
            {
                if (script == null) continue;
                var type = script.GetClass();
                if (type != null && typeof(Component).IsAssignableFrom(type))
                {
                    result.Add(type);
                }
            }
            return result;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Filter non-Component entries
            for (int i = componentScripts.Count - 1; i >= 0; i--)
            {
                var s = componentScripts[i];
                if (s == null) continue;
                var t = s.GetClass();
                if (t == null || !typeof(Component).IsAssignableFrom(t))
                {
                    componentScripts.RemoveAt(i);
                }
            }
        }
#endif

        private static string NormalizeSlashes(string path)
        {
            return string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');
        }

        private static string MakeSureFolderExists(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return folder;
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch
            {
                // ignore
            }
            return folder;
        }
    }
}