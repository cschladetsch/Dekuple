#if UNITY_EDITOR
namespace Dekuple.Editor
{
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.Serialization.Formatters.Binary;

    using UnityEditor;
    using UnityEngine;

    using Flow;

    using Debug = UnityEngine.Debug;

    public class PreferencesWindow
        : EditorWindow
    {
        private static Texture2D _logo;
        private static string _preferencesPath => Path.Combine(_preferencesDir, "Preferences.dks");
        private static string _preferencesDir => Path.Combine(Application.persistentDataPath, "Preferences/Dekuple/");
        private static string _logoPath => "Packages/com.cschladetsch.dekuple/Editor/Textures/Logo.png";

        private bool _changes;

        [MenuItem("Dekuple/Preferences")]
        private static void Init()
        {
            Load();

            _logo = (Texture2D)AssetDatabase.LoadAssetAtPath(_logoPath, typeof(Texture2D));
            var window = (PreferencesWindow)GetWindow(typeof(PreferencesWindow), true, "Dekuple Preferences", true);
            window.minSize = new Vector2(400, 200);
            window.Show();
        }

        private static void Save()
        {
            if (!Directory.Exists(_preferencesDir))
                Directory.CreateDirectory(_preferencesDir);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = File.Create(_preferencesPath);
            formatter.Serialize(fs, Preferences.Prefs);
            fs.Close();
            Debug.Log("Updated preferences for Dekuple.");
        }

        [InitializeOnLoadMethod]
        private static void Load()
        {
            if (!File.Exists(_preferencesPath))
                return;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = File.Open(_preferencesPath, FileMode.Open);
            Preferences.Prefs = (Preferences)formatter.Deserialize(fs);
            fs.Close();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (Preferences.Prefs == null)
                Preferences.Prefs = new Preferences();

            DrawLogo();
            DrawHeading();

            GUILayout.Space(6);

            Preferences.Prefs.UseViewBaseInspector =
                GUILayout.Toggle(Preferences.Prefs.UseViewBaseInspector, "Use Custom View Inspector");
            Preferences.Prefs.UseViewBaseHierarchy =
                GUILayout.Toggle(Preferences.Prefs.UseViewBaseHierarchy, "Enable Hierarchy Labels");

            GUILayout.Space(2);
            GUILayout.Label("Logging Level");
            Preferences.Prefs.LogLevel = GUILayout.Toolbar(Preferences.Prefs.LogLevel, typeof(ELogLevel).GetEnumNames());
            Preferences.Prefs.Verbosity = EditorGUILayout.IntSlider("Verbosity", Preferences.Prefs.Verbosity, 0, 100);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorGUI.EndChangeCheck())
                _changes = true;

            if (GUILayout.Button("Apply", GUILayout.MinWidth(100), GUILayout.Height(22)))
            {
                Save();
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

        private static void DrawLogo()
        {
            if (_logo != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(_logo, GUILayout.Width(200), GUILayout.Height(44));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(4);
            }
        }
    }
}
#endif
