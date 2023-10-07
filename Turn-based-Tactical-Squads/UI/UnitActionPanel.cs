using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject _ActionButtonPrefab;
    [SerializeField] private GameObject _ActionButtonContainer;
    [SerializeField] private ActionDetailsPanel _ActionDetailsPanel;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;

        HideActionDetails();
        ClearActionButtons();
        gameObject.SetActive(false);
    }

    private void CreateActionButtons()
    {
        ClearActionButtons();

        Unit l_SelectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (l_SelectedUnit != null)
        {
            foreach (BasicAction act in l_SelectedUnit.GetAllActions())
            {
                GameObject l_ActionButton = Instantiate(_ActionButtonPrefab, _ActionButtonContainer.transform);
                l_ActionButton.GetComponent<UnitActionButton>().SetActionType(act);
            }
        }
    }

    private void ClearActionButtons()
    {
        foreach (Transform child in _ActionButtonContainer.transform)
            Destroy(child.gameObject);
    }

    private void ShowActionDetails(BasicAction act)
    {
        _ActionDetailsPanel.UpdateDetails();
    }

    private void HideActionDetails()
    { 
        _ActionDetailsPanel.Hide();
    }

    private void UnitActionSystem_OnSelectedUnitChange(object sender, EventArgs e)
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            gameObject.SetActive(true);
            CreateActionButtons();            
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
    {
        if (UnitActionSystem.Instance.GetSelectedAction() != null)
        {
            ClearActionButtons();
            ShowActionDetails(UnitActionSystem.Instance.GetSelectedAction());
        }
        else if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            CreateActionButtons();
            HideActionDetails();
        }
    }
}
