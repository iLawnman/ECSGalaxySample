#if UNITY_EDITOR
using System.IO;
using Scaffold;
using Services;
using UnityEditor;
using UnityEngine;

public class SceneConfigWindow : EditorWindow
{
    private string lastDirectory;
    private SaveSettings settings;

    [MenuItem("Tools/Scene Config")] 
    public static void ShowWindow()
    {
        GetWindow<SceneConfigWindow>("Scene Config");
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Try load or create default settings asset
        if (settings == null)
        {
            settings = SaveSettingsUtility.LoadOrCreateSettingsAsset();
        }
#endif
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("JSON конфиг сцены", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        settings = (SaveSettings)EditorGUILayout.ObjectField("Save Settings", settings, typeof(SaveSettings), false);
        if (settings == null)
        {
            if (GUILayout.Button("Создать SaveSettings", GUILayout.Height(22)))
            {
#if UNITY_EDITOR
                settings = SaveSettingsUtility.LoadOrCreateSettingsAsset();
#endif
            }
        }
        else
        {
            EditorGUILayout.LabelField("Папка по умолчанию:", settings.ResolveFolderAbsolute());
            EditorGUILayout.LabelField("Файл по умолчанию:", settings.ResolveDefaultFileAbsolute());
            EditorGUILayout.LabelField("Компонентов для сохранения:", settings.componentScripts?.Count.ToString() ?? "0");
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Сохранить в JSON", GUILayout.Height(28)))
        {
            SaveToJson();
        }

        if (GUILayout.Button("Загрузить из JSON", GUILayout.Height(28)))
        {
            LoadFromJson();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Последняя папка:", string.IsNullOrEmpty(lastDirectory) ? "(не выбрана)" : lastDirectory);
    }

    private string GetDefaultDirectory()
    {
        if (!string.IsNullOrEmpty(lastDirectory)) return lastDirectory;
        if (settings != null)
        {
            var folder = settings.ResolveFolderAbsolute();
            if (!string.IsNullOrEmpty(folder)) return folder;
        }
        return Application.dataPath;
    }

    private void SaveToJson()
    {
        var config = SaveServiceLocator.Current.LoadSceneConfig();
        if (config == null)
        {
            EditorUtility.DisplayDialog("Нет данных", "SaveService вернул null.", "OK");
            return;
        }

        string defaultDir = GetDefaultDirectory();
        string defaultName = settings != null ? Path.GetFileName(settings.ResolveDefaultFileAbsolute()) : "SceneConfig.json";
        string path = EditorUtility.SaveFilePanel("Сохранить SceneConfig в JSON", defaultDir, Path.GetFileNameWithoutExtension(defaultName), "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(path, json);

        lastDirectory = Path.GetDirectoryName(path);
        Debug.Log($"SceneConfig сохранён: {path}");
    }

    private void LoadFromJson()
    {
        string defaultDir = GetDefaultDirectory();
        string path = EditorUtility.OpenFilePanel("Загрузить SceneConfig из JSON", defaultDir, "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        var config = JsonUtility.FromJson<Scaffold.SceneConfig>(json);
        if (config == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось распарсить JSON в SceneConfig.", "OK");
            return;
        }

        SaveServiceLocator.Current.SaveSceneConfig(config);

        lastDirectory = Path.GetDirectoryName(path);
        Debug.Log($"SceneConfig загружен из JSON и применён к SaveService: {path}");
    }
}
#endif