﻿using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal class SearchWindowQuestTab : SearchWindowTab<QuestData>
    {
        #region Const

        private const int KQuestPerPage = 100;
        private const string KQuestIdText = "QuestId";

        #endregion
        
        #region Variable
        
        protected override int MaxPageIndex => (TargetDataList.Count - 1) / KQuestPerPage;

        #endregion

        /// <summary>
        ///   <para>두 데이터가 서로 같음을 확인하는 방식 결정</para>
        /// </summary>
        protected override bool IsSameData(QuestData ta, QuestData tb)
        {
            return ta.QuestId == tb.QuestId;
        }
        
        /// <summary>
        ///   <para>현재 페이지 새롭게 그리기</para>
        /// </summary>
        protected override void RefreshPageListProcess()
        {
            // 추가 행동 없음
        }
        
        /// <summary>
        ///   <para>조회 실행</para>
        /// </summary>
        protected override void ActionSearch()
        {
            if (IsSearch)
            {
                SearchResultDataList = new List<QuestData>();
                foreach(var data in DataList)
                {
                    if (data.QuestId.Contains(SearchText) || data.Description.Contains(SearchText))
                    {
                        SearchResultDataList.Add(data);
                    }
                }
            }
        }

        /// <summary>
        ///   <para>테이블 그리기</para>
        /// </summary>
        protected override void DrawTableProcess()
        {
            List<QuestData> targetQuestDataList = TargetDataList;
            
            if (targetQuestDataList != null)
            {
                QuestData selectedQuestData = SelectedDataIndex.HasValue ? targetQuestDataList[SelectedDataIndex.Value] : null;

                for (int i = KQuestPerPage * CurrentPageIndex;
                    i < targetQuestDataList.Count && i < KQuestPerPage * (CurrentPageIndex + 1);
                    ++i)
                {
                    var questData = targetQuestDataList[i];
                    if (null != selectedQuestData && questData.QuestId == selectedQuestData.QuestId)
                    {
                        GUILayout.BeginHorizontal("LODSliderRangeSelected");
                    }
                    else
                    {
                        GUILayout.BeginHorizontal("box");
                    }

                    if (GUILayout.Button(questData.QuestId, "FrameBox",
                        GUILayout.Width(ColumnHeader.GetColumn(0).width - 10)))
                    {
                        SelectedDataIndex = i;
                    }

                    if (GUILayout.Button(questData.Description, "FrameBox",
                        GUILayout.Width(ColumnHeader.GetColumn(1).width - 10)))
                    {
                        SelectedDataIndex = i;
                    }

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
                var questData = TargetDataList[SelectedDataIndex.Value];
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Space(8);
                int beforeFontSize = EditorStyles.label.fontSize;
                EditorStyles.label.fontSize = 20;
                GUILayout.Label(questData.QuestId, EditorStyles.label);
                EditorStyles.label.fontSize = beforeFontSize;
                GUILayout.EndVertical();
                
                if (GUILayout.Button(TexturUpdateIcon, GUILayout.Width(40)))
                {
                    EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
                    window.Show();
                    window.UpdateQuestData(questData);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
                
                bool beforeWordWrap = EditorStyles.label.wordWrap;
                var beforeWordAlignment = EditorStyles.label.alignment;
                EditorStyles.label.wordWrap = true;
                EditorStyles.label.alignment = TextAnchor.UpperLeft;
                GUILayout.Label(questData.Description, EditorStyles.label,  GUILayout.Width(TabPosition.width - 10), GUILayout.ExpandHeight(true));
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
                    headerContent = new GUIContent(KQuestIdText),
                    width = TabPosition.width * 0.3f,
                    minWidth = TabPosition.width * 0.2f,
                    maxWidth = TabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(KDescriptionText),
                    width = TabPosition.width * 0.7f,
                    minWidth = TabPosition.width * 0.3f,
                    maxWidth = TabPosition.width * 0.8f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                }
            };
        }
        
        
    }
}
