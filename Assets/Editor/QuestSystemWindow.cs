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
            
            if (isRefresh)
            {
                //유니티에디터에 저장된 디비이름을 가져온다
                if (!SQLiteManager.Instance.IsConnected)
                {
                    SQLiteManager.Instance.Connect(PreferencesWindow.QuestSystemDatabaseName);
                }

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
        
        


    }
}


