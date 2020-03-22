using System;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public abstract class QuestSystemWindow : EditorWindow
    {

        #region Variable

        /// <summary>
        ///   <para>현재 창을 구성하는 Database 이름</para>
        /// </summary>
        protected string DatabaseName { get; private set; }


        #endregion
        
        
        #region UnityEventFunction
        
        protected void OnEnable()
        {
            EnableProcess();
        }
        
        private void OnGUI()
        {
            if (!SQLiteManager.Instance.IsConnected)
            {
                Debug.LogError("SQLiteManager DB is Not Connect");
                return;
            }

            GUIProcess();
        }
        
        private void OnFocus()
        {
            string beforeDatabaseName = DatabaseName;
            DatabaseName = PreferencesWindow.QuestSystemDatabaseName;
            
            if (!SQLiteManager.Instance.IsConnected)
            {
                SQLiteManager.Instance.Connect(DatabaseName);
            }
            
            bool isRefresh = false;
            
            if (string.IsNullOrEmpty(DatabaseName))
            {
                //결정된 디비가 없음
                return;
            }
            else if (beforeDatabaseName != DatabaseName)
            {
                //디비가 바뀌었음
                isRefresh = true;
                SQLiteManager.Instance.Connect(DatabaseName);
            }
            
            if (!SQLiteManager.Instance.IsConnected)
            {
                Debug.LogError("SQLite is Not Connect");
                return;
            }
            
            if (isRefresh)
            {
                RefreshProcess();
            }
            
            FocusProcess();
        }
        
        #endregion  
        
        
        #region Virtualfunction

        /// <summary>
        ///   <para>OnEnable 되었을 시 행동</para>
        /// </summary>
        protected abstract void EnableProcess();

        /// <summary>
        ///   <para>Database 변경되었을 시 행동</para>
        /// </summary>
        protected abstract void RefreshProcess();

        /// <summary>
        ///   <para>OnGUI 되었을 시 행동</para>
        /// </summary>
        protected abstract void GUIProcess();

        /// <summary>
        ///   <para>OnFocus 되었을 시 행동</para>
        /// </summary>
        protected abstract void FocusProcess();

        #endregion
    }
    
        
}


