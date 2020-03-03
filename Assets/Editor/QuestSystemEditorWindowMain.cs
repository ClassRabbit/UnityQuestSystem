using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace QuestSystem
{
    
    public class QuestSystemEditorWindowMain : EditorWindow
    {
        enum EMode
        {
            Quest = 0,
            Switch = 1
        }

        private const float KToolbarPadding = 15;
        private const float KMenubarPadding = 32;
    
        private EMode _selectedMode = EMode.Quest;
        private int _selectedObjectType = 0;

        private QuestSystemEditorWindowQuestTab _questTab;
    
        private void OnEnable()
        {

            Rect subPos = GetSubWindowArea();
            if (_questTab == null)
            {
                _questTab = new QuestSystemEditorWindowQuestTab();
            }
            _questTab.OnEnable();
        } 
    
    
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/SwitchEditorWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            QuestSystemEditorWindowMain window = (QuestSystemEditorWindowMain)EditorWindow.GetWindow(typeof(QuestSystemEditorWindowMain));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            _selectedMode = (EMode)GUILayout.Toolbar((int)_selectedMode, new string[]{ "Quest", "Switch" }, "LargeButton");
            GUILayout.EndHorizontal();
            
            
            switch(_selectedMode)
            {
                case EMode.Quest:
                    _questTab.OnGUI(GetSubWindowArea());
                    break;
                case EMode.Switch:
                default:
                    break;
            }
        }
    
    
        private Rect GetSubWindowArea()
        {
            float padding = KMenubarPadding;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }

    
    
    }
}
