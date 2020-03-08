using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal class SearchWindowQuestTab : SearchWindowTab<QuestData>
    {
        private const int KQuestPerPage = 100;

        protected override int MaxPageIndex => (TargetDataList.Count - 1) / KQuestPerPage;


        internal void EnableProcess()
        {
            
        }
        
        protected override bool CompareData(QuestData ta, QuestData tb)
        {
            return ta.QuestId == tb.QuestId;
        }

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
                EditorStyles.label.wordWrap = true;
                GUILayout.Label(questData.Description, EditorStyles.label);
                EditorStyles.label.wordWrap = beforeWordWrap;
                
                
            }
        }


        protected override void ResizeColumn()
        {
            Columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("QuestId"),
                    width = TabPosition.width * 0.3f,
                    minWidth = TabPosition.width * 0.2f,
                    maxWidth = TabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Description"),
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
