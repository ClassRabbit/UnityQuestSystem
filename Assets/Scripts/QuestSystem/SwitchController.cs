using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

namespace QuestSystem
{
    public abstract class SwitchController : MonoBehaviour
    {
        public bool IsRegistered { get; set; } = false;

        [Header("SwitchController"), SerializeField] 
        private string _switchId = string.Empty;
        public string SwitchId => _switchId;
        
        /// <summary>
        ///   <para>QuestManager에 구독</para>
        /// </summary>
        protected virtual void Start()
        {
            QuestManager.Instance.RegisterSwitch(this);
        }

        /// <summary>
        ///   <para>QuestManager에 구독 취소</para>
        /// </summary>
        protected virtual void OnDestroy()
        {
            QuestManager.Instance.CancelRegisterSwitch(this);
        }

        /// <summary>
        ///   <para>스위치의 상태가 변화된다면 실행된다</para>
        /// </summary>
        public abstract void OnSwitch(bool isOn);
    }
}

