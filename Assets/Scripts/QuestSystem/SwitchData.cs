using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using System;
using UnityEngine;

public class SwitchData
{
    public string SwitchId { get; private set; }
    
    public List<List<SwitchComponentData>> StateList { get; private set; }

    public List<SwitchStateResultData> ResultList { get; private set; }

    public Action<bool> Action { get; set; }

    private SwitchData(string switchId, List<List<SwitchComponentData>> stateList,
        List<SwitchStateResultData> resultList)
    {
        SwitchId = switchId;
        StateList = stateList;
        ResultList = resultList;
    }

//    public bool GetResult(HashSet<string> clearedQuestSet)
//    {
//        int result = false;
//        for (int i = 0; i < StateList.Count; ++i)
//        {
//            
//        }
//    }


    public static SwitchData CreateSwitchData(string switchId)
    {
        try
        {
            var componentDataList = SQLiteManager.Instance.GetSwitchComponentDataList(switchId);
            if (0 == componentDataList.Count)
            {
                return null;
            }
        
            var resultList = SQLiteManager.Instance.GetSwitchStateResultDataList(switchId);
            if (0 == resultList.Count)
            {
                return null;
            }

            var stateList = new List<List<SwitchComponentData>>(componentDataList[componentDataList.Count - 1].State + 1);
            if (resultList.Count != stateList.Count)
            {
                return null;
            }
            
            foreach (var componentData in componentDataList)
            {
                if (null == stateList[componentData.State])
                {
                    stateList[componentData.State] = new List<SwitchComponentData>();
                }
                stateList[componentData.State].Add(componentData);
            }
            
        
            var switchData = new SwitchData(switchId, stateList, resultList);

            return switchData;
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
            return null;
        }
        
        
        
    }
}
