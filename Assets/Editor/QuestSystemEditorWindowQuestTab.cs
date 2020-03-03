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
            public string Discription { get; set; } = string.Empty;
        }

        #endregion
        
        
        private QuestMakeData _questMakeData;
        
        private static Texture2D _rowTexture;
        public static Texture2D RowTexture {
            get {
                if (_rowTexture == null ) {
                    _rowTexture = new Texture2D ( 1 , 1 );
//                    _rowTexture.SetPixel ( 0 , 0 , new Color32 ( 222 , 222 , 222 , 255 ));
                    _rowTexture.SetPixel ( 0 , 0 , new Color32 ( 255 , 255 , 255 , 255 ));
                }
                return _rowTexture;
            }
        }
        
        private GUIStyle guiStyle = new GUIStyle();

        private Rect _position;
        private Vector2 _scrollPosition;
        
        MultiColumnHeader columnHeader;
        MultiColumnHeaderState.Column[] columns;
        
        
        internal void OnEnable(Rect position)
        {
            _position = position;
            
            if (null == _questMakeData)
            {
                _questMakeData = new QuestMakeData();
            }
            
            
        }

        
        
        internal void OnGUI(Rect position)
        {
            if (_position != position || null == columns)
            {
                _position = position;
                ResizeColumn();

            }
            
            GUILayout.Space(10);
            
            //
            //
            //
            
            columnHeader = new MultiColumnHeader(new MultiColumnHeaderState(columns));
            columnHeader.height = 25;
            columnHeader.ResizeToFit();
            
            // calculate the window visible rect
            GUILayout.FlexibleSpace();
            var windowVisibleRect = GUILayoutUtility.GetLastRect();
            windowVisibleRect.width = position.width;
            windowVisibleRect.height = position.height;
 
            // draw the column headers
            var headerRect = windowVisibleRect;
            headerRect.height = columnHeader.height;
            float xScroll = 0;
            columnHeader.OnGUI(headerRect, xScroll);
 
            GUILayout.Space(25);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < 40; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box("Test", GUILayout.Width(columnHeader.GetColumn(0).width-6));
                GUILayout.Box("Test2", GUILayout.Width(columnHeader.GetColumn(1).width-6));
                GUILayout.EndHorizontal();
                
            }
            GUILayout.EndScrollView();
            
            
//            for (int n = 0; n < 20; ++n)
//            {
//                for (int i = 0; i < columns.Length; i++)
//                {
//                    // calculate column content rect
//                    var contentRect = columnHeader.GetColumnRect(i);
//                    contentRect.x -= xScroll;
//                    contentRect.y = contentRect.yMax;
//                    contentRect.yMax = windowVisibleRect.yMax;
//
//                    // custom content GUI...
//                    //                GUI.DrawTexture(contentRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(1f, 0f, 0f, 0.5f), 10, 10);
//                    GUI.Label(contentRect, "Test");
//                }
//            }
            
            GUILayout.Space(10);
        }


        void ResizeColumn()
        {
            columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("col1"),
                    width = _position.width * 0.3f,
                    minWidth = _position.width * 0.2f,
                    maxWidth = _position.width * 0.7f,
                    autoResize = true,
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("col2"),
                    width = _position.width * 0.7f,
                    minWidth = _position.width * 0.3f,
                    maxWidth = _position.width * 0.8f,
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
                _questMakeData.Discription = GUILayout.TextArea(_questMakeData.Discription, GUILayout.MaxWidth(width), GUILayout.Height(300));
            }
            GUILayout.EndVertical();
            
            
        }
        
        
        
        
    }
}
