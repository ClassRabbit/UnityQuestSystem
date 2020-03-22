using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Text;
using SQLite4Unity3d;
using UnityEngine;


namespace QuestSystem
{
    public class SQLiteManager
    {
        private SQLiteConnection _connection;
        
        public static SQLiteManager Instance { get; }

        public bool IsConnected
        {
            get => null != _connection;
        }
        
        
        
        static SQLiteManager()
        {
            Instance = new SQLiteManager();
        }
        private SQLiteManager()
        {
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
        
        T DeleteData<T>(T t)
        {
            _connection.RunInTransaction(() =>
            {
                _connection.Delete(t);
            });
            return t;
        }

        IEnumerable<T> GetDatas<T>() where T : new()
        {
            return _connection.Table<T>();
        }
        


        

        #region QuestData

        public QuestData CreateQuestData(QuestData questData)
        {
            return CreateData(questData);
        }
        
        public QuestData UpdateQuestData(QuestData questData)
        {
            return UpdateData(questData);
        }
        
        public QuestData DeleteQuestData(QuestData questData)
        {
            return DeleteData(questData);
        }

        
        private const string QueryGetQuestData = @"SELECT * FROM QuestData 
                WHERE QuestId = '{0}'";
        public QuestData GetQuestData(string questId)
        {
            var questDataList = _connection.Query<QuestData>(string.Format(QueryGetQuestData, questId));
            return questDataList.Count == 1 ? questDataList[0] : null;
        }
        
        private const string QueryGetAllQuestDataList = @"SELECT * FROM QuestData 
                ORDER BY QuestId";
        public List<QuestData> GetAllQuestDataList()
        {
            return _connection.Query<QuestData>(QueryGetAllQuestDataList);
        }
        

        #endregion
        


        #region SwitchDescriptionData
        
        public SwitchDescriptionData CreateSwitchDescriptionData(SwitchDescriptionData descriptionData)
        {
            return CreateData(descriptionData);
        }
        
        public SwitchDescriptionData UpdateSwitchDescriptionData(SwitchDescriptionData descriptionData)
        {
            return UpdateData(descriptionData);
        }
        
        public SwitchDescriptionData DeleteSwitchDescriptionData(SwitchDescriptionData descriptionData)
        {
            return DeleteData(descriptionData);
        }
        
        private const string QueryDeleteSwitchDescriptionDataBySwitchId = @"Delete FROM SwitchDescriptionData 
                WHERE SwitchId = '{0}'";
        public List<SwitchDescriptionData> DeleteSwitchDescriptionDataBySwitchId(string switchId)
        {
            return _connection.Query<SwitchDescriptionData>(string.Format(QueryDeleteSwitchDescriptionDataBySwitchId, switchId));
        }
        
        private const string QueryGetSwitchDescriptionData = @"SELECT * FROM SwitchDescriptionData 
                WHERE SwitchId = '{0}'";
        public SwitchDescriptionData GetSwitchDescriptionData(string switchId)
        {
            var descriptionDataList = _connection.Query<SwitchDescriptionData>(string.Format(QueryGetSwitchDescriptionData, switchId));
            return descriptionDataList.Count == 1 ? descriptionDataList[0] : null;
        }
        
        private const string QueryGetAllSwitchDescriptionDataList = @"SELECT * FROM SwitchDescriptionData 
                ORDER BY SwitchId";
        public List<SwitchDescriptionData> GetAllSwitchDescriptionDataList()
        {
            return _connection.Query<SwitchDescriptionData>(QueryGetAllSwitchDescriptionDataList);
        }


        private const string QueryGetSearchSwitchDescriptionDataList = @"SELECT * FROM SwitchDescriptionData 
                WHERE SwitchId IN 
                    (SELECT DISTINCT SwitchId FROM SwitchComponentData 
                        WHERE QuestId LIKE '%{0}%' 
                        OR SwitchId LIKE '%{0}%')";
        public List<SwitchDescriptionData> GetSearchSwitchDescriptionDataList(string searchText)
        {
            return _connection.Query<SwitchDescriptionData>(string.Format(QueryGetSearchSwitchDescriptionDataList, searchText));
        }
        
        #endregion
        
        
        
        #region SwitchStateResultData
        
        public SwitchStateResultData CreateSwitchStateResultData(SwitchStateResultData stateResultData)
        {
            return CreateData(stateResultData);
        }
        
        public SwitchStateResultData UpdateSwitchStateResultData(SwitchStateResultData stateResultData)
        {
            return UpdateData(stateResultData);
        }
        
        public SwitchStateResultData DeleteSwitchStateResultData(SwitchStateResultData stateResultData)
        {
            return DeleteData(stateResultData);
        }
        
        private const string QueryDeleteSwitchStateResultDataBySwitchId = @"Delete FROM SwitchStateResultData 
                WHERE SwitchId = '{0}'";
        public List<SwitchStateResultData> DeleteSwitchStateResultDataBySwitchId(string switchId)
        {
            return _connection.Query<SwitchStateResultData>(string.Format(QueryDeleteSwitchStateResultDataBySwitchId, switchId));
        }
        
        private const string QueryGetSwitchStateResultData = @"SELECT * FROM SwitchStateResultData
            WHERE SwitchId = '{0}'
            ORDER BY State";
        public List<SwitchStateResultData> GetSwitchStateResultDataList(string switchId)
        {
            return _connection.Query<SwitchStateResultData>(string.Format(QueryGetSwitchStateResultData, switchId));
        }
        
        private const string QueryGetAllSwitchStateResultDataList = @"SELECT * FROM SwitchStateResultData";
        public List<SwitchStateResultData> GetAllSwitchStateResultDataList()
        {
            return _connection.Query<SwitchStateResultData>(QueryGetAllSwitchStateResultDataList);
        }
        
        
        
        #endregion

        
        
        #region SwitchComponentData

        
        public SwitchComponentData CreateSwitchComponentData(SwitchComponentData stateResultData)
        {
            return CreateData(stateResultData);
        }
        
        public SwitchComponentData UpdateSwitchComponentData(SwitchComponentData stateResultData)
        {
            return UpdateData(stateResultData);
        }
        
        public SwitchComponentData DeleteSwitchComponentData(SwitchComponentData stateResultData)
        {
            return DeleteData(stateResultData);
        }
        
        private const string QueryDeleteSwitchComponentDataBySwitchId = @"Delete FROM SwitchComponentData 
                WHERE SwitchId = '{0}'";
        public List<SwitchComponentData> DeleteSwitchComponentDataBySwitchId(string switchId)
        {
            return _connection.Query<SwitchComponentData>(string.Format(QueryDeleteSwitchComponentDataBySwitchId, switchId));
        }
        
        private const string QueryGetSwitchComponentDataList = @"SELECT * FROM SwitchComponentData 
                WHERE SwitchId = '{0}'
                ORDER BY State";
        public List<SwitchComponentData> GetSwitchComponentDataList(string switchId)
        {
            return _connection.Query<SwitchComponentData>(string.Format(QueryGetSwitchComponentDataList, switchId));
        }
        
        private const string QueryGetAllSwitchComponentDataList = @"SELECT * FROM SwitchComponentData";
        public List<SwitchComponentData> GetAllSwitchComponentDataList()
        {
            return _connection.Query<SwitchComponentData>(QueryGetAllSwitchComponentDataList);
        }
        
        
        private const string QueryGetSwitchDescriptionDataListByQuestId = @"SELECT DISTINCT SwitchId FROM SwitchComponentData 
            WHERE QuestId IN ({0})";
        public List<SwitchDescriptionData> GetSwitchDescriptionDataListByQuestId(List<string> questIdList)
        {
            if (questIdList == null || 0 == questIdList.Count)
            {
                return new List<SwitchDescriptionData>();
            }
            
            var stringBuilder = new StringBuilder($"'{questIdList[0]}'");
            for (int questIdListIdx = 1; questIdListIdx < questIdList.Count; ++questIdListIdx)
            {
                stringBuilder.Append($", '{questIdList[questIdListIdx]}'");
            }
            
            return _connection.Query<SwitchDescriptionData>(string.Format(QueryGetSwitchDescriptionDataListByQuestId, stringBuilder.ToString()));
        }
        
        #endregion
        

        
    }

}