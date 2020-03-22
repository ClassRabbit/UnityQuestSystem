using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditQuestWindow : QuestSystemEditWindow
    {
        #region Const

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

        
        private const string CreateFailEmptyIdText = "QuestId가 입력되지 않았습니다.";
        private const string CreateFailOverlapIdText = "QuestId가 중복되었습니다.";
        private const string DeleteFailUseSwitchText = "SwitchData에서 사용 중인 QuestData는 삭제할 수 없습니다.";

        private const string WindowTitleText = "QuestData 생성";
        private const string QuestIdText = "QuestId";
        
        #endregion
        
        
        #region Variable
        
        private bool IsUpdate { get; set; } = false;
        private QuestData _questData = new QuestData();
        private EConfirmState _confirmState = EConfirmState.None;
        
        #endregion

        
        
        [MenuItem("QuestSystem/EditQuestWindow")]
        static void Init()
        {
            EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
            window.Show();
        }

        /// <summary>
        ///   <para>SearchWindow에서 수정 요청 함수</para>
        /// </summary>
        internal void UpdateQuestData(QuestData questData)
        {
            if (null == questData)
            {
                return;
            }
            
            IsUpdate = true;
            _questData = new QuestData(questData.QuestId, questData.Description);
        }


        
        /// <summary>
        ///   <para>확인창 구성하는 행동</para>
        /// </summary>
        protected override void ConfirmWindowProcess()
        {
            switch (_confirmState)
            {
                case EConfirmState.CreateSuccess:
                    ConfirmWindowNoticeText = CreateSuccessTextValue;
                    break;
                case EConfirmState.CreateFailEmptyId:
                    ConfirmWindowNoticeText = CreateFailEmptyIdText;
                    break;
                case EConfirmState.CreateFailOverlapId:
                    ConfirmWindowNoticeText = CreateFailOverlapIdText;
                    break;
                case EConfirmState.UpdateSuccess:
                    ConfirmWindowNoticeText = UpdateSuccessTextValue;
                    break;
                case EConfirmState.DeleteSuccess:
                    ConfirmWindowNoticeText = DeleteSuccessTextValue;
                    break;
                case EConfirmState.DeleteFailUseSwitch:
                    ConfirmWindowNoticeText = DeleteFailUseSwitchText;
                    break;
            }

            ConfirmWindowAction = () =>
            {
                switch (_confirmState)
                {
                    case EConfirmState.CreateSuccess:
                    case EConfirmState.DeleteSuccess:
                        ResetEditor();
                        break;
                }
                _confirmState = EConfirmState.None;
            };
            
            
        }
        
        protected override void ResetEditor()
        {
            IsUpdate = false;
            _questData.QuestId = string.Empty;
            _questData.Description = string.Empty;
        }


        protected override void EnableProcess()
        {
        }

        protected override void RefreshProcess()
        {
        }

        protected override void GUIProcess()
        {
            GUILayout.Space(10);
            
            GUILayout.Label(WindowTitleText, "DefaultCenteredLargeText");
            
            GUILayout.Space(10);
            
            EditorGUI.BeginDisabledGroup(_confirmState != EConfirmState.None);
            {
                //퀘스트 아이디
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(IsUpdate);
                    {
                        EditorGUILayout.PrefixLabel(QuestIdText);
                        _questData.QuestId = GUILayout.TextField(_questData.QuestId);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
                
                //퀘스트 설명
                GUILayout.Space(10);
                GUILayout.Label(DescriptionTextValue);
                _questData.Description = GUILayout.TextArea(_questData.Description, GUILayout.Height(position.height - 125 - 10));
                GUILayout.Space(10);
            
                GUILayout.BeginHorizontal();
                GUILayout.Space(position.width * 0.2f);

                if (IsUpdate)
                {
                
                    if (GUILayout.Button(UpdateTextValue))
                    {
                        SQLiteManager.Instance.UpdateQuestData(_questData);
                        _confirmState = EConfirmState.UpdateSuccess;
                    }
                    if (GUILayout.Button(DeleteTextValue))
                    {
                        //SwitchData 구성요소인지 체크
                        if (SQLiteManager.Instance.GetSearchSwitchDescriptionDataList(_questData.QuestId).Count() != 0)
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
                    if (GUILayout.Button(CreateTextValue))
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
                
                GUILayout.Space(position.width * 0.2f);
                GUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            if (_confirmState != EConfirmState.None)
            {
                DrawConfirmWindow(string.Empty);
            }
            
            GUILayout.Space(10);
        }

        protected override void FocusProcess()
        {
        }
    }
}