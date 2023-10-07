using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld s_Instance;

    [SerializeField] private LayerMask floorLayerMask;

    private void Awake() 
    {
        s_Instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, s_Instance.floorLayerMask);
        return hit.point;
    }
}
