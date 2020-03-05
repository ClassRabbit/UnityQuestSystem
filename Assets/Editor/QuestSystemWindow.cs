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
            if (!SQLiteManager.Instance.IsConnected)
            {
                SQLiteManager.Instance.Connect(PreferencesWindow.QuestSystemDatabaseName);
            }
            
            bool isRefresh = false;
            string beforeDatabaseName = DatabaseName;
            DatabaseName = PreferencesWindow.QuestSystemDatabaseName;
            if (string.IsNullOrEmpty(DatabaseName))
            {
                //결정된 디비가 없음
                return;
            }
            else if (beforeDatabaseName != DatabaseName)
            {
                //디비가 바뀌었음
                isRefresh = true;
            }
            
            if (!SQLiteManager.Instance.IsConnected)
            {
                Debug.LogError("DB is Not Connect");
                return;
            }
            
            if (isRefresh)
            {
                //유니티에디터에 저장된 디비이름을 가져온다
                RefreshProcess();
            }

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
            if (!SQLiteManager.Instance.IsConnected)
            {
                Debug.LogError("DB is Not Connect");
                return;
            }
            
            FocusProcess();
        }
        
        protected virtual void FocusProcess()
        {
        }
    }
}


