#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TemplateMenu
{
    private const string _templatesPath = @"Packages\Dekuple\Editor\Templates";
    private static string _viewClass;
    private static string _viewInterface;
    private static string _agentClass;
    private static string _agentInterface;
    private static string _modelClass;
    private static string _modelInterface;

    [InitializeOnLoadMethod]
    private static void Init()
    {
//        var workDir = Environment.GetEnvironmentVariable("WORK_DIR", EnvironmentVariableTarget.User);
//        if (workDir == null)
//        {
//            //Debug.Log("Please set your WORK_DIR system environment variable.");
//            workDir = "w";
////            return;
//        }
        var workDir = "w:\\";
        var templatesPath = Path.Combine(workDir, _templatesPath);
        _viewClass = Path.Combine(templatesPath, "ViewTemplate.cs.txt");
        _viewInterface = Path.Combine(templatesPath, "IViewTemplate.cs.txt");
        _agentClass = Path.Combine(templatesPath, "AgentTemplate.cs.txt");
        _agentInterface = Path.Combine(templatesPath, "IAgentTemplate.cs.txt");
        _modelClass = Path.Combine(templatesPath, "ModelTemplate.cs.txt");
        _modelInterface = Path.Combine(templatesPath, "IModelTemplate.cs.txt");
    }

    [MenuItem("Dekuple/Create Entity Scripts", false, 0)]
    public static void CreateEntity()
        => CreateEntityPopup.Init(_viewClass, _agentClass, _modelClass, _viewInterface, _agentInterface, _modelInterface);

    [MenuItem("Assets/Create/Dekuple/C# View Script", false, 0)]
    public static void CreateView()
        => ClassNamePopup.Init(_viewClass, "View");

    [MenuItem("Assets/Create/Dekuple/C# Agent Script", false, 0)]
    public static void CreateAgent()
        => ClassNamePopup.Init(_agentClass, "Agent");

    [MenuItem("Assets/Create/Dekuple/C# Model Script", false, 0)]
    public static void CreateModel()
        => ClassNamePopup.Init(_modelClass, "Model");

    public static void CreateFile(string templateText, string name, string path)
    {
        var viewName = $"{name}View";
        var agentName = $"{name}Agent";
        var modelName = $"{name}Model";

        var template = File.ReadAllText(templateText);
        var output = template.Replace("$VIEWNAME$", viewName);
        output = output.Replace("$AGENTNAME$", agentName);
        output = output.Replace("$MODELNAME$", modelName);
        File.WriteAllText(path, output);

        AssetDatabase.Refresh();
    }

    public static string GetDirectory()
    {
        var obj = Selection.activeObject;
        return obj == null ? "Assets" : AssetDatabase.GetAssetPath(obj.GetInstanceID());
    }
}

public class CreateEntityPopup
    : EditorWindow
{
    private static string _viewClass;
    private static string _viewInterface;
    private static string _agentClass;
    private static string _agentInterface;
    private static string _modelClass;
    private static string _modelInterface;

    private static string _viewClassPath;
    private static string _viewInterfacePath;
    private static string _agentClassPath;
    private static string _agentInterfacePath;
    private static string _modelClassPath;
    private static string _modelInterfacePath;

    private string _input;

    public static void Init(string viewClass, string agentClass, string modelClass, string viewInterface, string agentInterface, string modelInterface)
    {
        _viewClass = viewClass;
        _viewInterface = viewInterface;
        _agentClass = agentClass;
        _agentInterface = agentInterface;
        _modelClass = modelClass;
        _modelInterface = modelInterface;

        var parent = Directory.GetParent(Application.dataPath).FullName;
        _viewClassPath = parent + "/Assets/App/Views/Impl";
        _viewInterfacePath = parent + "/Assets/App/Views";
        _agentClassPath = parent + "/Assets/App/Agents/Impl";
        _agentInterfacePath = parent + "/Assets/App/Agents";
        _modelClassPath = parent + "/Assets/App/Models/Impl";
        _modelInterfacePath = parent + "/Assets/App/Models";

        CreateEntityPopup window = CreateInstance<CreateEntityPopup>();
        window.name = "Create Entity";
        window.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.Label($"Create Dekuple Entity", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 100;

        _input = EditorGUILayout.TextField("Name", _input);

        GUILayout.Space(6);

        var pathStyle = new GUIStyle("ToolbarTextField") { stretchWidth = true };
        EditorGUIUtility.labelWidth = 150f;
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("View Class Path: ");
        if (GUILayout.Button(_viewClassPath, pathStyle))
            _viewClassPath = EditorUtility.OpenFolderPanel("Select folder to create view.", _viewClassPath, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("View Interface Path: ");
        if (GUILayout.Button(_viewInterfacePath, pathStyle))
            _viewInterfacePath = EditorUtility.OpenFolderPanel("Select folder to create view.", _viewInterfacePath, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Agent Class Path: ");
        if (GUILayout.Button(_agentClassPath, pathStyle))
            _agentClassPath = EditorUtility.OpenFolderPanel("Select folder to create view.", _agentInterfacePath, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Agent Interface Path: ");
        if (GUILayout.Button(_agentInterfacePath, pathStyle))
            _agentInterfacePath = EditorUtility.OpenFolderPanel("Select folder to create view.", _agentInterfacePath, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Model Class Path: ");
        if (GUILayout.Button(_modelClassPath, pathStyle))
            _modelClassPath = EditorUtility.OpenFolderPanel("Select folder to create view.", _modelClassPath, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Model Interface Path: ");
        if (GUILayout.Button(_modelInterfacePath, pathStyle))
            _modelInterfacePath = EditorUtility.OpenFolderPanel("Select folder to create view.", _modelInterfacePath, "");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            TemplateMenu.CreateFile(_viewInterface, _input, $"{_viewInterfacePath}/I{_input}View.cs");
            TemplateMenu.CreateFile(_agentInterface, _input, $"{_agentInterfacePath}/I{_input}Agent.cs");
            TemplateMenu.CreateFile(_modelInterface, _input, $"{_modelInterfacePath}/I{_input}Model.cs");
            TemplateMenu.CreateFile(_viewClass, _input, $"{_viewClassPath}/{_input}View.cs");
            TemplateMenu.CreateFile(_agentClass, _input, $"{_agentClassPath}/{_input}Agent.cs");
            TemplateMenu.CreateFile(_modelClass, _input, $"{_modelClassPath}/{_input}Model.cs");
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}

public class ClassNamePopup
    : EditorWindow
{
    private static string _template;
    private static string _suffix;
    private string _input;

    public static void Init(string template, string suffix)
    {
        _template = template;
        _suffix = suffix;
        ClassNamePopup window = CreateInstance<ClassNamePopup>();
        window.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.Label($"Create Dekuple {_suffix}", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 100;
        _input = EditorGUILayout.TextField("Base Name", _input);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            TemplateMenu.CreateFile(_template, _input, $"{Directory.GetParent(Application.dataPath).FullName}/{TemplateMenu.GetDirectory()}/{_input}{_suffix}.cs");
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
#endif
