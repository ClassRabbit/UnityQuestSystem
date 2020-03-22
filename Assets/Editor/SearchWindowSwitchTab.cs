using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Text;

namespace QuestSystem
{

    internal class SearchWindowSwitchTab : SearchWindowTab<SwitchDescriptionData>
    {
        #region Const

        private const int SwitchPerPage = 10;
        
        private const string SwitchIdTextValue = "SwitchId";
        private const string DefaultResultTextValue = "기본 결과";
        private const string StateTextValue = "상태";
        private const string FormulaTextValue = "계산식";
        private const string StateResultTextValue = "상태 결과";

        #endregion



        #region Variable

        private List<List<SwitchComponentData>> _currentPageComponentDataList;
        private List<List<SwitchStateResultData>> _currentPageStateResultDataList;
        
        protected override int MaxPageIndex => (TargetDataList.Count - 1) / SwitchPerPage;

        #endregion
        
        
        /// <summary>
        ///   <para></para>
        /// </summary>
        void SetTablePageData()
        {
            var targetDataList = TargetDataList;
            _currentPageComponentDataList = new List<List<SwitchComponentData>>();
            _currentPageStateResultDataList = new List<List<SwitchStateResultData>>();
            
            if (targetDataList != null)
            {
                for (int i = SwitchPerPage * CurrentPageIndex;
                    i < targetDataList.Count && i < SwitchPerPage * (CurrentPageIndex + 1);
                    ++i)
                {
                    var descriptionData = targetDataList[i];
                    
                    var componentDataList = SQLiteManager.Instance.GetSwitchComponentDataList(descriptionData.SwitchId);
                    _currentPageComponentDataList.Add(componentDataList);

                    var stateResultDataList = SQLiteManager.Instance.GetSwitchStateResultDataList(descriptionData.SwitchId);
                    _currentPageStateResultDataList.Add(stateResultDataList);
                }
            }
        }
        
        /// <summary>
        ///   <para>두 데이터가 서로 같음을 확인하는 방식 결정</para>
        /// </summary>
        protected override bool IsSameData(SwitchDescriptionData ta, SwitchDescriptionData tb)
        {
            return ta.SwitchId == tb.SwitchId;
        }
        
        /// <summary>
        ///   <para>현재 페이지 새롭게 그리기</para>
        /// </summary>
        protected override void RefreshPageListProcess()
        {
            SetTablePageData();
        }

        /// <summary>
        ///   <para>조회 실행</para>
        /// </summary>
        protected override void ActionSearch()
        {
            if (!string.IsNullOrEmpty(UsingSearchText))
            {
                SearchResultDataList = SQLiteManager.Instance.GetSearchSwitchDescriptionDataList(UsingSearchText);
            }
            SetTablePageData();
        }

        /// <summary>
        ///   <para>테이블 그리기</para>
        /// </summary>
        protected override void DrawTableProcess()
        {
            var targetDataList = TargetDataList;
            
            
            if (targetDataList != null)
            {
                SwitchDescriptionData selectedDescriptionData = SelectedDataIndex.HasValue ? targetDataList[SelectedDataIndex.Value] : null;
                
                int firstPageSwitchIdx = SwitchPerPage * CurrentPageIndex;
                for (int switchIdx = firstPageSwitchIdx;
                    switchIdx < targetDataList.Count && switchIdx < SwitchPerPage * (CurrentPageIndex + 1);
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
                    if (GUILayout.Button(descriptionData.DefaultResult.ToString(), "FrameBox",
                        GUILayout.Width(ColumnHeader.GetColumn(1).width - 10), GUILayout.Height(25 * resultList.Count + 4 * (resultList.Count - 1))))
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
                            GUILayout.Width(ColumnHeader.GetColumn(2).width)))
                        {
                            SelectedDataIndex = switchIdx;
                        }
                        
                        if (GUILayout.Button(stringBuilder.ToString(), "FrameBox",
                            GUILayout.Width(ColumnHeader.GetColumn(3).width)))
                        {
                            SelectedDataIndex = switchIdx;
                        }
                        
                        if (GUILayout.Button(resultList[resultListIdx].Result.ToString(), "FrameBox",
                            GUILayout.Width(ColumnHeader.GetColumn(4).width)))
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
        
        /// <summary>
        ///   <para>세부 정보 그리기</para>
        /// </summary>
        protected override void DrawDetailProcess()
        {
            if (SelectedDataIndex.HasValue)
            {
                var descriptionData = TargetDataList[SelectedDataIndex.Value];
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Space(8);
                int beforeFontSize = EditorStyles.label.fontSize;
                EditorStyles.label.fontSize = 20;
                GUILayout.Label(descriptionData.SwitchId, EditorStyles.label);
                EditorStyles.label.fontSize = beforeFontSize;
                GUILayout.EndVertical();
                
                if (GUILayout.Button(TexturUpdateIcon, GUILayout.Width(40)))
                {
                    EditSwitchWindow window = (EditSwitchWindow)EditorWindow.GetWindow(typeof(EditSwitchWindow));
                    window.Show();
                    
                    int pageSwitchIdx = SelectedDataIndex.Value - (SwitchPerPage * CurrentPageIndex);
                    var componentList = _currentPageComponentDataList[pageSwitchIdx];
                    var resultList = _currentPageStateResultDataList[pageSwitchIdx];
                    window.UpdateSwitchData(descriptionData, componentList, resultList);
                    
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3);

                bool beforeWordWrap = EditorStyles.label.wordWrap;
                var beforeWordAlignment = EditorStyles.label.alignment;
                EditorStyles.label.wordWrap = true;
                EditorStyles.label.alignment = TextAnchor.UpperLeft;
                GUILayout.Label(descriptionData.Description, EditorStyles.label, GUILayout.Width(TabPosition.width - 10), GUILayout.ExpandHeight(true));
                EditorStyles.label.alignment = beforeWordAlignment;
                EditorStyles.label.wordWrap = beforeWordWrap;
            }
        }

        /// <summary>
        ///   <para>테이블 컬럼 resize될시 실행</para>
        /// </summary>
        protected override void ResizeColumn()
        {
            Columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(SwitchIdTextValue),
                    width = TabPosition.width * 0.2f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(DefaultResultTextValue),
                    width = TabPosition.width * 0.1f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(StateTextValue),
                    width = TabPosition.width * 0.15f,
                    minWidth = TabPosition.width * 0.1f,
                    maxWidth = TabPosition.width * 0.4f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(FormulaTextValue),
                    width = TabPosition.width * 0.4f,
                    minWidth = TabPosition.width * 0.3f,
                    maxWidth = TabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(StateResultTextValue),
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
