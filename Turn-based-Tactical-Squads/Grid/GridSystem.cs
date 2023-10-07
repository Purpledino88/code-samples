using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int m_Width;
    public int GetWidth() { return m_Width; }
    private int m_Height;
    public int GetHeight() { return m_Height; }

    private TGridObject[,] m_GridObjectArray;

    public GridSystem(int width, int height, Func<GridSystem<TGridObject>, GridPosition, TGridObject> new_gridobject_function)
    {
        m_Width = width;
        m_Height = height;
        
        m_GridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Height; z++)
            {
                m_GridObjectArray[x, z] = new_gridobject_function(this, new GridPosition(x, z));
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition grid_position)
    {
        return new Vector3(grid_position.x, 0, grid_position.z);
    }

    public GridPosition GetGridPosition(Vector3 world_position)
    {
        return new GridPosition(
            Mathf.RoundToInt(world_position.x),
            Mathf.RoundToInt(world_position.z)
        );
    }

    public TGridObject GetGridObject(GridPosition grid_position)
    {
        return m_GridObjectArray[grid_position.x, grid_position.z];
    }

    public void CreateDebugObjects(GameObject debug_prefab)
    {
        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Height; z++)
            {
                GridPosition pos = new GridPosition(x, z);

                GameObject l_DebugVisual = GameObject.Instantiate(debug_prefab, GetWorldPosition(pos), Quaternion.identity);
                l_DebugVisual.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(pos));
            }
        }
    }

    public bool IsValid(GridPosition grid_position)
    {
        return ((grid_position.x >= 0) && (grid_position.x < m_Width) && (grid_position.z >= 0) && (grid_position.z < m_Height));
    }
}
