using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SQLite4Unity3d;
using UnityEngine;
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
                Debug.LogError(e.StackTrace);
                return false;
            }

            return true;
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public QuestData CreateQuestData(QuestData questData)
        {
            return CreateData(questData);
        }
        
        public QuestData UpdateQuestData(QuestData questData)
        {
            return UpdateData(questData);
        }

        public QuestData GetQuestData(string questId)
        {
            return GetDatas<QuestData>().Where((questData) => questData.QuestId == questId).FirstOrDefault();
        }
        
        public IEnumerable<QuestData> GetAllQuestDatas()
        {
            return GetDatas<QuestData>();
        }


#region SwitchData
        
        //
        // SwitchDescriptionData
        //
        public SwitchDescriptionData CreateSwitchDescriptionData(SwitchDescriptionData descriptionData)
        {
            return CreateData(descriptionData);
        }
        
        public SwitchDescriptionData UpdateSwitchDescriptionData(SwitchDescriptionData descriptionData)
        {
            return UpdateData(descriptionData);
        }
        
        public SwitchDescriptionData GetSwitchDescriptionData(string switchId)
        {
            return GetDatas<SwitchDescriptionData>().Where((descriptionData) => descriptionData.SwitchId == switchId).FirstOrDefault();
        }
        
        public IEnumerable<SwitchDescriptionData> GetAllSwitchDescriptionDatas()
        {
            return GetDatas<SwitchDescriptionData>();
        }


        private const string KQueryGetSearchSwitchDescriptionDatas = @"SELECT * FROM SwitchDescriptionData 
                WHERE SwitchId IN 
                    (SELECT DISTINCT SwitchId FROM SwitchComponentData 
                        WHERE QuestId LIKE '%{0}%' 
                        OR SwitchId LIKE '%{0}%')";

        public IEnumerable<SwitchDescriptionData> GetSearchSwitchDescriptionDatas(string searchText)
        {
            return _connection.Query<SwitchDescriptionData>(string.Format(KQueryGetSearchSwitchDescriptionDatas, searchText));
        }
        
        
        //
        // SwitchStateResultData
        //
        public SwitchStateResultData CreateSwitchStateResultData(SwitchStateResultData stateResultData)
        {
            return CreateData(stateResultData);
        }
        
        public SwitchStateResultData UpdateSwitchStateResultData(SwitchStateResultData stateResultData)
        {
            return UpdateData(stateResultData);
        }
        
        public IEnumerable<SwitchStateResultData> GetSwitchStateResultData(string switchId)
        {
            return GetDatas<SwitchStateResultData>().Where((stateResultData) => stateResultData.SwitchId == switchId)
                .OrderBy((switchData) => switchData.State);
        }
        
        public IEnumerable<SwitchStateResultData> GetAllSwitchStateResultDatas()
        {
            return GetDatas<SwitchStateResultData>();
        }
        
        
        //
        // SwitchComponentData
        //
        public SwitchComponentData CreateSwitchComponentData(SwitchComponentData stateResultData)
        {
            return CreateData(stateResultData);
        }
        
        public SwitchComponentData UpdateSwitchComponentData(SwitchComponentData stateResultData)
        {
            return UpdateData(stateResultData);
        }
        
        public IEnumerable<SwitchComponentData> GetSwitchComponentDatas(string switchId)
        {
            return GetDatas<SwitchComponentData>().Where((switchData) => switchData.SwitchId == switchId)
                .OrderBy((switchData) => switchData.State);
        }
        
        public IEnumerable<SwitchComponentData> GetAllSwitchComponentDatas()
        {
            return GetDatas<SwitchComponentData>();
        }
        
#endregion
        


        T CreateData<T>(T t)
        {
            _connection.Insert(t);
            return t;
        }
        
        T UpdateData<T>(T t)
        {
            _connection.RunInTransaction(() =>
            {
                _connection.Update(t);
            });
            return t;
        }

        IEnumerable<T> GetDatas<T>() where T : new()
        {
            return _connection.Table<T>();
        }
        
        
    }

}