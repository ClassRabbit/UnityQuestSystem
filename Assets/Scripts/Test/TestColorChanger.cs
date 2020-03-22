using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

public class TestColorChanger : SwitchController
{
    [Header("ColorChanger"), SerializeField] 
    private Color _color = Color.black;

    private MeshRenderer _meshRenderer = null;

    private void Start()
    {
        QuestManager.Instance.RegisterSwitch(this);
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDestroy()
    {
        QuestManager.Instance.CancelRegisterSwitch(this);
    }

    public override void OnSwitch(bool isOn)
    {
        _meshRenderer.material.color = isOn ? _color : Color.black;
    }
}
