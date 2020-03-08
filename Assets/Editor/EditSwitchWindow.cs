using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public class EditSwitchWindow : QuestSystemEditWindow
    {
        enum EConfirmState
        {
            None,
        }
        private int KSpace = 10;
        
        private SwitchDescriptionData _descriptionData = new SwitchDescriptionData();
        private List<List<SwitchComponentData>> _stateList = new List<List<SwitchComponentData>>();
        private List<SwitchStateResultData> _stateResultDataList = new List<SwitchStateResultData>();
        
        private EConfirmState _confirmState = EConfirmState.None;
        private readonly string[] operTexts = {"AND", "OR"};

        private int _stateIndex = 0;

        protected override void EnableProcess()
        {
            if (_stateList.Count == 0)
            {
                AddState();
            }
        }
        
        
        
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/EditSwitchWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditSwitchWindow window = (EditSwitchWindow)EditorWindow.GetWindow(typeof(EditSwitchWindow));
            window.Show();
        }

        protected override void GUIProcess()
        {
            //로고
            GUILayout.Space(KSpace);
            
            GUILayout.Label("Create SwitchData", "DefaultCenteredLargeText");
            
            GUILayout.Space(KSpace);
            
            EditorGUI.BeginDisabledGroup(_confirmState != EConfirmState.None);
            {
                Rect areaRect = position;
                areaRect.height = position.height * 0.8f;
                
                //스위치 아이디
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(IsUpdate);
                    {
                        EditorGUILayout.PrefixLabel("SwitchId ");
                        _descriptionData.SwitchId = GUILayout.TextField(_descriptionData.SwitchId);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
                
                //스위치 설명
                GUILayout.Space(KSpace);
                GUILayout.Label("Description ");
                _descriptionData.Description = GUILayout.TextArea(_descriptionData.Description, GUILayout.Height(100));
                GUILayout.Space(KSpace);
                
                //팝업내용 만들기
                string[] popupTexts = new string[_stateList.Count + 1];
                int stateIndex = 0;
                for (stateIndex = 0; stateIndex < _stateList.Count; ++stateIndex)
                {
                    popupTexts[stateIndex] = $"상태 {stateIndex}";
                }
                popupTexts[stateIndex] = $"상태 {stateIndex} 추가";
            
                //팝업드랍박스
                GUILayout.BeginHorizontal();
                _stateIndex = EditorGUILayout.Popup(_stateIndex, popupTexts);
                if (_stateList.Count == _stateIndex)
                {
                    AddState();
                }
                EditorGUI.BeginDisabledGroup(_stateIndex == 0);
                {
                    if (GUILayout.Button("상태 삭제", GUILayout.Width(120)))
                    {
                        _stateList.Remove(_stateList[_stateIndex]);
                        if (_stateList.Count <= _stateIndex)
                        {
                            _stateIndex = _stateList.Count - 1;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
                
                
                
                //결과창
//                int resultIndex = _stateResultDataList[_stateIndex].Result ? 1 : 0;
//                resultIndex = EditorGUILayout.Popup(resultIndex, resultTexts);
//                _stateResultDataList[_stateIndex].Result = resultIndex == 1 ? true : false;

                GUILayout.BeginHorizontal(GUILayout.Width(150));
                GUILayout.Space(position.width * 0.5f - 75);
                GUILayout.BeginVertical();
                GUILayout.Space(18);
                GUILayout.Label("결과 : ");
                GUILayout.EndVertical();
//                Debug.LogWarning("_stateIndex : " + _stateIndex);
//                Debug.LogError("_stateResultDataList.Count : " + _stateResultDataList.Count);
//                Debug.LogWarning("_stateResultDataList[_stateIndex] : " + _stateResultDataList[_stateIndex]);
                var resultText = _stateResultDataList[_stateIndex].Result.ToString();
                if (GUILayout.Button(resultText, "GroupBox", GUILayout.Width(100)))
                {
                    _stateResultDataList[_stateIndex].Result = !_stateResultDataList[_stateIndex].Result;
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(20);
                
                GUILayout.BeginVertical(GUILayout.Height(position.height - 320));
                
                //디폴트 퀘스트아이디 인풋
                _stateList[_stateIndex][0].QuestId = EditorGUILayout.TextField("QuestId ", _stateList[_stateIndex][0].QuestId);
                //추가 인풋
                for (int stateComponentIndex = 1; stateComponentIndex < _stateList[_stateIndex].Count; ++stateComponentIndex)
                {
                    GUILayout.BeginHorizontal();
                    int operIndex = _stateList[_stateIndex][stateComponentIndex].Operator == operTexts[0] ? 0 : 1;
                    operIndex = EditorGUILayout.Popup(operIndex, operTexts, GUILayout.Width(148));
                    _stateList[_stateIndex][stateComponentIndex].Operator = operTexts[operIndex];
                    _stateList[_stateIndex][stateComponentIndex].QuestId = EditorGUILayout.TextField(_stateList[_stateIndex][stateComponentIndex].QuestId);
                    GUILayout.Space(10);
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(20)))
                    {
                        _stateList[_stateIndex].Remove(_stateList[_stateIndex][stateComponentIndex]);
                    }
                    GUILayout.Space(15);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                //퀘스트아이디 추가 버튼
                if (GUILayout.Button("연산 추가"))
                {
                    var switchComponent = new SwitchComponentData();
                    switchComponent.Operator = operTexts[0];
                    _stateList[_stateIndex].Add(switchComponent);
                }
                
                GUILayout.EndVertical();
                //버튼 기능
                if (IsUpdate)
                {
                
                    if (GUILayout.Button("Update"))
                    {
                        Debug.Log("업데이트 성공");
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        //SwitchData 구성요소인지 체크
                    }
                }
                else
                {
                    if (GUILayout.Button("생성"))
                    {
                        string message = string.Empty;
                        if (SaveProcess(out message))
                        {
                            Debug.Log("성공 : " + message);
                        }
                        else
                        {
                            Debug.LogError("실패 : " + message);
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }


        //현제 스테이트에서 하나 추가한다.
        public void AddState()
        {
            var state = new List<SwitchComponentData>();
            var stateComponent = new SwitchComponentData();
            stateComponent.Operator = "";
            state.Add(stateComponent);
            _stateList.Add(state);

            var stateResultData = new SwitchStateResultData();
            _stateResultDataList.Add(stateResultData);
        }

        public bool SaveProcess(out string message)
        {
            var switchId = _descriptionData.SwitchId;
            if (string.IsNullOrEmpty(switchId))
            {
                message = "SwitchId가 비었다.";
                return false;
            }

            if (null != SQLiteManager.Instance.GetSwitchDescriptionData(switchId))
            {
                message = "중복된 SwitchId가 있습니다.";
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
                        message = $"QuestId가 비어있습니다 : 상태 {stateIdx} - {componentIdx}번 ";
                        return false;
                    }
                    
                    if (null == SQLiteManager.Instance.GetQuestData(stateComponent.QuestId))
                    {
                        message = $"등록되지 않은 QuestId입니다. : 상태 {stateIdx} - {componentIdx}번 - {stateComponent.QuestId}";
                        return false;
                    }

                    stateComponent.SwitchId = switchId;
                    stateComponent.State = stateIdx;
                    stateComponent.Order = componentIdx;
                }
            }
            
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

            message = "완료되었습니다.";
            return true;
        }


    }
}