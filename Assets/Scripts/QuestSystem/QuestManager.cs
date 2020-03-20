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
    private List<string> _requireUpdatingSwitchIdList = new List<string>();

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
        //이번 프래임에 클리어된 questId대기열 목록을 ClearSet에 등록
        if (0 != _clearedQuestIdList.Count)
        {
            foreach (var questId in _clearedQuestIdList)
            {
                ClearQuest(questId);
            }
            
            //클리어된 questId와 관련된 SwitchDescriptionData를 가져와서 업데이트 대기열에 등록
            var descriptionDataList = SQLiteManager.Instance.GetSwitchDescriptionDataListByQuestId(_clearedQuestIdList);
            foreach (var descriptionData  in descriptionDataList)
            {
                AddRequireUpdatingSwitchIdList(descriptionData.SwitchId);
            }
            //questId 대기열
            _clearedQuestIdList.Clear();
        }

        //Switch업데이트 대기열에 등록된 스위치들을 업데이트
        if (0 != _requireUpdatingSwitchIdList.Count)
        {
            foreach (var switchId in _requireUpdatingSwitchIdList)
            {
                UpdateSwitch(switchId);
            }
            _requireUpdatingSwitchIdList.Clear();
        }
    }

    /// <summary>
    ///   <para>해당 QuestId를 해결함. 이번 프래임 이후 해당 QuestId와 관련된 스위치 업데이트</para>
    /// </summary>
    public void ClearQuest(string questId)
    {
        if (!_clearedQuestSet.Contains(questId))
        {
            _clearedQuestSet.Add(questId);
        }
    }

    /// <summary>
    ///   <para>이번 프래임 끝에 업데이트될 스위치를 등록</para>
    /// </summary>
    void AddRequireUpdatingSwitchIdList(string switchId)
    {
        if (!_requireUpdatingSwitchIdList.Contains(switchId))
        {
            _requireUpdatingSwitchIdList.Add(switchId);
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
    ///   <para>SwitchController를 Dictionary에 추가</para>
    /// </summary>
    public void RegisterSwitch(SwitchController switchController)
    {
        if (switchController.IsRegistered)
        {
            return;
        }

        var switchId = switchController.SwitchId;
        if (!_switchDataDic.ContainsKey(switchId))
        {
            //스위치 데이터 생성
            var switchData = SwitchData.CreateSwitchData(switchId);
            if (switchData == null)
            {
                return;
            }

            switchData.Action = switchController.OnSwitch;
            _switchDataDic.Add(switchController.SwitchId, switchData);
        }
        else
        {
            _switchDataDic[switchController.SwitchId].Action += switchController.OnSwitch;
        }

        switchController.IsRegistered = true;
        
        AddRequireUpdatingSwitchIdList(switchId);
    }
    

    void UpdateSwitch(string switchId)
    {
        if (_switchDataDic.TryGetValue(switchId, out var switchData))
        {
            switchData.Action(switchData.GetResult(_clearedQuestSet));
        }
    }
}
