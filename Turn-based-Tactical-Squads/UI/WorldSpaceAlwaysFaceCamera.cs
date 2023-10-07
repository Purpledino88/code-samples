using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceAlwaysFaceCamera : MonoBehaviour
{
    private Transform m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = m_Camera.rotation;
    }
}
