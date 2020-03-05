using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;
using SQLite4Unity3d;
using Object = UnityEngine.Object;

namespace QuestSystem
{
    public class SQLiteManager
    {
        private SQLiteManager()
        {
        }

        private static volatile SQLiteManager _instance = null;
        private static object syncRoot = new Object();

        private SQLiteConnection _connection;
        
        public static SQLiteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new SQLiteManager();
                        }
                    }
                }

                return _instance;
            }
        }

        public bool IsConnected
        {
            get => null != _connection;
        }


        public bool Connect(string databaseName)
        {
            if (null != _connection)
            {
                _connection.Close();
            }

            try
            {
#if UNITY_EDITOR
                var dbPath = $@"Assets/StreamingAssets/{databaseName}";
#else
                // check if file exists in Application.persistentDataPath
                var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

                if (!File.Exists(filepath))
                {
                    Debug.Log("Database not in Persistent path");
                    // if it doesn't ->
                    // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
                    var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
                    while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                    // then save to Application.persistentDataPath
                    File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                     var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
#elif UNITY_WP8
                    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		            var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		            // then save to Application.persistentDataPath
		            File.Copy(loadDb, filepath);
#elif UNITY_STANDALONE_OSX
		            var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		            // then save to Application.persistentDataPath
		            File.Copy(loadDb, filepath);
#else
	                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	                // then save to Application.persistentDataPath
	                File.Copy(loadDb, filepath);
#endif

                }

                var dbPath = filepath;
#endif
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public QuestData CreateQuestData(string questId, string description)
        {
            var questData = new QuestData
            {
                QuestId = questId,
                Description = description
            };

            _connection.Insert(questData);
            return questData;
        }

        public QuestData GetQuestData(string questId)
        {
            return _connection.Table<QuestData>().Where(questData => questData.QuestId == questId).FirstOrDefault();
        }
        
        public IEnumerable<QuestData> GetAllQuestDatas()
        {
            return _connection.Table<QuestData>();
        }
        
//        public SwitchData? GetSwitchData(string switchId)
//        {
//            return _connection.Table<SwitchData>().Where(switchId => switchId.SwitchId == switchId);
//        }
//        
//        public IEnumerable<QuestData> GetAllSwitchDatas()
//        {
//            return _connection.Table<SwitchData>();
//        }
        
    }

}