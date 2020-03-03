using System;
using UnityEngine;
using UnityEditor;

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
        
        
        enum ESection
        {
            Show = 0,
            Edit = 1
        }
        
        private QuestMakeData _questMakeData;
        
        private static Texture2D _backgroundTexture;
        public static Texture2D BackgroundTexture {
            get {
                if (_backgroundTexture == null ) {
                    _backgroundTexture = new Texture2D ( 1 , 1 );
                    _backgroundTexture.SetPixel ( 0 , 0 , new Color32 ( 222 , 222 , 222 , 255 ));
                }
                return _backgroundTexture;
            }
        }
        
        private ESection _selectedSection = ESection.Show;
        private GUIStyle guiStyle = new GUIStyle(); //create a new variable
        
        
        internal void OnEnable()
        {
            if (null == _questMakeData)
            {
                _questMakeData = new QuestMakeData();
            }
            
        }

        
        
        internal void OnGUI(Rect position)
        {
            float sectionAreaWidth = (position.width - 60) * 0.08f;
            float viewAreaWidth = (position.width - 60) - sectionAreaWidth;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                
                DrawSectionBox(sectionAreaWidth);
                
                GUILayout.Space(20);

                switch (_selectedSection)
                {
                    case ESection.Show:
                        break;
                    case ESection.Edit:
                    default:
                        DrawEditArea(viewAreaWidth);
                        break;
                }
                
                GUILayout.Space(20);
            }
            GUILayout.EndHorizontal();
        }
        
        void DrawSectionBox(float width)
        {
            GUILayout.BeginVertical(GUILayout.Width(width));
            {
                _selectedSection = (ESection)GUILayout.SelectionGrid((int)_selectedSection, Enum.GetNames(typeof(ESection)), 1, GUILayout.Height(100));
            }
            GUILayout.EndVertical();
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
