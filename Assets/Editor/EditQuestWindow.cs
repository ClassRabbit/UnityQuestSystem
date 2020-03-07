using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditQuestWindow : QuestSystemEditWindow
    {
        enum EConfirmState
        {
            None,
            CreateSuccess,
            CreateFailEmptyId,
            CreateFailOverlapId,
            UpdateSuccess,
            DeleteSuccess,
            DeleteFailUseSwitch
        }
        
        private int KSpace = 10;
        QuestData _questData = new QuestData();
        
        private EConfirmState _confirmState = EConfirmState.None;
        
        protected override void ConfirmWindowProcess()
        {
            GUILayout.Label("수정되었습니다.");
            if (GUILayout.Button("Confirm"))
            {
                _confirmState = EConfirmState.None;
            }
        }
    
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
            if (questData == null)
            {
                return;
            }
            
            IsUpdate = true;
            _questData = questData;
        }

        protected override void GUIProcess()
        {
            GUILayout.Space(KSpace);
            
            GUILayout.Label("Create QuestData", "DefaultCenteredLargeText");
            
            GUILayout.Space(KSpace);
            
            EditorGUI.BeginDisabledGroup(_confirmState != EConfirmState.None);
            {
                //퀘스트 아이디
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(IsUpdate);
                    {
                        EditorGUILayout.PrefixLabel("QuestId ");
                        _questData.QuestId = GUILayout.TextField(_questData.QuestId);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
                
                //퀘스트 설명
                GUILayout.Space(KSpace);
                GUILayout.Label("Description ");
                _questData.Description = GUILayout.TextArea(_questData.Description, GUILayout.Height(position.height - 125 - KSpace));
                GUILayout.Space(KSpace);
            
                GUILayout.BeginHorizontal();
                GUILayout.Space(position.width * 0.2f);

                if (IsUpdate)
                {
                
                    if (GUILayout.Button("Update"))
                    {
                        SQLiteManager.Instance.UpdateQuestData(_questData);
                        _confirmState = EConfirmState.UpdateSuccess;
                        Debug.Log("업데이트 성공");
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        //SwitchData 구성요소인지 체크
                        
                        _confirmState = EConfirmState.DeleteSuccess;
                    }
                }
                else
                {
                    if (GUILayout.Button("Create"))
                    {
                        //아이디 채크
                        if (string.IsNullOrEmpty(_questData.QuestId))
                        {
                            Debug.LogError("QuestId는 빈칸일 수 없다.");
                            _confirmState = EConfirmState.CreateFailEmptyId;
                        }
                        else if (null != SQLiteManager.Instance.GetQuestData(_questData.QuestId))
                        {
                            Debug.LogError("중복된 QuestId");
                            _confirmState = EConfirmState.CreateFailOverlapId;
                        }
                        else
                        {
                            SQLiteManager.Instance.CreateQuestData(_questData);
                            _confirmState = EConfirmState.CreateSuccess;
                            Debug.Log("만드는데 성공");
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            if (_confirmState != EConfirmState.None)
            {
                DrawConfirmWindow(string.Empty);
            }
            
            GUILayout.Space(position.width * 0.2f);
            GUILayout.EndHorizontal();
            GUILayout.Space(KSpace);
        }
        
        

    
    }
}