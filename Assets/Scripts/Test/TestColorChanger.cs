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

    protected override void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        base.Start();
    }

    public override void OnSwitch(bool isOn)
    {
        _meshRenderer.material.color = isOn ? _color : Color.black;
    }
}
