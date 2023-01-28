using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Simple class to handle one of the options for a player to choose in the Conveyor Belt game
*/
public class OptionButtonConveyor : MonoBehaviour
{
    private int m_DataIndex; //int pointer to one of the vocabulary words in this game session
    private GameConveyorBelt m_Manager;

    /*
    Called by game manager to update button to match appropriate vocabulary
    */
    public void Load(GameConveyorBelt manager, Sprite picture, int index)
    {
        m_DataIndex = index;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = picture;
        m_Manager = manager;
    }

    /*
    Simple on button click to pass this button's stored data index up to game manager for evaluation
    */
    public void Select()
    {
        m_Manager.MakeSelection(m_DataIndex);
    }
}
