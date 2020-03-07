using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public abstract class QuestSystemEditWindow : QuestSystemWindow
    {
        protected bool IsUpdate { get; set; } = false;
        private Rect _windowRect = new Rect(0, 0, 400, 300);
        protected Rect WindowRect => _windowRect;
        protected void ConfirmWindow(int unusedWindowID)
        {
            
        }

        protected virtual void ConfirmWindowProcess()
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

        protected void DrawConfirmWindow(string windowName)
        {
            Rect rect = new Rect(0, 0, position.width, position.height);
            EditorGUI.DrawRect(rect, new Color32(0, 0, 0, 122));
            BeginWindows();
            _windowRect.x = (position.width - _windowRect.width) / 2;
            _windowRect.y = (position.height - _windowRect.height) / 2;
            _windowRect = GUILayout.Window(1, _windowRect, ConfirmWindow, windowName);
            EndWindows();
        }
    
    
    }
}