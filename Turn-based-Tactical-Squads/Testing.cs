using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    void Start()
    {
       
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mousePosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startPosition = new GridPosition(0, 0);

            List<GridPosition> pathList = Pathfinder.Instance.FindPath(startPosition, mousePosition, out int move_points_used);

            if (pathList != null)
            {
                for (int i = 0; i < pathList.Count - 1; i++)
                {
                    Debug.DrawLine(
                        LevelGrid.Instance.GetWorldPosition(pathList[i]),
                        LevelGrid.Instance.GetWorldPosition(pathList[i + 1]),
                        Color.blue,
                        10f
                    );
                }
            }
            else
            {
                Debug.Log("Cannot find path");
            }
        }
    }
}
