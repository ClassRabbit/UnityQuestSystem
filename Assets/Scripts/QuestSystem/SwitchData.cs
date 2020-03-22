using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using System;
using UnityEngine;

public class SwitchData
{
    enum EOperator
    {
        AND,
        OR
    }
    
    public string SwitchId { get; private set; }
    public bool DefaultResult { get; private set; }
    
    public List<List<SwitchComponentData>> StateList { get; private set; }

    public List<SwitchStateResultData> ResultList { get; private set; }

    private Action<bool> _action;
    public Action<bool> Action
    {
        get => _action;
        set
        {
            _action = value;
            value(CurrentResult);
        }
    }

    public bool CurrentResult { get; set; }

    private SwitchData(SwitchDescriptionData descriptionData, List<List<SwitchComponentData>> stateList,
        List<SwitchStateResultData> resultList)
    {
        SwitchId = descriptionData.SwitchId;
        DefaultResult = descriptionData.DefaultResult;
        CurrentResult = descriptionData.DefaultResult;
        StateList = stateList;
        ResultList = resultList;
    }

    /// <summary>
    ///   <para>해결된 QuestSet을 조회해서 현재 스위치의 상태를 결정함</para>
    /// </summary>
    public void Update(Dictionary<string, bool> clearedQuestDic)
    {
        bool result = DefaultResult;
        //isOn된 마지막 상태의 결과로 설정한다. 
        for (int stateIdx = StateList.Count - 1; stateIdx >= 0; --stateIdx)
        {
            bool isOn = clearedQuestDic[StateList[stateIdx][0].QuestId];
            
            for (int stateComponentIdx = 1; stateComponentIdx < StateList[stateIdx].Count; ++stateComponentIdx)
            {
                if (StateList[stateIdx][stateComponentIdx].Operator == EOperator.AND.ToString())
                {
                    isOn = isOn & clearedQuestDic[StateList[stateIdx][stateComponentIdx].QuestId];
                }
                else
                {
                    isOn = isOn | clearedQuestDic[StateList[stateIdx][stateComponentIdx].QuestId];
                }
            }

            if (isOn)
            {
                result = ResultList[stateIdx].Result;
                break;
            }
        }

        if (CurrentResult != result)
        {
            CurrentResult = result;
            _action?.Invoke(CurrentResult);
        }
    }

    /// <summary>
    ///   <para>SwitchData 생성 팩토리 메서드, 생성 도중 오류가 생길 수 있는 점에 대한 대처방안</para>
    /// </summary>
    public static SwitchData CreateSwitchData(string switchId)
    {
        try
        {
            var descriptionData = SQLiteManager.Instance.GetSwitchDescriptionData(switchId);
            if (null == descriptionData)
            {
                Debug.LogError("description null");
                return null;
            }
            
            var componentDataList = SQLiteManager.Instance.GetSwitchComponentDataList(switchId);
            if (0 == componentDataList.Count)
            {
                Debug.LogError("componentDataList null");
                return null;
            }
        
            var resultList = SQLiteManager.Instance.GetSwitchStateResultDataList(switchId);
            if (0 == resultList.Count)
            {
                Debug.LogError("resultList null");
                return null;
            }
            
            var stateList = new List<List<SwitchComponentData>>(componentDataList[componentDataList.Count - 1].State + 1);
            for (int stateIdx = 0; stateIdx < stateList.Capacity; ++stateIdx)
            {
                stateList.Add(new List<SwitchComponentData>());
            }
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
            
        
            var switchData = new SwitchData(descriptionData, stateList, resultList);

            return switchData;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR : CreateSwitchData ({switchId})");
            Debug.LogError(e.StackTrace);
            return null;
        }
    }
    
}
