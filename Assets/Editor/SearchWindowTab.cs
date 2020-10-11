using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace QuestSystem
{
    /// <summary>
    ///     검색 창의 탭 추상 클래스
    /// </summary>
    internal abstract class SearchWindowTab<T> where T : class
    {

        #region Const

        private const int SkipPageIndex = -1;
        private const float DetailAreaRatio = 0.25f;
        private const int PageAreaHeight = 100;
        private const int TopAreaHeight = 60;
        private const int TabPadding = 10;

        protected const string DescriptionTextValue = "세부 정보";

        #endregion


        #region Variable
        
        protected List<T> DataList { get; set; }
        protected List<T> SearchResultDataList { get; set; }
        protected List<T> TargetDataList => string.IsNullOrEmpty(UsingSearchText) ? DataList : SearchResultDataList;
        
        //텝의 영역
        protected Rect TabPosition { get; set; }
        
        //스크롤 위치
        protected Vector2 TableScrollPosition { get; set; }
        protected Vector2 DetailScrollPosition { get; set; }
        
        //테이블 컬럼해더 
        protected MultiColumnHeader ColumnHeader { get; set; }
        protected MultiColumnHeaderState.Column[] Columns { get; set; }
        
        
        //입력바에 입력된 텍스트값
        protected string InputSearchText { get; set; } = string.Empty;
        //현재 검색에 적용된 텍스트값
        protected string UsingSearchText { get; set; } = string.Empty;
        
        
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
        ///     Focus시 데이터 업데이트
        /// </summary>
        internal void FocusProcess(List<T> dataList)
        {
            T selectedData = null;
            if (SelectedDataIndex.HasValue)
            {
                selectedData = DataList[SelectedDataIndex.Value];
            }
            DataList = dataList;

            if (!string.IsNullOrEmpty(UsingSearchText))
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

            float detailHeight = TabPosition.height * DetailAreaRatio;
            //상단 여백
            GUILayout.Space(TabPadding);
            //검색창, 생성버튼
            DrawSearchBar();
            //테이블 그리기
            DrawTable(detailHeight);
            //디테일 그리기
            DrawDetail(detailHeight);

        }

        /// <summary>
        ///     현재 페이지의 데이터 재확인
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
                ShowingPageIndexList.Add(SkipPageIndex);
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
                ShowingPageIndexList.Add(SkipPageIndex);
            }

            
            if (!ShowingPageIndexList.Contains(MaxPageIndex))
            {
                ShowingPageIndexList.Add(MaxPageIndex);
            }

            RefreshPageListProcess();
        }

        /// <summary>
        ///     검색바 그리기
        /// </summary>
        void DrawSearchBar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(4);
                    InputSearchText = GUILayout.TextField(InputSearchText, "SearchTextField");
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("조회", GUILayout.Width(100)))
                {
                    SelectedDataIndex = null;
                    UsingSearchText = InputSearchText;
                    TableScrollPosition = Vector2.zero;
                    ActionSearch();
                    
                    RefreshPageList();
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(TabPadding);

        }

        /// <summary>
        ///     테이블 그리기
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
            
            GUILayout.BeginArea(new Rect(TabPosition.x, TabPosition.y + TopAreaHeight, TabPosition.width, TabPosition.height - PageAreaHeight - detailHeight));
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
                    if (SkipPageIndex == pageIndex)
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
            Rect tableBottomBarRect = new Rect(TabPosition.x, TabPosition.height - detailHeight - TabPadding, TabPosition.width, 4);
            GUILayout.Space(3);
        }

        /// <summary>
        ///     페이지 버튼 생성
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
        ///     디테일 그리기
        /// </summary>
        void DrawDetail(float detailHeight)
        {
            DetailScrollPosition = GUILayout.BeginScrollView(DetailScrollPosition, "TE ElementBackground",
                GUILayout.Height(TabPosition.height * DetailAreaRatio), GUILayout.Width(TabPosition.width), GUILayout.ExpandWidth(false));
            DrawDetailProcess();
            GUILayout.EndScrollView();
        }

        #region AbstractFunction

        /// <summary>
        ///     두 데이터가 서로 같음을 확인하는 방식 결정
        /// </summary>
        protected abstract bool IsSameData(T ta, T tb);

        /// <summary>
        ///     현재 페이지 새롭게 그리기
        /// </summary>
        protected abstract void RefreshPageListProcess();

        /// <summary>
        ///     조회 실행
        /// </summary>
        protected abstract void ActionSearch();

        /// <summary>
        ///     테이블 그리기
        /// </summary>
        protected abstract void DrawTableProcess();

        /// <summary>
        ///     세부 정보 그리기
        /// </summary>
        protected abstract void DrawDetailProcess();

        /// <summary>
        ///     테이블 컬럼 resize될시 실행
        /// </summary>
        protected abstract void ResizeColumn();
        
        #endregion

    }
}
