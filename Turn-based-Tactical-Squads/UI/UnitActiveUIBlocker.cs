using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActiveUIBlocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
        Hide();
        UnitActionSystem.Instance.OnActiveChange += UnitActionSystem_OnActiveChange;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnActiveChange(object sender, bool is_active)
    {
        if (is_active)
            Show();
        else
            Hide();
    }


}
