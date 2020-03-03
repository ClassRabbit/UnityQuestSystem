using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class GUITest : EditorWindow
{

    #region SwitchMakeData
    
    class SwitchMakeData
    {
        class SwitchCondition
        {
            class SwitchConditionBracket
            {
                public string QuestId { get; set; } = string.Empty;
                
            }
            
            List<SwitchConditionBracket> _bracketList = new List<SwitchConditionBracket>();
        }
    }
    

    #endregion


    private string search;
    private Vector2 scrollPosition;

    SwitchMakeData _switchMakeData = new SwitchMakeData();
    Vector2 ScrollPos = Vector2.zero;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/GUITest")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GUITest window = (GUITest)EditorWindow.GetWindow(typeof(GUITest));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("Click on the left style to copy its name to the clipboard", "label");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Find:");
        search = EditorGUILayout.TextField(search);
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //foreach (GUIStyle style in GUI.skin.customStyles)
        foreach (GUIStyle style in GUI.skin)
        {
            //filter
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                //Set parity background different background
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                GUILayout.Space(20);//Left white 20
                if (GUILayout.Button(style.name, style))
                {
                    //Store the name in the clipboard 
                    EditorGUIUtility.systemCopyBuffer = style.name; // "\"" + style.name + "\"";
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                GUILayout.EndHorizontal();
                GUILayout.Space(20);//White on the right 20
            }
        }

        GUILayout.EndScrollView();



    }

    
    
}