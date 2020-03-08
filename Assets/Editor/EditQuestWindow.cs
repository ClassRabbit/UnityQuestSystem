using System;
using System.Linq;
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
            string confirmText = String.Empty;
            switch (_confirmState)
            {
                case EConfirmState.CreateSuccess:
                    confirmText = "생성되었습니다.";
                    break;
                case EConfirmState.CreateFailEmptyId:
                    confirmText = "QuestId가 입력되지 않았습니다.";
                    break;
                case EConfirmState.CreateFailOverlapId:
                    confirmText = "QuestId가 중복되었습니다.";
                    break;
                case EConfirmState.UpdateSuccess:
                    confirmText = "수정되었습니다.";
                    break;
                case EConfirmState.DeleteSuccess:
                    confirmText = "삭제되었습니다.";
                    break;
                case EConfirmState.DeleteFailUseSwitch:
                    confirmText = "SwitchData에서 사용 중인 QuestData는 삭제할 수 없습니다.";
                    break;
            }
            GUILayout.Label(confirmText);
            if (GUILayout.Button("Confirm"))
            {
                switch (_confirmState)
                {
                    case EConfirmState.CreateSuccess:
                        _questData.QuestId = string.Empty;
                        _questData.Description = string.Empty;
                        break;
                    case EConfirmState.DeleteSuccess:
                        IsClose = true;
                        return;
                }
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
            if (null == questData)
            {
                return;
            }
            
            IsUpdate = true;
            _questData = questData;
        }

        protected override void GUIProcess()
        {
            if (IsClose)
            {
                this.Close();
                return;
            }
            
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
                
                    if (GUILayout.Button("수정"))
                    {
                        SQLiteManager.Instance.UpdateQuestData(_questData);
                        _confirmState = EConfirmState.UpdateSuccess;
                    }
                    if (GUILayout.Button("삭제"))
                    {
                        //SwitchData 구성요소인지 체크
                        if (SQLiteManager.Instance.GetSearchSwitchDescriptionDatas(_questData.QuestId).Count() != 0)
                        {
                            _confirmState = EConfirmState.DeleteFailUseSwitch;
                        }
                        else
                        {
                            SQLiteManager.Instance.DeleteQuestData(_questData);
                            _confirmState = EConfirmState.DeleteSuccess;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("생성"))
                    {
                        //아이디 채크
                        if (string.IsNullOrEmpty(_questData.QuestId))
                        {
                            _confirmState = EConfirmState.CreateFailEmptyId;
                        }
                        else if (null != SQLiteManager.Instance.GetQuestData(_questData.QuestId))
                        {
                            _confirmState = EConfirmState.CreateFailOverlapId;
                        }
                        else
                        {
                            SQLiteManager.Instance.CreateQuestData(_questData);
                            _confirmState = EConfirmState.CreateSuccess;
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