using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHealthIndicator : MonoBehaviour
{
    [SerializeField] private Image m_Indicator;

    public void ShowHealth(UnitElement element)
    {
        if (element == null)
        {
            m_Indicator.color = Color.black;
            return;
        }

        switch (element.GetHealth())
        {
            case 4: //Healthy
            {
                m_Indicator.color = Color.green;
                break;
            }
            case 3: //Walking Wounded
            {
                m_Indicator.color = Color.yellow;
                break;
            }
            case 2: //Badly Wounded
            {
                m_Indicator.color = Color.red;
                    break;
            }
            case 1: //Unconcious
            {
                m_Indicator.color = Color.grey;
                break;
            }
            case 0: //Dead
            {
                m_Indicator.color = Color.black;
                break;
            }
        }
    }
}
