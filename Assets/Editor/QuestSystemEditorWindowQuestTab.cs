using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace QuestSystem
{

    internal class QuestSystemEditorWindowQuestTab
    {

        
        #region QuestMakeData

        class QuestMakeData
        {
            public string QuestId { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        #endregion
        
        
        private QuestMakeData _questMakeData;
        
//        private static Texture2D _rowTexture;
//        public static Texture2D RowTexture {
//            get {
//                if (_rowTexture == null ) {
//                    _rowTexture = new Texture2D ( 1 , 1 );
////                    _rowTexture.SetPixel ( 0 , 0 , new Color32 ( 222 , 222 , 222 , 255 ));
//                    _rowTexture.SetPixel ( 0 , 0 , new Color32 ( 255 , 255 , 255 , 255 ));
//                }
//                return _rowTexture;
//            }
//        }
        
        private GUIStyle guiStyle = new GUIStyle();

        private Rect _tabPosition;
        
        private Vector2 _tableScrollPosition;
        private Vector2 _detailScrollPosition;
        private string _searchText;
        
        MultiColumnHeader _columnHeader;
        MultiColumnHeaderState.Column[] _columns;
        private int? _selectedRowIdx;
        
        
        internal void OnEnable(Rect tabPosition)
        {
            _tabPosition = tabPosition;
            
            if (null == _questMakeData)
            {
                _questMakeData = new QuestMakeData();
            }

            guiStyle.alignment = TextAnchor.MiddleLeft;

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
            GUILayout.BeginArea(new Rect(_tabPosition.x, _tabPosition.y + 60, _tabPosition.width, _tabPosition.height - 60));
            _tableScrollPosition = GUILayout.BeginScrollView(_tableScrollPosition);
            for (int rowIdx = 0; rowIdx < 1; ++rowIdx)
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
                    Debug.LogError(rowIdx);
                    _selectedRowIdx = rowIdx;
                }

                if (GUILayout.Button("DDDDDD", "FrameBox", GUILayout.Width(_columnHeader.GetColumn(1).width - 10)))
                {
                    Debug.LogError(rowIdx);
                    _selectedRowIdx = rowIdx;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawDetail()
        {
//            if (_selectedRowIdx.HasValue)
//            {
//                _detailScrollPosition = GUILayout.BeginScrollView(_detailScrollPosition, "TE ElementBackground", GUILayout.Height(_tabPosition.height*0.2f), GUILayout.Width(_tabPosition.width), GUILayout.ExpandWidth(false));
//                GUILayout.Space(3);
//                GUILayout.Label("QuestID");
//                GUILayout.Space(3);
//                GUILayout.Label("DescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescription");
//                GUILayout.EndScrollView();
//            }

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
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

        void DrawEditArea(float width)
        {
            GUILayout.BeginVertical(GUILayout.Width(width));
            {
                guiStyle.fontSize = 20;
                GUILayout.Label("Create", guiStyle);
                //아이디 입력
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("QuestId ", EditorStyles.boldLabel, GUILayout.Width(70));
                    _questMakeData.QuestId = GUILayout.TextArea(_questMakeData.QuestId, GUILayout.MaxWidth(width - 70));
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
                //설명입력
                GUILayout.Label("Description", EditorStyles.boldLabel);
                _questMakeData.Description = GUILayout.TextArea(_questMakeData.Description, GUILayout.MaxWidth(width), GUILayout.Height(300));
            }
            GUILayout.EndVertical();
            
            
        }
        
        
        
        
    }
}
