using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    /// <summary>
    ///   <para>QuestId, SwitchId 생성 창의 추상 클래스</para>
    /// </summary>
    public abstract class QuestSystemEditWindow : QuestSystemWindow
    {
        #region Const

        protected const string KConfirmText = "확인";
        protected const string KDescriptionText = "설명";
        protected const string KCreateText = "생성";
        protected const string KUpdateText = "수정";
        protected const string KDeleteText = "삭제";
        
        protected const string KCreateSuccessText = "생성되었습니다.";
        protected const string KUpdateSuccessText = "수정되었습니다.";
        protected const string KDeleteSuccessText = "삭제되었습니다.";

        #endregion
        
        #region Variable

        // 확인창 크기
        private Rect _windowRect = new Rect(0, 0, 400, 300);
        protected Rect WindowRect => _windowRect;
        
        #endregion


        #region ConfirmWindow

        protected void ConfirmWindow(int unusedWindowID)
        {
            ConfirmWindowProcess();
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

        #endregion
        
        
        #region AbstractFunction

        /// <summary>
        ///   <para>확인창 구성하는 행동</para>
        /// </summary>
        protected abstract void ConfirmWindowProcess();

        /// <summary>
        ///   <para>창을 초기화하는 행동</para>
        /// </summary>
        protected abstract void ResetEditor();
        
        #endregion
        
    
    
    }
}