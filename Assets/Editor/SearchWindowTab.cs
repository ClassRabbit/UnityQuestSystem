using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{
    
    internal abstract class SearchWindowTab<T> where T : class
    {

        #region Const

        private const int KSkipPageIndex = -1;
        private const float KDetailAreaRatio = 0.25f;
        private const int KPageAreaHeight = 100;
        private const int KTopAreaHeight = 60;
        private const int KTabPadding = 10;
        
        
        protected const string KDescriptionText = "세부 정보";

        #endregion


        #region Variable
        
        protected List<T> DataList { get; set; }
        protected List<T> SearchResultDataList { get; set; }
        protected List<T> TargetDataList => IsSearch ? SearchResultDataList : DataList;
        
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

        private Texture2D _texturUpdateIcon = null;

        public Texture2D TexturUpdateIcon
        {
            get
            {
                if (_texturUpdateIcon == null)
                {
                    _texturUpdateIcon = (Texture2D)EditorGUIUtility.Load("Icon_Update.png");
                }

                return _texturUpdateIcon;
            }
        }

        //페이지
        protected List<int> ShowingPageIndexList { get; set; }  = new List<int>();
        protected int CurrentPageIndex { get; set; }
        protected virtual int MaxPageIndex => 0;
        
        #endregion
        
        
        /// <summary>
        ///   <para>Focus시 데이터 업데이트</para>
        /// </summary>
        internal void FocusProcess(List<T> dataList)
        {
            T selectedData = null;
            if (SelectedDataIndex.HasValue)
            {
                selectedData = DataList[SelectedDataIndex.Value];
            }
            DataList = dataList;

            if (IsSearch)
            {
                ActionSearch();
            }
            if (selectedData != null)
            {
                bool isFound = false;
                var targetDataList = TargetDataList;
                for (int i = 0; i < targetDataList.Count; ++i)
                {
                    if (IsSameData(selectedData, targetDataList[i]))
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

        /// <summary>
        ///   <para>현재 페이지의 데이터 재확인</para>
        /// </summary>
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

            RefreshPageListProcess();
        }
        
        /// <summary>
        ///   <para>검색바 그리기</para>
        /// </summary>
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
                    SelectedDataIndex = null;
                    IsSearch = !string.IsNullOrEmpty(SearchText);
                    
                    ActionSearch();
                    
                    RefreshPageList();
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(KTabPadding);

        }

        /// <summary>
        ///   <para>테이블 그리기</para>
        /// </summary>
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

            if (TargetDataList != null)
            {
                CreatePageButton(CurrentPageIndex == 0, "<", 
                    () => {
                        --CurrentPageIndex;
                        isChangingPage = true;
                    });
                foreach (var pageIndex in ShowingPageIndexList)
                {
                    if (KSkipPageIndex == pageIndex)
                    {
                        CreatePageButton(true, "...", () => { });
                    }
                    else
                    {
                        CreatePageButton(pageIndex == CurrentPageIndex, (pageIndex + 1).ToString(), 
                            () => {
                                CurrentPageIndex = pageIndex;
                                isChangingPage = true;
                            });
                    }
                }
                CreatePageButton(CurrentPageIndex == MaxPageIndex, ">", 
                    () => {
                        ++CurrentPageIndex;
                        isChangingPage = true;
                    });
            }
            
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
        
        /// <summary>
        ///   <para>페이지 버튼 생성</para>
        /// </summary>
        void CreatePageButton(bool isDisabled, string buttonText, Action buttonAction)
        {
            EditorGUI.BeginDisabledGroup(isDisabled);
            if (GUILayout.Button(buttonText))
            {
                buttonAction();
                TableScrollPosition = Vector2.zero;
            }
            EditorGUI.EndDisabledGroup();
        }
        
        /// <summary>
        ///   <para>디테일 그리기</para>
        /// </summary>
        void DrawDetail(float detailHeight)
        {
            DetailScrollPosition = GUILayout.BeginScrollView(DetailScrollPosition, "TE ElementBackground",
                GUILayout.Height(TabPosition.height * KDetailAreaRatio), GUILayout.Width(TabPosition.width), GUILayout.ExpandWidth(false));
            DrawDetailProcess();
            GUILayout.EndScrollView();
        }
        
        

        
        
        #region AbstractFunction
        
        /// <summary>
        ///   <para>두 데이터가 서로 같음을 확인하는 방식 결정</para>
        /// </summary>
        protected abstract bool IsSameData(T ta, T tb);
        
        /// <summary>
        ///   <para>현재 페이지 새롭게 그리기</para>
        /// </summary>
        protected abstract void RefreshPageListProcess();
        
        /// <summary>
        ///   <para>조회 실행</para>
        /// </summary>
        protected abstract void ActionSearch();
        
        /// <summary>
        ///   <para>테이블 그리기</para>
        /// </summary>
        protected abstract void DrawTableProcess();
        
        /// <summary>
        ///   <para>세부 정보 그리기</para>
        /// </summary>
        protected abstract void DrawDetailProcess();
        
        /// <summary>
        ///   <para>테이블 컬럼 resize될시 실행</para>
        /// </summary>
        protected abstract void ResizeColumn();
        
        #endregion

    }
}
