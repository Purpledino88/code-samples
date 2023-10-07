using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDetailsPanel : MonoBehaviour
{
    [SerializeField] private ActionPointsDisplay _ActionPointsDisplay;
    [SerializeField] private GameObject m_ElementDetailCollection;
    [SerializeField] private ElementStatusPanel m_ElementDetailsPanelPrefab;

    private void Start()
    {        
        Hide();
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitDetailsPanel_OnSelectedUnitChange;
        Unit.OnAnyUnitStatusChange += Unit_OnAnyUnitStatusChange;
        MouseUnitHover.Instance.OnHoveredUnitChange += MouseUnitHover_OnHoveredUnitChange;
    }

    private void Show(Unit target_unit, float cover_modifier)
    {
        gameObject.SetActive(true);
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            _ActionPointsDisplay.UpdateDisplay(UnitActionSystem.Instance.GetSelectedUnit().GetAvailableActionPoints());

            foreach (Transform child in m_ElementDetailCollection.transform)
                Destroy(child.gameObject);

            foreach (UnitElement element in UnitActionSystem.Instance.GetSelectedUnit().GetStartingElements())
            {
                ElementStatusPanel element_status_panel = Instantiate(m_ElementDetailsPanelPrefab.gameObject, m_ElementDetailCollection.transform).GetComponent<ElementStatusPanel>();
                element_status_panel.UpdateDetails(element, target_unit, cover_modifier);
            }
        }
        else Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UnitDetailsPanel_OnSelectedUnitChange(object sender, System.EventArgs e)
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            Show(null, 0f);
        }
        else
        {
            Hide();
        }
    }

    private void Unit_OnAnyUnitStatusChange(object sender, System.EventArgs e)
    {
        Show(null, 0f);
    }

    private void MouseUnitHover_OnHoveredUnitChange(object sender, System.EventArgs e)
    {
        Unit l_SelectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (l_SelectedUnit != null) 
        {
            if ((UnitActionSystem.Instance.GetSelectedAction() != null) && (UnitActionSystem.Instance.GetSelectedAction() is ShootAction))
            {
                Unit l_HoveredUnit = MouseUnitHover.Instance.GetHoveredUnit();

                if ((l_HoveredUnit != null) && (Unit.AreOpposed(l_SelectedUnit.GetUnitForce(), l_HoveredUnit.GetUnitForce())))
                {
                    Show(l_HoveredUnit, LevelGrid.Instance.GetCoverModifer(l_SelectedUnit.GetUnitHeadHeight().position, l_HoveredUnit));
                    return;
                }
            }
        }
        
        Show(null, 0f);
    }
}
