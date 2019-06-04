using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TemplateMenu
{
    private const string _templatesPath = @"Packages\Dekuple\Editor\Templates";
    private static string _viewTemplate;
    private static string _agentTemplate;
    private static string _modelTemplate;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        var workDir = Environment.GetEnvironmentVariable("WORK_DIR", EnvironmentVariableTarget.User);
        if (workDir == null)
        {
            Debug.Log("Please set your WORK_DIR system environment variable.");
            return;
        }

        var templatesPath = Path.Combine(workDir, _templatesPath);
        _viewTemplate = Path.Combine(templatesPath, "ViewTemplate.cs.txt");
        _agentTemplate = Path.Combine(templatesPath, "AgentTemplate.cs.txt");
        _modelTemplate = Path.Combine(templatesPath, "ModelTemplate.cs.txt");
    }

    [MenuItem("Assets/Create/Dekuple/C# View Script", false, 0)]
    public static void CreateView()
        => InputPopup.Init(_viewTemplate, "View");

    [MenuItem("Assets/Create/Dekuple/C# Agent Script", false, 0)]
    public static void CreateAgent()
        => InputPopup.Init(_agentTemplate, "Agent");

    [MenuItem("Assets/Create/Dekuple/C# Model Script", false, 0)]
    public static void CreateModel()
        => InputPopup.Init(_modelTemplate, "Model");

    public static void CreateFile(string templateText, string name, string fileName)
    {
        var viewName = $"{name}View";
        var agentName = $"{name}Agent";
        var modelName = $"{name}Model";

        var destinationDir = GetDirectory();
        var path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, destinationDir, fileName);
        path = path.Replace('\\', '/');
        Debug.Log(path);
        var template = File.ReadAllText(templateText);

        var output = template.Replace("$VIEWNAME$", viewName);
        output = output.Replace("$AGENTNAME$", agentName);
        output = output.Replace("$MODELNAME$", modelName);
        Debug.Log(path);
        File.WriteAllText(path, output);

        AssetDatabase.Refresh();
    }

    private static string GetDirectory()
    {
        var obj = Selection.activeObject;
        return obj == null ? "Assets" : AssetDatabase.GetAssetPath(obj.GetInstanceID());
    }
}

public class InputPopup : EditorWindow
{
    private static string _template;
    private static string _suffix;
    private string _input;

    public static void Init(string template, string suffix)
    {
        _template = template;
        _suffix = suffix;
        InputPopup window = CreateInstance<InputPopup>();
        window.position = new Rect(Screen.width / 2f+ 250, Screen.height / 2f + 50, 300, 67);
        window.ShowPopup();
    }

    void OnGUI()
    {
        GUILayout.Label($"Create Dekuple {_suffix}", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 100;
        _input = EditorGUILayout.TextField("Base Name", _input);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            TemplateMenu.CreateFile(_template, _input, $"{_input}{_suffix}.cs");
            this.Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
        GUILayout.EndHorizontal();
    }
}