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

        protected override bool CompareSearchText(QuestData data)
        {
            return (data.QuestId.Contains(SearchText) || data.Description.Contains(SearchText));
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
                GUILayout.Label(questData.QuestId);
                GUILayout.Space(3);
                GUILayout.Label(questData.Description);
                if (GUILayout.Button("Edit"))
                {
                    EditQuestWindow window = (EditQuestWindow)EditorWindow.GetWindow(typeof(EditQuestWindow));
                    window.Show();
                    window.UpdateQuestData(questData);
                }
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
