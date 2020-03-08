using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace QuestSystem
{
    
    public class TestWindow : EditorWindow
    {
    
        private void OnEnable()
        {

        }
        
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/TestWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            TestWindow window = (TestWindow)EditorWindow.GetWindow(typeof(TestWindow));
            window.Show();
        }

        void OnGUI()
        {
            
            
        }
    
    
    }
}