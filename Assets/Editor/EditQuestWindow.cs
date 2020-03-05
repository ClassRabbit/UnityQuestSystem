using System;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditQuestWindow : QuestSystemWindow
    {
        private string _questId = string.Empty;
        private string _description = string.Empty;
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/EditQuestWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
            window.Show();
        }

        protected override void GUIProcess()
        {
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("QuestId ");
                _questId = GUILayout.TextField(_questId, GUILayout.Width(Screen.width * 0.8f));
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Label("Description ");
            _description = GUILayout.TextArea(_description, GUILayout.Height(100));
            
            if (GUILayout.Button("Create"))
            {
                //아이디 채크
                if (string.IsNullOrEmpty(_questId))
                {
                    Debug.LogError("QuestId는 빈칸일 수 없다.");
                }
                else if (null != SQLiteManager.Instance.GetQuestData(_questId))
                {
                    Debug.LogError("중복된 QuestId");
                }
                else
                {
                    SQLiteManager.Instance.CreateQuestData(_questId, _description);
                    Debug.Log("만드는데 성공");
                }
            }
        }

    
    }
}