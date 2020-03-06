using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditQuestWindow : QuestSystemWindow
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
        private string _questId = string.Empty;
        private string _description = string.Empty;
        
        private bool IsUpdate = false;
        private EConfirmState _confirmState = EConfirmState.None;
        
        public Rect windowRect = new Rect(0, 0, 400, 300);
        void ConfirmWindowProcess(int unusedWindowID)
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
            IsUpdate = true;
            _questId = questData.QuestId;
            _description = questData.Description;
        }

        protected override void GUIProcess()
        {
            GUILayout.Space(KSpace);
            
            GUILayout.Label("Create QuestData", "DefaultCenteredLargeText");
            
            GUILayout.Space(KSpace);
            
            EditorGUI.BeginDisabledGroup(_confirmState != EConfirmState.None);
            {
            
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(IsUpdate);
                    {
                        EditorGUILayout.PrefixLabel("QuestId ");
                        _questId = GUILayout.TextField(_questId);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(KSpace);
                
                GUILayout.Label("Description ");
                _description = GUILayout.TextArea(_description, GUILayout.Height(position.height - 125 - KSpace));
                GUILayout.Space(KSpace);
            
                GUILayout.BeginHorizontal();
                GUILayout.Space(position.width * 0.2f);

                if (IsUpdate)
                {
                
                    if (GUILayout.Button("Update"))
                    {
                        SQLiteManager.Instance.UpdateQuestData(_questId, _description);
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
                        if (string.IsNullOrEmpty(_questId))
                        {
                            Debug.LogError("QuestId는 빈칸일 수 없다.");
                            _confirmState = EConfirmState.CreateFailEmptyId;
                        }
                        else if (null != SQLiteManager.Instance.GetQuestData(_questId))
                        {
                            Debug.LogError("중복된 QuestId");
                            _confirmState = EConfirmState.CreateFailOverlapId;
                        }
                        else
                        {
                            SQLiteManager.Instance.CreateQuestData(_questId, _description);
                            _confirmState = EConfirmState.CreateSuccess;
                            Debug.Log("만드는데 성공");
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            
            
            
            if (_confirmState != EConfirmState.None)
            {
                Rect rect = new Rect(0, 0, position.width, position.height);
                EditorGUI.DrawRect(rect, new Color32(0, 0, 0, 122));
                BeginWindows();
                windowRect.x = (position.width - windowRect.width) / 2;
                windowRect.y = (position.height - windowRect.height) / 2;
                windowRect = GUILayout.Window(1, windowRect, ConfirmWindowProcess, "");
                EndWindows();
            }
            
            GUILayout.Space(position.width * 0.2f);
            GUILayout.EndHorizontal();
            GUILayout.Space(KSpace);
        }
        
        

    
    }
}