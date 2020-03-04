using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace QuestSystem
{

    internal class SearchWindowQuestTab
    {
        
//        private static Texture2D _rowTexture;
//        public static Texture2D RowTexture {
//            get {
//                if (_rowTexture == null ) {
//                    _rowTexture = new Texture2D ( 1 , 1 );
//                    _rowTexture.SetPixel ( 0 , 0 , new Color32 ( 255 , 255 , 255 , 255 ));
//                }
//                return _rowTexture;
//            }
//        }

        private const float KToolbarPadding = 15;

        private Rect _tabPosition;
        
        private Vector2 _tableScrollPosition;
        private Vector2 _detailScrollPosition;
        private string _searchText;
        
        MultiColumnHeader _columnHeader;
        MultiColumnHeaderState.Column[] _columns;
        private int? _selectedRowIdx;

        private const float KDetailAreaRatio = 0.2f;
        
        
        internal void EnableProcess(Rect tabPosition)
        {
            _tabPosition = tabPosition;
        }
        
        internal void OnGUI(Rect tabPosition)
        {
            if (_tabPosition != tabPosition || null == _columns)
            {
                _tabPosition = tabPosition;
                ResizeColumn();

            }
            
            //상단 여백
            GUILayout.Space(10);
            //검색창, 생성버튼
            DrawTopBar();
            //테이블 그리기
            DrawTable();
            DrawDetail();

            //하단 여백
            GUILayout.Space(10);
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
//            for (int i = 0; i < words.Count; i++)
//            {
//                if (string.IsNullOrEmpty(searchText) || words[i].Contains(searchText))
//                {
//                    GUILayout.Button(words[i]);
//                }
//            }

                if (GUILayout.Button("Create", GUILayout.Width(100)))
                {
                
                }
                if (GUILayout.Button("DeleteAll", GUILayout.Width(100)))
                {
                
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);

        }

        void DrawTable()
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
            
            if (_selectedRowIdx.HasValue)
            {
                GUILayout.BeginArea(new Rect(_tabPosition.x, _tabPosition.y + 60, _tabPosition.width, _tabPosition.height - 60 - (_tabPosition.height * KDetailAreaRatio)));
            }
            else
            {
                GUILayout.BeginArea(new Rect(_tabPosition.x, _tabPosition.y + 60, _tabPosition.width, _tabPosition.height - 60));
            }
            
            _tableScrollPosition = GUILayout.BeginScrollView(_tableScrollPosition);
            for (int rowIdx = 0; rowIdx < 20; ++rowIdx)
            {

                if (rowIdx == _selectedRowIdx)
                {
                    GUILayout.BeginHorizontal("LODSliderRangeSelected");
                }
                else
                {
                    GUILayout.BeginHorizontal("box");
                }
                
                if (GUILayout.Button("Test" + rowIdx, "FrameBox", GUILayout.Width(_columnHeader.GetColumn(0).width - 10)))
                {
//                    Debug.LogError(rowIdx);
                    _selectedRowIdx = rowIdx;
                }

                if (GUILayout.Button("DDDDDD", "FrameBox", GUILayout.Width(_columnHeader.GetColumn(1).width - 10)))
                {
//                    Debug.LogError(rowIdx);
                    _selectedRowIdx = rowIdx;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawDetail()
        {
            if (_selectedRowIdx.HasValue)
            {
                _detailScrollPosition = GUILayout.BeginScrollView(_detailScrollPosition, "TE ElementBackground",
                    GUILayout.Height(_tabPosition.height * KDetailAreaRatio), GUILayout.Width(_tabPosition.width), GUILayout.ExpandWidth(false));
                GUILayout.Space(3);
                GUILayout.Label("QuestID");
                GUILayout.Space(3);
                GUILayout.Label("DescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescription");
                GUILayout.EndScrollView();
            }

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
