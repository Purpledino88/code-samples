using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit m_Unit;

    private MeshRenderer m_MeshRenderer;
    
    private void Awake() 
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() 
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UpdateVisual();
    }

    //Event subscription
    private void UnitActionSystem_OnSelectedUnitChange(object sender, EventArgs args)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == m_Unit)
            m_MeshRenderer.enabled = true;
        else m_MeshRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
    }
}
