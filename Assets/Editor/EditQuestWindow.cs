using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditQuestWindow : QuestSystemWindow
    {
        private int KSpace = 10;
        private string _questId = string.Empty;
        private string _description = string.Empty;
        
        public bool IsUpdate = false;
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/EditQuestWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
            window.Show();
        }

        internal void UpdateQuestData(QuestData questData)
        {
            IsUpdate = true;
            _questId = questData.QuestId;
            _description = questData.Description;
        }

        protected override void GUIProcess()
        {
            GUILayout.Space(KSpace);
            
            GUILayout.Label("Create QuestData", "DefaultCenteredLargeText");
            
            GUILayout.Space(KSpace);
            
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(IsUpdate);
                EditorGUILayout.PrefixLabel("QuestId ");
                _questId = GUILayout.TextField(_questId);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(KSpace);
            
            GUILayout.Label("Description ");
            _description = GUILayout.TextArea(_description, GUILayout.Height(position.height - 125 - KSpace));
            
            GUILayout.Space(KSpace);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.2f);
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
            GUILayout.Space(position.width * 0.2f);
            GUILayout.EndHorizontal();
            GUILayout.Space(KSpace);
        }

    
    }
}