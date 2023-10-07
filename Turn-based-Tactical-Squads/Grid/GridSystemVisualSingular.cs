using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingular : MonoBehaviour
{
    [SerializeField] private MeshRenderer _MeshRenderer;

    public void ShowVisual(Color visual_colour)
    {
        _MeshRenderer.material.color = visual_colour;
        _MeshRenderer.enabled = true;
    }

    public void HideVisual()
    {
        _MeshRenderer.enabled = false;
    }
}
