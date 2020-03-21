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

    public override void OnSwitch(bool isOn)
    {
        if (isOn)
        {
            _meshRenderer.material.color = _color;
        }
        else
        {
            _meshRenderer.material.color = Color.black;
        }
    }
}
