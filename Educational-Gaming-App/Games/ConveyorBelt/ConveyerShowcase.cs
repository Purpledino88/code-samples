using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Class used in the 'Conveyor Belt' game to parade a consistent procession of objects repeatedly.
Objects can be paraded in one of two ways:
    - Before player input, slowly, to allow players to memorise the objects
    - After all player input, quickly, to evaluate the players answers
Order of objects must be consistent, though number of objects shown will increase as the game goes on.
For example: 1st round - memorise object at index 0
            2nd round - memorise objects at indices 0 and 1
            3rd round - memorise objects at indices 0, 1, and 2
            etc.
*/
public class ConveyerShowcase : MonoBehaviour
{
    //Base time for an object to cross the width of the screen
    private const float BASE_DURATION = 2f;

    //boolean flag to show if showcase is currently parading the objects
    private bool m_InProgress;

    //INdex of the next object to be spawned
    private int m_CurrentIndex;
    //Index of the final object to be paraded this round
    private int m_LastIndex;

    //time for an object to cross the width of the screen in this showcase
    private float m_TraversalTime;

    //Time between creation of new objects (in seconds)
    private float m_MaxSpawnTime;
    //Time to next object spawn (seconds)
    private float m_CurrentSpawnTime;

    //Spawn position for prefabed objects
    private RectTransform m_ObjectStartPosition;

    //List containing all ConveyedObject instances created for this game session
    private List<ConveyedObject> m_ConveyedObjects = new List<ConveyedObject>();

    //Boolean functions to be used by game manager
    public bool ReadyToStart() { return (!m_InProgress && m_CurrentIndex == 0); }
    public bool HasEnded() { return (!m_InProgress && m_CurrentIndex > 0); }

    /*
    Function called once by game manager during game setup.
    Images will be a list of images of the words used in the game.
    */
    public void InitialiseShowcase(GameObject prefab, List<Sprite> images, GameConveyorBelt manager, RectTransform start_position)
    {
        //Save start position to member variable
        m_ObjectStartPosition = start_position;

        //For each image supplied, create a ConveyedObject and put them in an ordered list
        for (int i = 0; i < images.Count; i++)
        {
            ConveyedObject newObj = Instantiate(prefab, m_ObjectStartPosition).GetComponent<ConveyedObject>();
            newObj.Initiate(i, images[i], manager);
            m_ConveyedObjects.Add(newObj);
        }

        //Call ResetShowcase to set starting values
        ResetShowcase();
    }

    /*
    Called every time the showcase should begin parading.
    Parameters determine how many objects are paraded and how fast.
    */
    public void StartShowcase(int last_index, float speed_multiplier)
    {
        m_TraversalTime = BASE_DURATION / speed_multiplier;
        m_MaxSpawnTime = m_TraversalTime * 0.6f; //Allow object to get 60% of the way acroos the screen before spawning next object
        m_CurrentSpawnTime = 0; //Set to zero to spawn new object immediately
        m_InProgress = true;
        m_LastIndex = last_index;
    }

    /*
    Reset showcase to initial settings
    */
    public void ResetShowcase()
    {
        m_InProgress = false;
        m_CurrentIndex = 0;
    }

    /*
    Update function called every frame
    */
    public void UpdateShowcase(float dt)
    {
        //Decrease time to next spawn
        m_CurrentSpawnTime -= dt;

        if (m_CurrentSpawnTime < 0f) //If it is time to spawn a new object
        {
            if (m_CurrentIndex <= m_LastIndex) //If there are more objects to show this round, spawn the next one
                SpawnObject();
            else    //If we've spawned the last object, check if they have all finished traversing the screen
                CheckActivations();
        }

        //Update all active ConveyedObjects
        foreach (ConveyedObject co in m_ConveyedObjects)
        {
            if (co.gameObject.activeSelf)
                co.UpdatePosition(dt);
        }
    }

    /*
    Function to check if we still have active ConveyedObjects on screen.
    Should only be called after all objects for this round have been spawned.
    */
    private void CheckActivations()
    {
        bool have_active_objects = false;
        foreach (ConveyedObject co in m_ConveyedObjects)
        {
            if (co.gameObject.activeSelf)
                have_active_objects = true;
        }

        if (!have_active_objects)
            m_InProgress = false;
    }

    /*
    Function to spawn the next object in our showcase
    */
    private void SpawnObject()
    {
        m_ConveyedObjects[m_CurrentIndex].Reset(m_ObjectStartPosition, m_TraversalTime);
        m_CurrentSpawnTime = m_MaxSpawnTime;
        m_CurrentIndex++; //Prepare the next object index
    }
}
