using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // SwitchController 모두 가져와서 스위치 이벤트 딕셔너리 생성
    
    // ClearQuest 퀘스트클리어된 id를 id리스트에 넣는다
    
    // id리스트가 비어있지 않는다면
    // Update 시 ready한 id리스트를 모두 클리어시키고 씬을 업데이트한다
    // 리스트내 id들을 가진 스위치를 db에서 검색해서 그 스위치id값을 받아온다
    // 그 스위치들의 result를 계산하고 이벤트 딕셔너리에 result로 호출한다
    
    // forcedupdate 강제로 현재 업데이트
    
    List<string> _clearedQuestIdList = new List<string>();
    Dictionary<string, Action<bool>> _switchActionDic = new Dictionary<string, Action<bool>>();
    HashSet<string> _clearedQuestSet = new HashSet<string>();

    private QuestManager()
    {
        
    }
    
    //정적 생성자를 이용한 싱클톤 생성
    public static QuestManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    private void Update()
    {
        if (_clearedQuestIdList.Count != 0)
        {
            foreach (var questId in _clearedQuestIdList)
            {
                if (!_clearedQuestSet.Contains(questId))
                {
                    _clearedQuestSet.Add(questId);
                }
            }
            
            _clearedQuestIdList.Clear();
//            UpdateSwitch();
        }
    }

    void SetSwitchActionDic()
    {
        _switchActionDic.Clear();

        var switchControllers = FindObjectsOfType<SwitchController>();
        foreach (var switchController in switchControllers)
        {
            if (!_switchActionDic.ContainsKey(switchController.SwitchId))
            {
                _switchActionDic.Add(switchController.SwitchId, switchController.OnSwitch);
            }
            else
            {
                _switchActionDic[switchController.SwitchId] += switchController.OnSwitch;
            }
        }
        
    }
    

    void UpdateAllSwitch()
    {
        
    }
}
