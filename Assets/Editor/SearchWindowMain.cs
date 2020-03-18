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

        private SearchWindowQuestTab _questTab = new SearchWindowQuestTab();
        private SearchWindowSwitchTab _switchTab = new SearchWindowSwitchTab();
        
        protected override void EnableProcess()
        {
        } 
    
    
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/SearchWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            SearchWindowMain window = (SearchWindowMain)EditorWindow.GetWindow(typeof(SearchWindowMain));
            window.Show();
        }

        protected override void GUIProcess()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.02f);
            _selectedMode = (EMode)GUILayout.Toolbar((int)_selectedMode, System.Enum.GetNames(typeof(EMode)), "LargeButton");
            GUILayout.Space(position.width * 0.02f);
            GUILayout.EndHorizontal();

            switch(_selectedMode)
            {
                case EMode.Quest:
                    _questTab.GUIProcess(GetTabPosition());
                    break;
                case EMode.Switch:
                default:
                    _switchTab.GUIProcess(GetTabPosition());
                    break;
            }
        }

        protected override void FocusProcess()
        {
            var questDataList = SQLiteManager.Instance.GetAllQuestDataList();
            _questTab.FocusProcess(questDataList);
            
            var switchDescriptionDataList = SQLiteManager.Instance.GetAllSwitchDescriptionDataList();
            _switchTab.FocusProcess(switchDescriptionDataList);
        }
    
    
        private Rect GetTabPosition()
        {
            float padding = KMenubarPadding;
            Rect tabPosition = new Rect(0, padding, position.width, position.height - padding);
            return tabPosition;
        }
    
    }
}
