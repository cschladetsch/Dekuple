namespace Dekuple.Editor
{
    [System.Serializable]
    public class Preferences
    {
        [System.NonSerialized]
        public static Preferences Prefs;

        public bool UseViewBaseInspector;
        public bool UseViewBaseHierarchy;
        public int LogLevel;
        public int Verbosity;
    }
}
