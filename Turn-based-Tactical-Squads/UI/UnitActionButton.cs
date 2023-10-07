using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionButton : MonoBehaviour
{
    [SerializeField] private Button _Button;
    [SerializeField] private TMPro.TextMeshProUGUI _Text;

    public void SetActionType(BasicAction act)
    {
        _Text.text = act.GetActionName();
        if (!UnitActionSystem.Instance.GetSelectedUnit().CanTakeAction(act))
            _Button.interactable = false;
        _Button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(act);
        });
    }
}
