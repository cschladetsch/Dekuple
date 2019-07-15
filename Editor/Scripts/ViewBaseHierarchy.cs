#if UNITY_EDITOR
using Dekuple.View;
using Dekuple.Editor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ViewBaseHierarchy
{
    static ViewBaseHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem_CB;
    }

    private static void HierarchyWindowItem_CB(int selectionID, Rect rect)
    {
        if (Preferences.Prefs != null)
        {
            if (!Preferences.Prefs.UseViewBaseHierarchy)
            {
                return;
            }
        }

        Object o = EditorUtility.InstanceIDToObject(selectionID);
        if (o == null) return;
        if (((GameObject)o).GetComponent<IViewBase>() == null) return;
        IViewBase viewBase = ((GameObject)o).GetComponent<IViewBase>();
        if (Event.current.type != EventType.Repaint) return;

        rect.x = rect.max.x;
        rect.width = 48;
        rect.x -= 14;
        GUI.Label(rect, EditorGUIUtility.IconContent("NetworkLobbyPlayer Icon"));
        Color inactiveColor = new Color(0.75f, 0.75f, 0.75f, 0.45f);
        Color activeColor = Color.white;

        GUI.color = viewBase.AgentBase == null ? inactiveColor : activeColor;

        rect.x -= 14;
        GUI.Label(rect, EditorGUIUtility.IconContent("NetworkIdentity Icon"));
        GUI.color = viewBase.AgentBase?.BaseModel == null ? inactiveColor : activeColor;

        rect.x -= 14;
        GUI.Label(rect, EditorGUIUtility.IconContent("NetworkProximityChecker Icon"));
        GUI.color = Color.white;

        EditorApplication.RepaintHierarchyWindow();
    }
}

#endif
