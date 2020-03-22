using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using SQLite4Unity3d;

namespace QuestSystem
{
    
    public class QuestSystemPreferencesWindow : EditorWindow
    {
        private const string SettingDatabaseTextValue = "결정";
        private const string EmptyTableTextValue = "QuestSystem 관련 테이블 비우기";
        private const string QuestSystemDatabaseNameKey = "QuestSystemDatabaseName";
        
        public static string QuestSystemDatabaseName
        {
            get
            {
                return EditorPrefs.GetString(QuestSystemDatabaseNameKey, string.Empty);
            }
            set
            {
                Debug.Log($"<color=blue>QuestSystem Database : {value}</color>");
                EditorPrefs.SetString(QuestSystemDatabaseNameKey, value);
            }
        }

        
        private string _inputedDatabaseName = string.Empty;
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/PreferencesWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            QuestSystemPreferencesWindow window = (QuestSystemPreferencesWindow)EditorWindow.GetWindow(typeof(QuestSystemPreferencesWindow));
            window.Show();
        }

        void OnGUI()
        {
            
            GUILayout.Label("현재 Database : " + QuestSystemDatabaseName);
            
            GUILayout.BeginHorizontal();
            {
                _inputedDatabaseName = EditorGUILayout.TextField("Database 변경", _inputedDatabaseName);
                if (GUILayout.Button(SettingDatabaseTextValue, GUILayout.Width(80)))
                {
                    if (string.IsNullOrEmpty(_inputedDatabaseName))
                    {
                        //에러처리
                    }
                    else
                    {
                        QuestSystemDatabaseName = _inputedDatabaseName;
                        //StreamingAssets 생성
                        StringBuilder stringBuilder = new StringBuilder(Application.streamingAssetsPath);
                        DirectoryInfo directoryInfo = new DirectoryInfo(stringBuilder.ToString());
                        if (!directoryInfo.Exists)
                        {
                            Directory.CreateDirectory(stringBuilder.ToString());
                        }

                        stringBuilder.Append($"/{QuestSystemDatabaseName}");
                        var dbPath = stringBuilder.ToString();
                        var connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                        connection.CreateTable<QuestData>();
                        connection.CreateTable<SwitchDescriptionData>();
                        connection.CreateTable<SwitchComponentData>();
                        connection.CreateTable<SwitchStateResultData>();
                        connection.Close();
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button(EmptyTableTextValue))
            {
                try
                {
                    var connection = new SQLiteConnection(Application.streamingAssetsPath + $"/{QuestSystemDatabaseName}", 
                        SQLiteOpenFlags.ReadWrite);
                    connection.DeleteAll<QuestData>();
                    connection.DeleteAll<SwitchDescriptionData>();
                    connection.DeleteAll<SwitchComponentData>();
                    connection.DeleteAll<SwitchStateResultData>();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.StackTrace);
                    throw;
                }
            }
        }
    }
}