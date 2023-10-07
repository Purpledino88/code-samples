using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDetailsPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _NameText;

    public void UpdateDetails()
    {
        gameObject.SetActive(true);
        _NameText.text = UnitActionSystem.Instance.GetSelectedAction().GetActionName();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
