using UnityEngine;
using UnityEditor;
using System;

namespace QuestSystem
{
    /// <summary>
    ///   <para>QuestId, SwitchId 생성 창의 추상 클래스</para>
    /// </summary>
    public abstract class QuestSystemEditWindow : QuestSystemWindow
    {
        #region Const

        protected const string ConfirmTextValue = "확인";
        protected const string DescriptionTextValue = "설명";
        protected const string CreateTextValue = "생성";
        protected const string UpdateTextValue = "수정";
        protected const string DeleteTextValue = "삭제";
        
        protected const string CreateSuccessTextValue = "생성되었습니다.";
        protected const string CreateFailTextValue = "생성을 실패하였습니다.";
        protected const string UpdateSuccessTextValue = "수정되었습니다.";
        protected const string UpdateFailTextValue = "수정을 실패하였습니다.";
        protected const string DeleteSuccessTextValue = "삭제되었습니다.";
        protected const string DeleteFaileTextValue = "삭제를 실패하였습니다.";

        #endregion

        #region Variable

        // 확인창 크기
        private Rect _confirmWindowRect = new Rect(0, 0, 300, 220);
        protected Rect ConfirmWindowRect => _confirmWindowRect;

        protected string ConfirmWindowNoticeText { get; set; } = string.Empty;
        protected Action ConfirmWindowAction { get; set; } = null;

        #endregion


        #region ConfirmWindow

        /// <summary>
        ///     확인 창 그리기
        /// </summary>
        protected void DrawConfirmWindowContent(int unusedWindowID)
        {
            ConfirmWindowProcess();
            
            GUILayout.Space(80);

            var beforeWordAlignment = EditorStyles.label.alignment;
            EditorStyles.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(ConfirmWindowNoticeText, EditorStyles.label);
            EditorStyles.label.alignment = beforeWordAlignment;
            GUILayout.Space(80);
            
            if (GUILayout.Button(ConfirmTextValue))
            {
                ConfirmWindowAction?.Invoke();
            }
        }

        /// <summary>
        ///     확인 창 화면 그리기
        /// </summary>
        protected void DrawConfirmWindow(string windowName)
        {
            Rect rect = new Rect(0, 0, position.width, position.height);
            EditorGUI.DrawRect(rect, new Color32(0, 0, 0, 122));
            BeginWindows();
            _confirmWindowRect.x = (position.width - _confirmWindowRect.width) / 2;
            _confirmWindowRect.y = (position.height - _confirmWindowRect.height) / 2;
            _confirmWindowRect = GUILayout.Window(1, _confirmWindowRect, DrawConfirmWindowContent, windowName);
            EndWindows();
        }

        #endregion


        #region AbstractFunction

        /// <summary>
        ///     확인창 구성하는 행동
        /// </summary>
        protected abstract void ConfirmWindowProcess();

        /// <summary>
        ///     창을 초기화하는 행동
        /// </summary>
        protected abstract void ResetEditor();
        
        #endregion

    }
}