using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitHover : MonoBehaviour
{
    public static MouseUnitHover Instance;

    public event EventHandler OnHoveredUnitChange;

    [SerializeField] private LayerMask m_UnitLayerMask;

    private MouseWorld m_MouseWorld;

    private Unit m_HoveredUnit = null;
    public Unit GetHoveredUnit() { return m_HoveredUnit; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of MouseUnitHover singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        Unit l_CurrentUnit = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, m_UnitLayerMask))
            l_CurrentUnit = hit.transform.GetComponent<Unit>();

        if (l_CurrentUnit != m_HoveredUnit)
        {
            m_HoveredUnit = l_CurrentUnit;
            OnHoveredUnitChange?.Invoke(this, EventArgs.Empty);
        }        
    }
}
