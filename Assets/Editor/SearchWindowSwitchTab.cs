using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestSystem
{

    internal class SearchWindowSwitchTab : SearchWindowTab<SwitchDescriptionData>
    {
        private const int KSwitchPerPage = 10;
        
        private List<List<SwitchComponentData>> _currentPageComponentDataList;
        private List<List<SwitchStateResultData>> _currentPageStateResultDataList;
        
        protected override int MaxPageIndex => (TargetDataList.Count - 1) / KSwitchPerPage;


        internal void EnableProcess()
        {
        }
        
        protected override bool CompareData(SwitchDescriptionData ta, SwitchDescriptionData tb)
        {
            return ta.SwitchId == tb.SwitchId;
        }

        protected override void ActionSearch()
        {
            if (IsSearch)
            {
                SearchResultDataList = SQLiteManager.Instance.GetSearchSwitchDescriptionDatas(SearchText).ToList();
            }
            SetPageData();
        }


        void SetPageData()
        {
            var targetDataList = TargetDataList;
            _currentPageComponentDataList = new List<List<SwitchComponentData>>();
            _currentPageStateResultDataList = new List<List<SwitchStateResultData>>();
            
            if (targetDataList != null)
            {
                for (int i = KSwitchPerPage * CurrentPageIndex;
                    i < targetDataList.Count && i < KSwitchPerPage * (CurrentPageIndex + 1);
                    ++i)
                {
                    var descriptionData = targetDataList[i];
                    
                    var enumerableComponentData = SQLiteManager.Instance.GetSwitchComponentDatas(descriptionData.SwitchId);
                    _currentPageComponentDataList.Add(enumerableComponentData.ToList());

                    var enumerableStateResultData = SQLiteManager.Instance.GetSwitchStateResultData(descriptionData.SwitchId);
                    _currentPageStateResultDataList.Add(enumerableStateResultData.ToList());
                }
            }
        }

        
//        StringBuilder stringBuilder =
//            new StringBuilder(_currentPageComponentDataList[stateIdx][0].QuestId);
//        stringBuilder.Append(' ');
//        for (int componentIdx = 1;
//        componentIdx < _currentPageComponentDataList[stateIdx].Count;
//        ++componentIdx)
//        {
//            var componentData = _currentPageComponentDataList[stateIdx][componentIdx];
//            stringBuilder.Append(componentData.Operator);
//            stringBuilder.Append(' ');
//            stringBuilder.Append(componentData.QuestId);
//            stringBuilder.Append(' ');
//        }

        protected override void DrawTableProcess()
        {
            var targetDataList = TargetDataList;
            
            
            if (targetDataList != null)
            {
                SwitchDescriptionData selectedDescriptionData = SelectedDataIndex.HasValue ? targetDataList[SelectedDataIndex.Value] : null;
                
                int firstPageSwitchIdx = KSwitchPerPage * CurrentPageIndex;
                for (int switchIdx = firstPageSwitchIdx;
                    switchIdx < targetDataList.Count && switchIdx < KSwitchPerPage * (CurrentPageIndex + 1);
                    ++switchIdx)
                {
                    var descriptionData = targetDataList[switchIdx];
                    int pageSwitchIdx = switchIdx - firstPageSwitchIdx;
                    int componentListIdx = 0;
                    int resultListIdx = 0;
                    
                    if (null != selectedDescriptionData && descriptionData.SwitchId == selectedDescriptionData.SwitchId)
                    {
                        GUILayout.BeginHorizontal("LODSliderRangeSelected");
                    }
                    else
                    {
                        GUILayout.BeginHorizontal("box");
                    }
                    
                    var componentList = _currentPageComponentDataList[pageSwitchIdx];
                    var resultList = _currentPageStateResultDataList[pageSwitchIdx];

                    GUILayout.BeginVertical();
                    if (GUILayout.Button(descriptionData.SwitchId, "FrameBox",
                        GUILayout.Width(ColumnHeader.GetColumn(0).width - 10), GUILayout.Height(25 * resultList.Count + 4 * (resultList.Count - 1))))
                    {
                        SelectedDataIndex = switchIdx;
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.BeginVertical();
                    while (componentList.Count > componentListIdx)
                    {
                        int currentState = componentList[componentListIdx].State;
                        StringBuilder stringBuilder = new StringBuilder($"[{componentList[componentListIdx].QuestId}]");
                        stringBuilder.Append(' ');
                        ++componentListIdx;
                        while (componentListIdx < componentList.Count && currentState == componentList[componentListIdx].State)
                        {
                            stringBuilder.Append(componentList[componentListIdx].Operator);
                            stringBuilder.Append(' ');
                            stringBuilder.Append($"[{componentList[componentListIdx].QuestId}]");
                            stringBuilder.Append(' ');
                            ++componentListIdx;
                        }
                        
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(currentState.ToString(), "FrameBox",
                            GUILayout.Width(ColumnHeader.GetColumn(1).width - 8)))
                        {
                            SelectedDataIndex = switchIdx;
                        }
                        
                        if (GUILayout.Button(stringBuilder.ToString(), "FrameBox",
                            GUILayout.Width(ColumnHeader.GetColumn(2).width - 6)))
                        {
                            SelectedDataIndex = switchIdx;
                        }
                        
                        if (GUILayout.Button(resultList[resultListIdx].Result.ToString(), "FrameBox",
                            GUILayout.Width(ColumnHeader.GetColumn(3).width)))
                        {
                            SelectedDataIndex = switchIdx;
                        }

                        ++resultListIdx;
                        
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }
            }
        }
        
        protected override void RefreshPageListProcess()
        {
            SetPageData();
        }


        protected override void DrawDetailProcess()
        {
            if (SelectedDataIndex.HasValue)
            {
//                var questData = TargetQuestDataList[SelectedDataIndex.Value];
//                GUILayout.Space(3);
//                GUILayout.Label(questData.QuestId);
//                GUILayout.Space(3);
//                GUILayout.Label(questData.Description);
//                if (GUILayout.Button("Edit"))
//                {
//                    EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
//                    window.Show();
//                    window.UpdateQuestData(questData);
//                }
            }
        }


        protected override void ResizeColumn()
        {
            Columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("SwitchId"),
                    width = TabPosition.width * 0.2f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("상태"),
                    width = TabPosition.width * 0.15f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("계산식"),
                    width = TabPosition.width * 0.5f,
                    minWidth = TabPosition.width * 0.3f,
                    maxWidth = TabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("결과"),
                    width = TabPosition.width * 0.15f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                }
                
            };
        }
        
        
    }
}
