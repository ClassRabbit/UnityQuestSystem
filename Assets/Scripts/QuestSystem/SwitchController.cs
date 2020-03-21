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

        public abstract void OnSwitch(bool isOn);
    }
}

