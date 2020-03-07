using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal class SearchWindowQuestTab : SearchWindowTab
    {
        private const int KQuestPerPage = 100;
        
        private List<QuestData> _questDataList;
        private List<QuestData> _searchResultQuestDataList;
        List<QuestData> TargetQuestDataList => IsSearch ? _searchResultQuestDataList : _questDataList;
        
        protected override int MaxPageIndex => (TargetQuestDataList.Count - 1) / KQuestPerPage;


        internal void EnableProcess()
        {
            
        }
        
        internal void FocusProcess(List<QuestData> questDataList)
        {
            QuestData selectedQuestData = null;
            if (SelectedDataIndex.HasValue)
            {
                selectedQuestData = _questDataList[SelectedDataIndex.Value];
            }
            _questDataList = questDataList;

            if (IsSearch)
            {
                RefreshSearchResultList();
            }
            if (selectedQuestData != null)
            {
                bool isFound = false;
                for (int i = 0; i < TargetQuestDataList.Count; ++i)
                {
                    if (TargetQuestDataList[i].QuestId == selectedQuestData.QuestId)
                    {
                        isFound = true;
                        SelectedDataIndex = i;
                        break;
                    }
                }

                if (!isFound)
                {
                    SelectedDataIndex = null;
                }
            }
            
            RefreshPageList();
        }
        
        
        protected override void RefreshSearchResultList()
        {
            _searchResultQuestDataList = new List<QuestData>();
            foreach(var questData in _questDataList)
            {
                if (questData.QuestId.Contains(SearchText) || questData.Description.Contains(SearchText))
                {
                    _searchResultQuestDataList.Add(questData);
                }
            }
        }

        


        protected override void DrawTableProcess()
        {
            List<QuestData> targetQuestDataList = TargetQuestDataList;
            
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
                var questData = TargetQuestDataList[SelectedDataIndex.Value];
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
