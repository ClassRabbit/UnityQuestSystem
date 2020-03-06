using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal class SearchWindowQuestTab
    {

        private const int KRowPerPage = 100;
        private const int KSkipPageIndex = -1;
        private const float KDetailAreaRatio = 0.25f;
        private const int KPageAreaHeight = 100;
        private const int KTopAreaHeight = 60;
        private const int KTabPadding = 10;

        private Rect _tabPosition;
        
        private Vector2 _tableScrollPosition;
        private Vector2 _detailScrollPosition;
        private string _searchText;
        
        MultiColumnHeader _columnHeader;
        MultiColumnHeaderState.Column[] _columns;
        
        

        private List<QuestData> _questDataList;

        private bool _isSearch = false;
        private List<QuestData> _searchResultQuestDataList;

        private int? _selectedQuestDataIndex;
        private int _currentPageIndex = 0;
        private int _maxPageIndex = 0;
        List<int> _showingPageIndexList = new List<int>();
        
        List<QuestData> TargetQuestDataList => _isSearch ? _searchResultQuestDataList : _questDataList;

        internal void EnableProcess()
        {
        }
        
        internal void FocusProcess(List<QuestData> questDataList)
        {
            QuestData _selectedQuestData = null;
            if (_selectedQuestDataIndex.HasValue)
            {
                _selectedQuestData = _questDataList[_selectedQuestDataIndex.Value];
            }
            _questDataList = questDataList;

            if (_isSearch)
            {
                ChangeSearchResultQuestList();
            }
            
            
            

            if (_selectedQuestData != null)
            {
                bool isFound = false;
                for (int i = 0; i < TargetQuestDataList.Count; ++i)
                {
                    if (TargetQuestDataList[i].QuestId == _selectedQuestData.QuestId)
                    {
                        isFound = true;
                        _selectedQuestDataIndex = i;
                        break;
                    }
                }

                if (!isFound)
                {
                    _selectedQuestDataIndex = null;
                }
            }
            
            ChangeShowingPageList();
            
            //바뀌면서 현재페이지가 범위밖일수도있다.
            if (_currentPageIndex > _maxPageIndex)
            {
                _currentPageIndex = _maxPageIndex;
            }
        }
        
        void ChangeSearchResultQuestList()
        {
            _searchResultQuestDataList = new List<QuestData>();
            foreach(var questData in _questDataList)
            {
                if (questData.QuestId.Contains(_searchText) || questData.Description.Contains(_searchText))
                {
                    _searchResultQuestDataList.Add(questData);
                }
            }
        }

        void ChangeShowingPageList()
        {
            _maxPageIndex = CalcPageIndex(TargetQuestDataList.Count - 1);
            _showingPageIndexList.Clear();
            _showingPageIndexList.Add(0);

            int additionalAreaCount = 3;
            
            //... 추가
            if (_currentPageIndex - additionalAreaCount > 1)
            {
                _showingPageIndexList.Add(KSkipPageIndex);
            }

            for (int i = _currentPageIndex - additionalAreaCount; i <= _currentPageIndex + additionalAreaCount && i <= _maxPageIndex; ++i)
            {
                if (i < 1)
                {
                    continue;
                }
                _showingPageIndexList.Add(i);
            }

            if (_showingPageIndexList[_showingPageIndexList.Count - 1] + 1 < _maxPageIndex - 1)
            {
                _showingPageIndexList.Add(KSkipPageIndex);
            }

            
            if (!_showingPageIndexList.Contains(_maxPageIndex))
            {
                _showingPageIndexList.Add(_maxPageIndex);
            }
        }

        int CalcPageIndex(int questDataIndex) => questDataIndex / KRowPerPage;
        
        internal void GUIProcess(Rect tabPosition)
        {
            if (_tabPosition != tabPosition || null == _columns)
            {
                _tabPosition = tabPosition;
                ResizeColumn();

            }

            float detailHeight = _tabPosition.height * KDetailAreaRatio;
            //상단 여백
            GUILayout.Space(KTabPadding);
            //검색창, 생성버튼
            DrawTopBar();
            //테이블 그리기
            DrawTable(detailHeight);
            DrawDetail(detailHeight);

        }

        void DrawTopBar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(4);
                    _searchText = GUILayout.TextField(_searchText, "SearchTextField");
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("Search", GUILayout.Width(100)))
                {
                    if (string.IsNullOrEmpty(_searchText))
                    {
                        _isSearch = false;
                    }
                    else
                    {
                        _isSearch = true;
                        ChangeSearchResultQuestList();
                    }
                    
                    ChangeShowingPageList();
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(KTabPadding);

        }

        

        void DrawTable(float detailHeight)
        {
            _columnHeader = new MultiColumnHeader(new MultiColumnHeaderState(_columns));
            _columnHeader.height = 25;
            _columnHeader.ResizeToFit();
            
            // calculate the window visible rect
            GUILayout.FlexibleSpace();
            var windowVisibleRect = GUILayoutUtility.GetLastRect();
            windowVisibleRect.width = _tabPosition.width;
            windowVisibleRect.height = _tabPosition.height;
 
            // draw the column headers
            var headerRect = windowVisibleRect;
            headerRect.height = _columnHeader.height;
            float xScroll = 0;
            _columnHeader.OnGUI(headerRect, xScroll);
 
            GUILayout.Space(25);
            
            GUILayout.BeginArea(new Rect(_tabPosition.x, _tabPosition.y + KTopAreaHeight, _tabPosition.width, _tabPosition.height - KPageAreaHeight - detailHeight));
            
            _tableScrollPosition = GUILayout.BeginScrollView(_tableScrollPosition);

            List<QuestData> targetQuestDataList = TargetQuestDataList;
            
            if (targetQuestDataList != null)
            {
                QuestData _selectedQuestData = _selectedQuestDataIndex.HasValue ? targetQuestDataList[_selectedQuestDataIndex.Value] : null;
                List<int> _pageList = new List<int>();
                
                for (int i = KRowPerPage * _currentPageIndex;
                    i < targetQuestDataList.Count && i < KRowPerPage * (_currentPageIndex + 1);
                    ++i)
                {
                    var questData = targetQuestDataList[i];
                    if (null != _selectedQuestData && questData.QuestId == _selectedQuestData.QuestId)
                    {
                        GUILayout.BeginHorizontal("LODSliderRangeSelected");
                    }
                    else
                    {
                        GUILayout.BeginHorizontal("box");
                    }

                    if (GUILayout.Button(questData.QuestId, "FrameBox",
                        GUILayout.Width(_columnHeader.GetColumn(0).width - 10)))
                    {
                        _selectedQuestDataIndex = i;
                    }

                    if (GUILayout.Button(questData.Description, "FrameBox",
                        GUILayout.Width(_columnHeader.GetColumn(1).width - 10)))
                    {
                        _selectedQuestDataIndex = i;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            bool isChangingPage = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(_tabPosition.width * 0.1f);
            
            MakePageButton(_currentPageIndex == 0, "<", 
                () => {
                    --_currentPageIndex;
                    isChangingPage = true;
                });
            foreach (var pageIndex in _showingPageIndexList)
            {
                if (KSkipPageIndex == pageIndex)
                {
                    MakePageButton(true, "...", () => { });
                }
                else
                {
                    MakePageButton(pageIndex == _currentPageIndex, (pageIndex + 1).ToString(), 
                        () => {
                            _currentPageIndex = pageIndex;
                            isChangingPage = true;
                        });
                }
            }
            MakePageButton(_currentPageIndex == _maxPageIndex, ">", 
                () => {
                    ++_currentPageIndex;
                    isChangingPage = true;
                });
            GUILayout.Space(_tabPosition.width * 0.1f);
            GUILayout.EndHorizontal();

            if (isChangingPage)
            {
                ChangeShowingPageList();
            }
            
            GUILayout.Space(7);
            
            Rect tableBottomBarRect = new Rect(_tabPosition.x, _tabPosition.height - detailHeight - KTabPadding, _tabPosition.width, 4);
            
            EditorGUI.DrawRect(tableBottomBarRect, new Color32(221, 221, 221, 255));
        }


        void MakePageButton(bool isDisabled, string buttonText, Action buttonAction)
        {
            EditorGUI.BeginDisabledGroup(isDisabled);
            if (GUILayout.Button(buttonText))
            {
                buttonAction();
            }
            EditorGUI.EndDisabledGroup();
        }
        

        void DrawDetail(float detailHeight)
        {
            _detailScrollPosition = GUILayout.BeginScrollView(_detailScrollPosition, "TE ElementBackground",
                GUILayout.Height(_tabPosition.height * KDetailAreaRatio), GUILayout.Width(_tabPosition.width), GUILayout.ExpandWidth(false));

            if (_selectedQuestDataIndex.HasValue)
            {
                var questData = TargetQuestDataList[_selectedQuestDataIndex.Value];
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
            GUILayout.EndScrollView();
        }


        void ResizeColumn()
        {
            _columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("QuestId"),
                    width = _tabPosition.width * 0.3f,
                    minWidth = _tabPosition.width * 0.2f,
                    maxWidth = _tabPosition.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Description"),
                    width = _tabPosition.width * 0.7f,
                    minWidth = _tabPosition.width * 0.3f,
                    maxWidth = _tabPosition.width * 0.8f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
            };
        }
        
        
    }
}
