using UnityEngine;
using UnityEditor;
using System.IO;
using SQLite4Unity3d;

namespace QuestSystem
{
    
    public class PreferencesWindow : EditorWindow
    {
        private const string KQuestSystemDatabaseNameKey = "QuestSystemDatabaseName";
        public static string QuestSystemDatabaseName
        {
            get
            {
                return EditorPrefs.GetString(KQuestSystemDatabaseNameKey, string.Empty);
            }
            set
            {
                Debug.Log($"<color=blue>QuestSystem Database : {value}</color>");
                EditorPrefs.SetString(KQuestSystemDatabaseNameKey, value);
            }
        }

        private string _inputedDatabaseName = string.Empty;
    
        // Add menu named "My Window" to the Window menu
        [MenuItem("QuestSystem/PreferencesWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            PreferencesWindow window = (PreferencesWindow)EditorWindow.GetWindow(typeof(PreferencesWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("현재 Database : " + QuestSystemDatabaseName);
            
            GUILayout.BeginHorizontal();
            {
                _inputedDatabaseName = GUILayout.TextField(_inputedDatabaseName);
                if (GUILayout.Button("Change"))
                {
                    if (string.IsNullOrEmpty(_inputedDatabaseName))
                    {
                        //에러처리
                    }
                    else
                    {
                        QuestSystemDatabaseName = _inputedDatabaseName;
                        //StreamingAssets 생성
                        var directoryPath = string.Format(@"Assets/StreamingAssets/");
                        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                        if (!directoryInfo.Exists)
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        var dbPath = string.Format(directoryPath + $"{QuestSystemDatabaseName}");
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
        }
    
    
    }
}