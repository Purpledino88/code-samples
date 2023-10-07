using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    //Singleton code
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChange;
    public event EventHandler OnSelectedActionChange;
    public event EventHandler<bool> OnActiveChange;

    [SerializeField] private LayerMask m_UnitLayerMask;

    private Unit m_SelectedUnit;
    public Unit GetSelectedUnit() { return m_SelectedUnit; }

    private BasicAction m_SelectedAction;
    public BasicAction GetSelectedAction() { return m_SelectedAction; }

    private bool m_IsActive = false;

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of UnitActionSystem singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {        
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
    }

    private void ActivateAction() 
    { 
        m_IsActive = true; 
        OnActiveChange?.Invoke(this, m_IsActive);
    }

    private void DeactivateAction() 
    { 
        m_IsActive = false; 
        SetSelectedAction(null);
        OnActiveChange?.Invoke(this, m_IsActive);
    }

    public void EndAIAction()
    {
        OnActiveChange?.Invoke(this, m_IsActive);
    }

    private void Update() 
    {
        if ((!m_IsActive) && (TurnSystem.Instance.GetActiveForce() == Unit.eUnitForce.UNIT_PLAYER))
        {            
            HandleCancellation();

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_SelectedAction == null)
                HandleUnitSelection();
            else
                HandleActionSelection();
        }
    }

    private void HandleCancellation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (m_SelectedAction != null)
                SetSelectedAction(null);
            else if (m_SelectedUnit != null)
                SetSelectedUnit(null);
        }
    }

    private void HandleUnitSelection()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, m_UnitLayerMask))
            {
                Unit unit = hit.transform.GetComponent<Unit>();
                if (unit.GetUnitForce() == Unit.eUnitForce.UNIT_PLAYER)
                    SetSelectedUnit(unit);
                else SetSelectedUnit(null);
            }
            else
            {
                SetSelectedUnit(null);
            }
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition l_MouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (m_SelectedAction.IsValidGridPosition(l_MouseGridPosition))
            {
                if (m_SelectedUnit.TryTakeAction(m_SelectedAction))
                {
                    m_SelectedAction.TakeAction(DeactivateAction, l_MouseGridPosition);
                    ActivateAction();
                }                
            }
        }
    }

    private void SetSelectedUnit(Unit selected_unit)
    {
        m_SelectedUnit = selected_unit;
        SetSelectedAction(null);
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BasicAction selected_action)
    {
        m_SelectedAction = selected_action;
        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }

    public void DeselectSelectedAction()
    {
        SetSelectedAction(null);
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        SetSelectedAction(null);
        SetSelectedUnit(null);
    }
}
