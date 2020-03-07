using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{

    internal abstract class SearchWindowTab
    {

        
        private const int KSkipPageIndex = -1;
        private const float KDetailAreaRatio = 0.25f;
        private const int KPageAreaHeight = 100;
        private const int KTopAreaHeight = 60;
        private const int KTabPadding = 10;
        
        //텝의 영역
        protected Rect TabPosition { get; set; }
        
        //스크롤 위치
        protected Vector2 TableScrollPosition { get; set; }
        protected Vector2 DetailScrollPosition { get; set; }
        
        //테이블 컬럼해더 
        protected MultiColumnHeader ColumnHeader { get; set; }
        protected MultiColumnHeaderState.Column[] Columns { get; set; }
        
        //조회 
        protected bool IsSearch { get; set; } = false;
        protected string SearchText { get; set; }
        
        
        //선택된 Row
        protected int? SelectedDataIndex { get; set; }
        
        //페이지
        protected List<int> ShowingPageIndexList { get; set; }  = new List<int>();
        protected int CurrentPageIndex { get; set; }
        protected virtual int MaxPageIndex => 0;


        protected virtual void Initialize()
        {
            
        }

        protected virtual void RefreshSearchResultList()
        {
        }


        protected void RefreshPageList()
        {
            if (MaxPageIndex <= CurrentPageIndex)
            {
                CurrentPageIndex = MaxPageIndex;
            }
            ShowingPageIndexList.Clear();
            ShowingPageIndexList.Add(0);

            int additionalAreaCount = 3;
            
            //... 추가
            if (CurrentPageIndex - additionalAreaCount > 1)
            {
                ShowingPageIndexList.Add(KSkipPageIndex);
            }

            for (int i = CurrentPageIndex - additionalAreaCount; i <= CurrentPageIndex + additionalAreaCount && i <= MaxPageIndex; ++i)
            {
                if (i < 1)
                {
                    continue;
                }
                ShowingPageIndexList.Add(i);
            }

            if (ShowingPageIndexList[ShowingPageIndexList.Count - 1] + 1 < MaxPageIndex - 1)
            {
                ShowingPageIndexList.Add(KSkipPageIndex);
            }

            
            if (!ShowingPageIndexList.Contains(MaxPageIndex))
            {
                ShowingPageIndexList.Add(MaxPageIndex);
            }
        }

        internal void GUIProcess(Rect tabPosition)
        {
            if (TabPosition != tabPosition || null == Columns)
            {
                TabPosition = tabPosition;
                ResizeColumn();
            }

            float detailHeight = TabPosition.height * KDetailAreaRatio;
            //상단 여백
            GUILayout.Space(KTabPadding);
            //검색창, 생성버튼
            DrawSearchBar();
            //테이블 그리기
            DrawTable(detailHeight);
            //디테일 그리기
            DrawDetail(detailHeight);

        }

        void DrawSearchBar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(4);
                    SearchText = GUILayout.TextField(SearchText, "SearchTextField");
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("조회", GUILayout.Width(100)))
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        IsSearch = false;
                    }
                    else
                    {
                        IsSearch = true;
                        RefreshSearchResultList();
                    }
                    
                    RefreshPageList();
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(KTabPadding);

        }

        

        
        
        
        
        
        
        void DrawTable(float detailHeight)
        {
            ColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(Columns));
            ColumnHeader.height = 25;
            ColumnHeader.ResizeToFit();
            
            // calculate the window visible rect
            GUILayout.FlexibleSpace();
            var windowVisibleRect = GUILayoutUtility.GetLastRect();
            windowVisibleRect.width = TabPosition.width;
            windowVisibleRect.height = TabPosition.height;
 
            // draw the column headers
            var headerRect = windowVisibleRect;
            headerRect.height = ColumnHeader.height;
            float xScroll = 0;
            ColumnHeader.OnGUI(headerRect, xScroll);
 
            GUILayout.Space(25);
            
            GUILayout.BeginArea(new Rect(TabPosition.x, TabPosition.y + KTopAreaHeight, TabPosition.width, TabPosition.height - KPageAreaHeight - detailHeight));
            TableScrollPosition = GUILayout.BeginScrollView(TableScrollPosition);
            {
                DrawTableProcess();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            bool isChangingPage = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(TabPosition.width * 0.1f);
            
            MakePageButton(CurrentPageIndex == 0, "<", 
                () => {
                    --CurrentPageIndex;
                    isChangingPage = true;
                });
            foreach (var pageIndex in ShowingPageIndexList)
            {
                if (KSkipPageIndex == pageIndex)
                {
                    MakePageButton(true, "...", () => { });
                }
                else
                {
                    MakePageButton(pageIndex == CurrentPageIndex, (pageIndex + 1).ToString(), 
                        () => {
                            CurrentPageIndex = pageIndex;
                            isChangingPage = true;
                        });
                }
            }
            MakePageButton(CurrentPageIndex == MaxPageIndex, ">", 
                () => {
                    ++CurrentPageIndex;
                    isChangingPage = true;
                });
            GUILayout.Space(TabPosition.width * 0.1f);
            GUILayout.EndHorizontal();

            if (isChangingPage)
            {
                RefreshPageList();
            }
            
            //맨밑 경계
            GUILayout.Space(7);
            Rect tableBottomBarRect = new Rect(TabPosition.x, TabPosition.height - detailHeight - KTabPadding, TabPosition.width, 4);
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

        protected virtual void DrawTableProcess()
        {
            
        }


        
        
        
        
        
        void DrawDetail(float detailHeight)
        {
            DetailScrollPosition = GUILayout.BeginScrollView(DetailScrollPosition, "TE ElementBackground",
                GUILayout.Height(TabPosition.height * KDetailAreaRatio), GUILayout.Width(TabPosition.width), GUILayout.ExpandWidth(false));
            DrawDetailProcess();
            GUILayout.EndScrollView();
        }

        protected virtual void DrawDetailProcess()
        {
            
        }


        protected virtual void ResizeColumn()
        {
        }
        
        
    }
}
