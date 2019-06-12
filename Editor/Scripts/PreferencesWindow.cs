using System.IO;
using Dekuple;
using Flow;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
//using Newtonsoft.Json;

public class PreferencesWindow
    : EditorWindow
{
    private static Texture2D _liminalLogo;
    private const string _preferencesPath = "Preferences/Dekuple/Preferences.json";
    private const string _preferencesDir = "Preferences/Dekuple/";
    private bool _changes;

    [MenuItem("Liminal/Dekuple/Preferences")]
    private static void Init()
    {
        LoadPreferences();

        _liminalLogo = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/Dekuple/Editor/Textures/LiminalLogo.png", typeof(Texture2D));
        var window = (PreferencesWindow)GetWindow(typeof(PreferencesWindow), true, "Liminal Preferences", true);
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    [InitializeOnLoadMethod]
    private static void LoadPreferences()
    {
        if (!File.Exists(_preferencesPath))
            return;

        //Preferences.Prefs = JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(_preferencesPath));
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        if (Preferences.Prefs == null)
            Preferences.Prefs = new Preferences();

        DrawLogo();
        DrawHeading();

        GUILayout.Space(6);

        Preferences.Prefs.UseViewBaseInspector = GUILayout.Toggle(Preferences.Prefs.UseViewBaseInspector, "Use Custom View Inspector");
        Preferences.Prefs.UseViewBaseHierarchy = GUILayout.Toggle(Preferences.Prefs.UseViewBaseHierarchy, "Enable Hierarchy Labels");

        GUILayout.Space(2);
        GUILayout.Label("Logging Level");
        Preferences.Prefs.LogLevel = (ELogLevel)GUILayout.Toolbar((int)Preferences.Prefs.LogLevel, typeof(ELogLevel).GetEnumNames());

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (EditorGUI.EndChangeCheck())
            _changes = true;

        if (GUILayout.Button("Apply", GUILayout.MinWidth(100), GUILayout.Height(22)))
        {
            SaveChanges();
            _changes = false;
        }

        if (_changes)
        {
            var rect = GUILayoutUtility.GetLastRect();
            GUI.Label(rect, GUIContent.none, "LightmapEditorSelectedHighlight");
        }

        GUILayout.Space(2);
        GUILayout.EndHorizontal();
        GUILayout.Space(6);
    }

    private static void DrawHeading()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Dekuple Preferences", new GUIStyle("AM MixerHeader") { alignment = TextAnchor.MiddleCenter });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static void SaveChanges()
    {
        if (!Directory.Exists(_preferencesDir))
            Directory.CreateDirectory(_preferencesDir);

        //var jsonString = JsonConvert.SerializeObject(Preferences.Prefs, Formatting.Indented);
        //File.WriteAllText(_preferencesPath, jsonString);
        Debug.Log("Updated preferences for Dekuple.");
    }

    private static void DrawLogo()
    {
        if (_liminalLogo != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_liminalLogo, GUILayout.Width(200), GUILayout.Height(44));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Space(4);
        }
    }
}

public class Preferences
{
    public static Preferences Prefs;

    public bool UseViewBaseInspector;
    public ELogLevel LogLevel;
    public bool UseViewBaseHierarchy;
}
