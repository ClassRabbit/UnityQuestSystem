using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

namespace QuestSystem
{
    public class QuestManager
    {
        Dictionary<string, SwitchData> _switchDataDic = new Dictionary<string, SwitchData>();
        public Dictionary<string, SwitchData> SwitchDataDic => _switchDataDic;
        Dictionary<string, bool> _clearedQuestDic = new Dictionary<string, bool>();
        public Dictionary<string, bool> ClearedQuestDic => _clearedQuestDic;


        List<string> _clearedQuestIdList = new List<string>();
        private List<string> _requireUpdatingSwitchIdList = new List<string>();

        static QuestManager()
        {
            Instance = new QuestManager();
        }

        private QuestManager()
        {
            //퀘스트값들을 Dictionary화
            var questDataList = SQLiteManager.Instance.GetAllQuestDataList();
            foreach (var questData in questDataList)
            {
                _clearedQuestDic.Add(questData.QuestId, false);
            }
        }

        public static QuestManager Instance { get; }


        /// <summary>
        ///   <para>클리어 QuestId들을 캐시해두다 한번에 조회해서 스위치를 업데이트한다.</para>
        /// </summary>
        public void Update()
        {
            //이번 프래임에 클리어된 questId대기열 목록을 ClearSet에 등록
            if (0 != _clearedQuestIdList.Count)
            {
                //클리어된 questId와 관련된 SwitchDescriptionData를 가져와서 업데이트 대기열에 등록
                var descriptionDataList =
                    SQLiteManager.Instance.GetSwitchDescriptionDataListByQuestId(_clearedQuestIdList);
                foreach (var descriptionData in descriptionDataList)
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
        public bool ClearQuest(string questId)
        {
            if (_clearedQuestDic.ContainsKey(questId))
            {
                _clearedQuestDic[questId] = true;
                _clearedQuestIdList.Add(questId);
                return true;
            }
            else
            {
                return false;
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
        ///   <para>SwitchController를 Dictionary에 추가</para>
        /// </summary>
        public void RegisterSwitch(SwitchController switchController)
        {
            if (switchController.IsRegistered)
            {
                return;
            }

            var switchId = switchController.SwitchId;
            if (!_switchDataDic.TryGetValue(switchId, out var switchData))
            {
                //스위치 데이터 생성
                switchData = SwitchData.CreateSwitchData(switchId);
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

//            AddRequireUpdatingSwitchIdList(switchId);
            switchController.OnSwitch(switchData.CurrentResult);
        }


        public void CancelRegisterSwitch(SwitchController switchController)
        {
            if (switchController.IsRegistered)
            {
                return;
            }

            var switchId = switchController.SwitchId;
            if (_switchDataDic.ContainsKey(switchId))
            {
                _switchDataDic[switchController.SwitchId].Action -= switchController.OnSwitch;
            }

            switchController.IsRegistered = false;
        }


        void UpdateSwitch(string switchId)
        {
            if (_switchDataDic.TryGetValue(switchId, out var switchData))
            {
                switchData.Update(_clearedQuestDic);
            }
        }



        public bool IsQuestClear(string questId)
        {
            if (_clearedQuestDic.ContainsKey(questId))
            {
                return _clearedQuestDic[questId];
            }
            else
            {
                return false;
            }
        }

        public bool GetSwitchResult(string switchId)
        {
            if (_switchDataDic.TryGetValue(switchId, out var switchData))
            {
                return switchData.CurrentResult;
            }
            else
            {
                return false;
            }
        }
    }
}
