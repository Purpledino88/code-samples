using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Class to represent a picture rolling along our conveyor belt in the game of the same name.
Will be a prefab in the editor, and instantiated by the game manager.
*/
[RequireComponent(typeof(Image), typeof(RectTransform))]
public class ConveyedObject : MonoBehaviour
{
    private const float TARGET_CANVAS_WIDTH = 1280;

    private GameConveyorBelt m_Manager;
    private int m_Index;
    private float m_Speed;
    private RectTransform m_CurrentTransform;
    private bool m_HasSpoken = false; //Flag: has this image given it's oral pronuciation yet?

    /*
    Called on creation by game manager
    */
    public void Initiate(int index, Sprite sprite, GameConveyorBelt manager)
    {
        m_Index = index;
        GetComponent<Image>().sprite = sprite;
        m_CurrentTransform = GetComponent<RectTransform>(); //Game manager will instantiate to an editor-set position just off the right screen edge
        m_Manager = manager;
        gameObject.SetActive(false); //deactivate until reset/activated by manager
    }

    /*
    Reset object to start position and activate
    */
    public void Reset(RectTransform pos, float duration)
    {
        m_CurrentTransform.localPosition = new Vector2(m_CurrentTransform.sizeDelta.x / 2, pos.localPosition.y);
        m_Speed = TARGET_CANVAS_WIDTH / duration;   //object must cross the canvas in the parameter-given time
        m_HasSpoken = false;
        gameObject.SetActive(true);
    }

    /*
    Update function called every frame
    */
    public void UpdatePosition(float dt)
    {
        //Modify position based on speed
        m_CurrentTransform.localPosition = new Vector2(m_CurrentTransform.localPosition.x - (m_Speed * dt), m_CurrentTransform.localPosition.y);

        //If object has passed the screen midpoint and hasn't played it's pronunciation yet, call manager to play the audio
        if (!m_HasSpoken && (m_CurrentTransform.localPosition.x < (0.5 * -(TARGET_CANVAS_WIDTH + m_CurrentTransform.sizeDelta.x))))
        {
            m_Manager.PlayAudio(m_Index);
            m_HasSpoken = true;
        }

        //If object has moved off the left side of the screen, deactivate it
        if (m_CurrentTransform.localPosition.x < -(TARGET_CANVAS_WIDTH + m_CurrentTransform.sizeDelta.x))
        {
            gameObject.SetActive(false);
        }
    }
}
