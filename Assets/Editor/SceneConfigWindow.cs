#if UNITY_EDITOR
using System.IO;
using Scaffold;
using Services;
using UnityEditor;
using UnityEngine;

public class SceneConfigWindow : EditorWindow
{
    private string lastDirectory;

    [MenuItem("Tools/Scene Config")] 
    public static void ShowWindow()
    {
        GetWindow<SceneConfigWindow>("Scene Config");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("JSON конфиг сцены", EditorStyles.boldLabel);
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

    private void SaveToJson()
    {
        var config = SaveServiceLocator.Current.LoadSceneConfig();
        if (config == null)
        {
            EditorUtility.DisplayDialog("Нет данных", "SaveService вернул null.", "OK");
            return;
        }

        string defaultDir = string.IsNullOrEmpty(lastDirectory) ? Application.dataPath : lastDirectory;
        string path = EditorUtility.SaveFilePanel("Сохранить SceneConfig в JSON", defaultDir, "SceneConfig", "json");
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
        string defaultDir = string.IsNullOrEmpty(lastDirectory) ? Application.dataPath : lastDirectory;
        string path = EditorUtility.OpenFilePanel("Загрузить SceneConfig из JSON", defaultDir, "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        var config = JsonUtility.FromJson<SceneConfig>(json);
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