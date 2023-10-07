using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointsDisplay : MonoBehaviour
{
    [SerializeField] private Image[] _ActionPointImageArray;

    public void UpdateDisplay(int available_points)
    {
        for (int i = 0; i < _ActionPointImageArray.Length; i++)
        {
            if (i < available_points)
                _ActionPointImageArray[i].gameObject.SetActive(true);
            else _ActionPointImageArray[i].gameObject.SetActive(false);
        }
    }
}
