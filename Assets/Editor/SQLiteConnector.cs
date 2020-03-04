//using SQLite4Unity3d;
//using UnityEngine;
//using System.IO;
//using System.Collections.Generic;
//
//namespace QuestSystem
//{
//	public class SQLiteConnectorPactory
//	{
//		public static SQLiteConnector MakeSQLiteConnector(string databaseName)
//		{
//			var sqliteConnector = new SQLiteConnector(databaseName);
//			if (sqliteConnector.IsConnected)
//			{
//				return sqliteConnector;
//			}
//			else
//			{
//				return null;
//			}
//		}
//	}
//	
//	public class SQLiteConnector
//    {
//	    public bool IsConnected
//	    {
//		    get { return _connection != null; }
//	    }
//	    private SQLiteConnection _connection;
//
//	    public SQLiteConnector(string databaseName)
//	    {
//		    var directoryPath = string.Format(@"Assets/StreamingAssets/");
//		    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
//		    if (!directoryInfo.Exists)
//		    {
//			    Directory.CreateDirectory(directoryPath);
//		    }
//		    var dbPath = string.Format(directoryPath + $"{databaseName}");
//		    _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//		    Debug.Log("Final PATH: " + dbPath);
//		    
//		    _connection.CreateTable<QuestData>();
////		    _connection.CreateTable<SwitchData>();
//	    }
//    }
//
//
//
////
////	public void CreateDB(){
////		_connection.DropTable<Person> ();
////		_connection.CreateTable<Person> ();
////
////		_connection.InsertAll (new[]{
////			new Person{
////				Id = 1,
////				Name = "Tom",
////				Surname = "Perez",
////				Age = 56
////			},
////			new Person{
////				Id = 2,
////				Name = "Fred",
////				Surname = "Arthurson",
////				Age = 16
////			},
////			new Person{
////				Id = 3,
////				Name = "John",
////				Surname = "Doe",
////				Age = 25
////			},
////			new Person{
////				Id = 4,
////				Name = "Roberto",
////				Surname = "Huertas",
////				Age = 37
////			}
////		});
////	}
////
////	public IEnumerable<Person> GetPersons(){
////		return _connection.Table<Person>();
////	}
////
////	public IEnumerable<Person> GetPersonsNamedRoberto(){
////		return _connection.Table<Person>().Where(x => x.Name == "Roberto");
////	}
////
////	public Person GetJohnny(){
////		return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
////	}
////
////	public Person CreatePerson(){
////		var p = new Person{
////				Name = "Johnny",
////				Surname = "Mnemonic",
////				Age = 21
////		};
////		_connection.Insert (p);
////		return p;
////	}
//
//}
