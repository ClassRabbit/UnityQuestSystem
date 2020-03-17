using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

namespace QuestSystem
{
    public abstract class SwitchController : MonoBehaviour
    {

        [SerializeField] 
        public string _switchId;

        public string SwitchId => _switchId;

        public abstract void OnSwitch(bool isOn);
    }
}

