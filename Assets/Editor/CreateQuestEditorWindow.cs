//using System;
//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Text;
//
//namespace QuestSystem
//{
//    
//    public class CreateQuestEditorWindow : EditorWindow
//    {
//
//        class CreateQuesteData
//        {
//            public string QuestId { get; private set; } = string.Empty;
//            public string Description { get; private set; } = string.Empty;
//        }
//    
//        private void OnEnable()
//        {
//
//        } 
//    
//    
//    
//        // Add menu named "My Window" to the Window menu
//        [MenuItem("Window/CreateQuestEditorWindow")]
//        static void Init()
//        {
//            // Get existing open window or if none, make a new one:
//            CreateQuestEditorWindow window = (CreateQuestEditorWindow)EditorWindow.GetWindow(typeof(CreateQuestEditorWindow));
//            window.Show();
//        }
//
//        void OnGUI()
//        {
////            GUILayout.BeginVertical();
////            {
////                GUILayout.Label("QuestData 생성");
////                //아이디 입력
////                GUILayout.BeginHorizontal();
////                {
////                    GUILayout.Label("QuestId ", EditorStyles.boldLabel, GUILayout.Width(70));
////                    _questMakeData.QuestId = GUILayout.TextArea(_questMakeData.QuestId, GUILayout.MaxWidth(width - 70));
////                }
////                GUILayout.EndHorizontal();
////                
////                GUILayout.Space(10);
////                //설명입력
////                GUILayout.Label("Description", EditorStyles.boldLabel);
////                _questMakeData.Description = GUILayout.TextArea(_questMakeData.Description, GUILayout.MaxWidth(width), GUILayout.Height(300));
////            }
////            GUILayout.EndVertical();
//        }
//    
//    
//    }
//}
