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
    
    
    
    
    Dictionary<string, SwitchData> _switchDataDic = new Dictionary<string, SwitchData>();
    HashSet<string> _clearedQuestSet = new HashSet<string>();
    
    
    List<string> _clearedQuestIdList = new List<string>();

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

    /// <summary>
    ///   <para>이번 프래임에 해결된 QuestId들을 캐시해두다 한번에 조회해서 스위치를 업데이트한다.</para>
    /// </summary>
    private void Update()
    {
        if (_clearedQuestIdList.Count != 0)
        {
            foreach (var questId in _clearedQuestIdList)
            {
                ClearQuest(questId);
            }

            var componentDataList = SQLiteManager.Instance.GetSwitchComponentDataListByQuestId(_clearedQuestIdList);

            foreach (var componentData  in componentDataList)
            {
                UpdateSwitch(componentData.SwitchId);
            }
            
            
            _clearedQuestIdList.Clear();
        }
    }

    public void ClearQuest(string questId)
    {
        if (!_clearedQuestSet.Contains(questId))
        {
            _clearedQuestSet.Add(questId);
        }
    }

    
    /// <summary>
    ///   <para>프레임 도중 강제로 스위치를 갱신하는 함수.</para>
    /// </summary>
    public void ForcedUpdateSwitchFrame()
    {
        Update();
    }

    
    /// <summary>
    ///     <para>
    ///        씬 로딩시 현재 씬에 대한 SwitchController를 파악해서 Dictionary화 해둔다
    ///     </para>
    /// </summary>
    IEnumerator SetSwitchDic()
    {
        _switchDataDic.Clear();

        var switchControllers = FindObjectsOfType<SwitchController>();
        foreach (var switchController in switchControllers)
        {
            var switchId = switchController.SwitchId;
            if (!_switchDataDic.ContainsKey(switchId))
            {
                //스위치 데이터 생성중 
                var switchData = SwitchData.CreateSwitchData(switchId);
                if (switchData == null)
                {
                    continue;
                }

                switchData.Action = switchController.OnSwitch;
                _switchDataDic.Add(switchController.SwitchId, switchData);
            }
            else
            {
                _switchDataDic[switchController.SwitchId].Action += switchController.OnSwitch;
            }

            yield return null;
        }
        
    }
    

    void UpdateSwitch(string switchId)
    {
        SwitchData switchData = null;
        if (_switchDataDic.TryGetValue(switchId, out switchData))
        {
            switchData.Action(switchData.GetResult(_clearedQuestSet));
        }
    }
}
