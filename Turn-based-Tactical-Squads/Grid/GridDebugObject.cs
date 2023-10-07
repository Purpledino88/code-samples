using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshPro _TextField;

    private object m_GridObject;
    public virtual void SetGridObject(object grid_object) { m_GridObject = grid_object; }

    protected virtual void Update()
    {
        _TextField.text = m_GridObject.ToString();
    }
}
