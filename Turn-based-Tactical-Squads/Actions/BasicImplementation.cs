using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicImplementation : MonoBehaviour
{
    protected UnitElement m_Element;

    protected bool m_Completable;
    public bool CanComplete() { return m_Completable; }

    public event EventHandler OnImplementationDestruction;

    private void Awake()
    {        
        m_Element = GetComponent<UnitElement>();
    }

    public abstract void StartElement();

    public abstract void UpdateElement();

    public abstract void EndElement();

    private void OnDestroy() 
    {
        OnImplementationDestruction?.Invoke(this, EventArgs.Empty);
    }
}
