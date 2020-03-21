using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private string _dbName = string.Empty;
    
    private void Awake()
    {
        SQLiteManager.Instance.Connect(_dbName);
    }

    private void Update()
    {
        QuestManager.Instance.Update();
    }

    private void OnGUI()
    {
        
    }
}
