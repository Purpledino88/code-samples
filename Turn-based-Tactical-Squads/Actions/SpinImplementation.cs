using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinImplementation : BasicImplementation
{
    private float m_AmountSpun;

    public override void StartElement()
    {
        m_Completable = false;
    }
    
    public override void UpdateElement()
    {
        if (!m_Completable)
        {
            float l_SpinAmount = 360f * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, l_SpinAmount, 0);

            m_AmountSpun += l_SpinAmount;

            if (m_AmountSpun >= 360f)
            {
                m_Completable = true;
            }
        }        
    }
    
    public override void EndElement()
    {
        m_AmountSpun = 0f;
    }
}
