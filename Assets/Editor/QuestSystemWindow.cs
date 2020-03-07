using System;
using UnityEngine;
using UnityEditor;

namespace QuestSystem
{
    
    public abstract class QuestSystemWindow : EditorWindow
    {
        protected string DatabaseName { get; private set; }

        protected void OnEnable()
        {
            EnableProcess();
        }

        protected virtual void EnableProcess()
        {
        }

        protected virtual void RefreshProcess()
        {
        }


        private void OnGUI()
        {
            if (!SQLiteManager.Instance.IsConnected)
            {
                Debug.LogError("DB is Not Connect");
                return;
            }

            GUIProcess();
        }
        
        
        protected virtual void GUIProcess()
        {
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
        
        protected virtual void FocusProcess()
        {
        }
    }
}


