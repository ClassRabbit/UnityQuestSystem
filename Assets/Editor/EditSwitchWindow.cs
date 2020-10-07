using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditSwitchWindow : QuestSystemEditWindow
    {
        #region Const

        enum EConfirmState
        {
            None,
            Fail,
            CreateSuccess,
            UpdateSuccess,
            DeleteSuccess,
        }
        
        private const string CheckFailEmptySwitchIdText = "SwitchId가 입력되지 않았습니다.";
        private const string CheckFailOverlapIdText = "SwitchId가 중복되었습니다.";
        private const string CheckFailEmptyQuestIdFormatText = "QuestId가 입력되지 않았습니다 : 상태 {0} - {1}번";
        private const string CheckFailNotExistQuestIdFormatText = "생성되지 않은 QuestId 입니다. : 상태 {0} - {1}번 - {2}";

        
        private const string WindowTitleText = "SwitchData 생성";
        private const string SwitchIdText = "SwitchId";
        private const string QuestIdText = "QuestId";
        private const string StateFormatText = "상태 {0}";
        private const string StateAddFormatText = "상태 {0} 추가";
        private const string DeleteStateText = "상태 삭제";
        private const string AddOperText = "연산 추가";
        private const string StartResultText = "기본 결과 : ";
        private const string StateResultText = "상태 결과 : ";

        #endregion


        #region Variable

        private bool IsUpdate { get; set; } = false;
        
        private EConfirmState _confirmState = EConfirmState.None;
        
        private readonly string[] _operTexts = { "AND", "OR" };

        private SwitchDescriptionData _descriptionData = new SwitchDescriptionData();
        private List<List<SwitchComponentData>> _stateList = new List<List<SwitchComponentData>>();
        private List<SwitchStateResultData> _stateResultDataList = new List<SwitchStateResultData>();
        private int _stateIndex = 0;

        #endregion
        
        
        [MenuItem("QuestSystem/EditSwitchWindow")]
        static void Init()
        {
            EditSwitchWindow window = (EditSwitchWindow)EditorWindow.GetWindow(typeof(EditSwitchWindow));
            window.Show();
        }
        
        /// <summary>
        ///   <para>SearchWindow에서 수정 요청 함수</para>
        /// </summary>
        internal void UpdateSwitchData(SwitchDescriptionData descriptionData, List<SwitchComponentData> componentDataList, List<SwitchStateResultData> stateResultDataList)
        {
            if (null == descriptionData || null == componentDataList || null == stateResultDataList)
            {
                return;
            }
            
            IsUpdate = true;
            
            _descriptionData = descriptionData;

            int stateIdx = componentDataList[0].State;
            _stateList = new List<List<SwitchComponentData>>();
            _stateList.Add(new List<SwitchComponentData>());
            for (int i = 0; i < componentDataList.Count; ++i)
            {
                if (stateIdx == componentDataList[i].State)
                {
                    _stateList[stateIdx].Add(componentDataList[i]);
                }
                else
                {
                    ++stateIdx;
                    _stateList.Add(new List<SwitchComponentData>());
                    _stateList[stateIdx].Add(componentDataList[i]);
                }
            }
            
            _stateResultDataList = new List<SwitchStateResultData>();
            foreach (var stateResultData in stateResultDataList)
            {
                _stateResultDataList.Add(stateResultData);
            }
        }
        
        /// <summary>
        ///   <para>상태를 추가한다.</para>
        /// </summary>
        private void AddState()
        {
            var state = new List<SwitchComponentData>();
            var stateComponent = new SwitchComponentData();
            stateComponent.Operator = string.Empty;
            state.Add(stateComponent);
            _stateList.Add(state);

            var stateResultData = new SwitchStateResultData();
            _stateResultDataList.Add(stateResultData);
        }
        
        /// <summary>
        ///   <para>OnEnable 되었을 시 행동</para>
        /// </summary>
        protected override void EnableProcess()
        {
            if (_stateList.Count == 0)
            {
                AddState();
            }
        }

        protected override void RefreshProcess()
        {
        }

        /// <summary>
        ///   <para>확인창 구성하는 행동</para>
        /// </summary>
        protected override void ConfirmWindowProcess()
        {
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
        
        /// <summary>
        ///   <para>창을 초기화하는 행동</para>
        /// </summary>
        protected override void ResetEditor()
        {
            IsUpdate = false;
            _stateIndex = 0;
            _descriptionData = new SwitchDescriptionData();
            _stateList = new List<List<SwitchComponentData>>();
            _stateResultDataList = new List<SwitchStateResultData>();
            AddState();
        }

        /// <summary>
        ///   <para>창을 그리는 행동</para>
        /// </summary>
        protected override void GUIProcess()
        {
            
            //로고
            GUILayout.Space(10);
            
            GUILayout.Label(WindowTitleText, "DefaultCenteredLargeText");
            
            GUILayout.Space(10);
            
            EditorGUI.BeginDisabledGroup(_confirmState != EConfirmState.None);
            {
                Rect areaRect = position;
                areaRect.height = position.height * 0.8f;
                
                //스위치 아이디
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(IsUpdate);
                    {
                        EditorGUILayout.PrefixLabel(SwitchIdText);
                        _descriptionData.SwitchId = GUILayout.TextField(_descriptionData.SwitchId);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
                
                //시작결과
                GUILayout.BeginHorizontal(GUILayout.Width(150));
                GUILayout.Space(position.width * 0.5f - 75);
                GUILayout.BeginVertical();
                GUILayout.Space(18);
                GUILayout.Label(StartResultText);
                GUILayout.EndVertical();
                var defaultResultText = _descriptionData.DefaultResult.ToString();
                if (GUILayout.Button(defaultResultText, "GroupBox", GUILayout.Width(100)))
                {
                    _descriptionData.DefaultResult = !_descriptionData.DefaultResult;
                }
                GUILayout.EndHorizontal();
                
                //스위치 설명
                GUILayout.Space(10);
                GUILayout.Label(DescriptionTextValue);
                _descriptionData.Description = GUILayout.TextArea(_descriptionData.Description, GUILayout.Height(100));
                
                
                //
                //
                //
                //
                
                GUILayout.Space(15);
                
                //팝업내용 만들기
                string[] popupTexts = new string[_stateList.Count + 1];
                int stateIndex = 0;
                for (stateIndex = 0; stateIndex < _stateList.Count; ++stateIndex)
                {
                    popupTexts[stateIndex] = string.Format(StateFormatText, stateIndex);
                }
                popupTexts[stateIndex] = string.Format(StateAddFormatText, stateIndex);
            
                //팝업드랍박스
                GUILayout.BeginHorizontal();
                _stateIndex = EditorGUILayout.Popup(_stateIndex, popupTexts);
                if (_stateList.Count == _stateIndex)
                {
                    AddState();
                }
                
                //현재 상태 삭제하기
                EditorGUI.BeginDisabledGroup(_stateIndex == 0);
                {
                    if (GUILayout.Button(DeleteStateText, GUILayout.Width(120)))
                    {
                        _stateList.Remove(_stateList[_stateIndex]);
                        if (_stateList.Count <= _stateIndex)
                        {
                            _stateIndex = _stateList.Count - 1;
                        }
                        _stateResultDataList.Remove(_stateResultDataList[_stateIndex]);
                    }
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(150));
                GUILayout.Space(position.width * 0.5f - 75);
                GUILayout.BeginVertical();
                GUILayout.Space(18);
                GUILayout.Label(StateResultText);
                GUILayout.EndVertical();
                var resultText = _stateResultDataList[_stateIndex].Result.ToString();
                if (GUILayout.Button(resultText, "GroupBox", GUILayout.Width(100)))
                {
                    _stateResultDataList[_stateIndex].Result = !_stateResultDataList[_stateIndex].Result;
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(20);
                
                GUILayout.BeginVertical(GUILayout.Height(position.height - 362));
                
                //디폴트 퀘스트아이디 인풋
                _stateList[_stateIndex][0].QuestId = EditorGUILayout.TextField(QuestIdText, _stateList[_stateIndex][0].QuestId);
                //추가 인풋
                for (int stateComponentIndex = 1; stateComponentIndex < _stateList[_stateIndex].Count; ++stateComponentIndex)
                {
                    GUILayout.BeginHorizontal();
                    int operIndex = _stateList[_stateIndex][stateComponentIndex].Operator == _operTexts[0] ? 0 : 1;
                    operIndex = EditorGUILayout.Popup(operIndex, _operTexts, GUILayout.Width(148));
                    _stateList[_stateIndex][stateComponentIndex].Operator = _operTexts[operIndex];
                    _stateList[_stateIndex][stateComponentIndex].QuestId = EditorGUILayout.TextField(_stateList[_stateIndex][stateComponentIndex].QuestId);
                    GUILayout.Space(10);
                    if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                    {
                        _stateList[_stateIndex].Remove(_stateList[_stateIndex][stateComponentIndex]);
                    }
                    GUILayout.Space(15);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                //퀘스트아이디 추가 버튼
                if (GUILayout.Button(AddOperText))
                {
                    var switchComponent = new SwitchComponentData();
                    switchComponent.Operator = _operTexts[0];
                    _stateList[_stateIndex].Add(switchComponent);
                }
                
                GUILayout.EndVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Space(position.width * 0.2f);
                //버튼 기능
                if (IsUpdate)
                {
                    
                    if (GUILayout.Button(UpdateTextValue))
                    {
                        if (CheckProcess())
                        {
                            DeleteProcess();
                            SaveProcess();
                            _confirmState = EConfirmState.UpdateSuccess;
                        }
                        else
                        {
                            _confirmState = EConfirmState.Fail;
                        }
                    }
                    if (GUILayout.Button(DeleteTextValue))
                    {
                        //SwitchData 구성요소인지 체크
                        DeleteProcess();
                        ConfirmWindowNoticeText = DeleteSuccessTextValue;
                        _confirmState = EConfirmState.DeleteSuccess;
                    }
                    
                }
                else
                {
                    if (GUILayout.Button(CreateTextValue))
                    {
                        if (CheckProcess())
                        {
                            SaveProcess();
                            _confirmState = EConfirmState.CreateSuccess;
                        }
                        else
                        {
                            _confirmState = EConfirmState.Fail;
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

        /// <summary>
        ///   <para>생성을 위한 구성요소 확인.</para>
        /// </summary>
        private bool CheckProcess()
        {
            var switchId = _descriptionData.SwitchId;
            if (string.IsNullOrEmpty(switchId))
            {
                ConfirmWindowNoticeText = CheckFailEmptySwitchIdText;
                return false;
            }

            if (!IsUpdate && null != SQLiteManager.Instance.GetSwitchDescriptionData(switchId))
            {
                ConfirmWindowNoticeText = CheckFailOverlapIdText;
                return false;
            }

            for (int stateIdx = 0; stateIdx < _stateList.Count; ++stateIdx)
            {
                var state = _stateList[stateIdx];
                for (int componentIdx = 0; componentIdx < state.Count; ++componentIdx)
                {
                    var stateComponent = state[componentIdx];
                    if (string.IsNullOrEmpty(stateComponent.QuestId))
                    {
                        ConfirmWindowNoticeText = string.Format(CheckFailEmptyQuestIdFormatText, stateIdx, componentIdx);
                        return false;
                    }
                    
                    if (null == SQLiteManager.Instance.GetQuestData(stateComponent.QuestId))
                    {
                        ConfirmWindowNoticeText = string.Format(CheckFailNotExistQuestIdFormatText, stateIdx, componentIdx, stateComponent.QuestId);
                        return false;
                    }

                    stateComponent.SwitchId = switchId;
                    stateComponent.State = stateIdx;
                    stateComponent.Order = componentIdx;
                }
            }

            return true;
            
        }
        
        /// <summary>
        ///   <para>SwitchData 생성</para>
        /// </summary>
        private void SaveProcess()
        {
            var switchId = _descriptionData.SwitchId;
            
            SQLiteManager.Instance.CreateSwitchDescriptionData(_descriptionData);
            
            foreach (var state in _stateList)
            {
                foreach (var stateComponent in state)
                {
                    SQLiteManager.Instance.CreateSwitchComponentData(stateComponent);
                }
            }

            for (int stateIdx = 0; stateIdx < _stateResultDataList.Count; ++stateIdx)
            {
                var stateResult = _stateResultDataList[stateIdx];
                stateResult.SwitchId = switchId;
                stateResult.State = stateIdx;
                
                SQLiteManager.Instance.CreateSwitchStateResultData(stateResult);
            }
            ConfirmWindowNoticeText = IsUpdate ? UpdateSuccessTextValue : CreateSuccessTextValue;
        }
        
        /// <summary>
        ///   <para>SwitchData 삭제</para>
        /// </summary>
        private void DeleteProcess()
        {
            var switchId = _descriptionData.SwitchId;
            
            SQLiteManager.Instance.DeleteSwitchDescriptionDataBySwitchId(switchId);
            
            SQLiteManager.Instance.DeleteSwitchComponentDataBySwitchId(switchId);

            SQLiteManager.Instance.DeleteSwitchStateResultDataBySwitchId(switchId);
        }

    }
}