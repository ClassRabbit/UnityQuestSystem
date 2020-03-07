using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal class SearchWindowSwitchTab : SearchWindowTab
    {
        private const int KSwitchPerPage = 100;
        
        private List<SwitchDescriptionData> _descriptionDataList;
        private List<SwitchDescriptionData> _searchResultDescriptionDataList;
        List<SwitchDescriptionData> TargetDescriptionDataList => IsSearch ? _searchResultDescriptionDataList : _descriptionDataList;
        
        protected override int MaxPageIndex => (TargetDescriptionDataList.Count - 1) / KSwitchPerPage;


        internal void EnableProcess()
        {
            
        }
        
        internal void FocusProcess(List<SwitchDescriptionData> descriptionDataList)
        {
            SwitchDescriptionData selectedDescriptionData = null;
            if (SelectedDataIndex.HasValue)
            {
                selectedDescriptionData = _descriptionDataList[SelectedDataIndex.Value];
            }
            _descriptionDataList = descriptionDataList;

            if (IsSearch)
            {
                RefreshSearchResultList();
            }
            if (selectedDescriptionData != null)
            {
                bool isFound = false;
                for (int i = 0; i < TargetDescriptionDataList.Count; ++i)
                {
                    if (TargetDescriptionDataList[i].SwitchId == selectedDescriptionData.SwitchId)
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
            _searchResultDescriptionDataList = new List<SwitchDescriptionData>();
            foreach(var descriptionData in _descriptionDataList)
            {
                if (descriptionData.SwitchId.Contains(SearchText) || descriptionData.Description.Contains(SearchText))
                {
                    _searchResultDescriptionDataList.Add(descriptionData);
                }
            }
        }

        


        protected override void DrawTableProcess()
        {
//            List<SwitchDescriptionData> targetSwitchDataList = TargetDescriptionDataList;
//            
//            if (targetQuestDataList != null)
//            {
//                SwitchDescriptionData selectedQuestData = SelectedDataIndex.HasValue ? targetQuestDataList[SelectedDataIndex.Value] : null;
//
//                for (int i = KSwitchPerPage * CurrentPageIndex;
//                    i < targetQuestDataList.Count && i < KSwitchPerPage * (CurrentPageIndex + 1);
//                    ++i)
//                {
//                    var questData = targetQuestDataList[i];
//                    if (null != selectedQuestData && questData.QuestId == selectedQuestData.QuestId)
//                    {
//                        GUILayout.BeginHorizontal("LODSliderRangeSelected");
//                    }
//                    else
//                    {
//                        GUILayout.BeginHorizontal("box");
//                    }
//
//                    if (GUILayout.Button(questData.QuestId, "FrameBox",
//                        GUILayout.Width(ColumnHeader.GetColumn(0).width - 10)))
//                    {
//                        SelectedDataIndex = i;
//                    }
//
//                    if (GUILayout.Button(questData.Description, "FrameBox",
//                        GUILayout.Width(ColumnHeader.GetColumn(1).width - 10)))
//                    {
//                        SelectedDataIndex = i;
//                    }
//
//                    GUILayout.EndHorizontal();
//                }
//            }
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
                    headerContent = new GUIContent("QuestId"),
                    width = TabPosition.width * 0.3f,
                    minWidth = TabPosition.width * 0.2f,
                    maxWidth = TabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("State"),
                    width = TabPosition.width * 0.7f,
                    minWidth = TabPosition.width * 0.3f,
                    maxWidth = TabPosition.width * 0.8f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Operator"),
                    width = TabPosition.width * 0.7f,
                    minWidth = TabPosition.width * 0.3f,
                    maxWidth = TabPosition.width * 0.8f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Result"),
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
