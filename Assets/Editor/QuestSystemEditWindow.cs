using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public abstract class QuestSystemEditWindow : QuestSystemWindow
    {
        protected bool IsClose { get; set; } = false;
        protected bool IsUpdate { get; set; } = false;
        private Rect _windowRect = new Rect(0, 0, 400, 300);
        protected Rect WindowRect => _windowRect;

        protected void ConfirmWindow(int unusedWindowID)
        {
            ConfirmWindowProcess();
        }

        protected virtual void ConfirmWindowProcess()
        {
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