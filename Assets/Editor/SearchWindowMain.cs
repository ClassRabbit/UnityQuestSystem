using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace QuestSystem
{
    
    public class SearchWindowMain : QuestSystemWindow
    {
        enum EMode
        {
            Quest = 0,
            Switch = 1
        }

        
        private const float KMenubarPadding = 32;
    
        private EMode _selectedMode = EMode.Quest;

        private SearchWindowQuestTab _questTab = null;

        protected override void EnableProcess()
        {
            Rect subPosition = GetSubWindowPosition();
            if (_questTab == null)
            {
                _questTab = new SearchWindowQuestTab();
            }
            _questTab.EnableProcess(subPosition);
            
        } 
    
    
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/SearchWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            SearchWindowMain window = (SearchWindowMain)EditorWindow.GetWindow(typeof(SearchWindowMain));
            window.Show();
        }

        void OnGUI()
        {

            if (string.IsNullOrEmpty(DatabaseName))
            {
                return;
            }
            
            GUILayout.BeginHorizontal();
            _selectedMode = (EMode)GUILayout.Toolbar((int)_selectedMode, System.Enum.GetNames(typeof(EMode)), "LargeButton");
            GUILayout.EndHorizontal();
            
            
            switch(_selectedMode)
            {
                case EMode.Quest:
                    _questTab.OnGUI(GetSubWindowPosition());
                    break;
                case EMode.Switch:
                default:
                    break;
            }
        }
    
    
        private Rect GetSubWindowPosition()
        {
            float padding = KMenubarPadding;
            Rect subPossition = new Rect(0, padding, position.width, position.height - padding);
            return subPossition;
        }

    
    
    }
}
