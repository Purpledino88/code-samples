using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Class to represent a single note of a musical sequence.
Will be attached to a rectangular image/bar which moves along the screen in time with the music.
Colour and Y position will change depending on the pitch of the note.
Width and speed will change depending on difficulty.
*/
[RequireComponent(typeof(Image))]
public class SongNote : MonoBehaviour
{
    // Static values to be set for all notes in this particular game session
    private static float s_WidthOfOneSecond; //X value which represents one second of game time
    private static float s_SongDelay;       //Delay between game start and arrival of first note
    private static float s_AllowedVariance; //Amount player is allowed to miss the perfect timing by an still count as a successful note
    private static float s_StartOffsetX;    //Starting positions of note images (should be off the right side of the screen)

    /*
    Called when musical sequence is being set up to set static values based on game difficulty.
    Should be called by both the intro sequence and the actual game.
    */
    public static void SetStaticValues(float width_of_one_second, float song_delay, float variance)
    {
        s_SongDelay = song_delay;
        s_WidthOfOneSecond = width_of_one_second;
        s_AllowedVariance = variance;
        s_StartOffsetX = width_of_one_second * song_delay;
    }

    //Pointer to the image of this note (spawned from prefab in the editor)
    private Image m_Image;

    //Pitch of this note
    private string m_Pitch;
    public string GetPitch() { return m_Pitch; }

    //m_Timing represents the perfect timing, while early and late times are calculated based on difficulty
    private float m_Timing;
    private float m_EarliestTime;
    private float m_LatestTime;
    public float GetTiming() { return m_Timing; }

    //Status check flags
    private bool m_Active = false;  //Can this note be played correctly at this time?
    private bool m_Passed = false;  //Has this note already been missed at this time?
    private bool m_Faded = false;   //Has this note faded into transparency at this time?
    
    //Status check functions to be used by the game manager
    public bool IsActive() { return m_Active; }
    public bool IsPassed() { return m_Passed; }
    public bool IsFaded() { return m_Faded; }

    /*
    Called on creation from prefab by the game manager.
    Parameters will be supplied from game manager lists which are modifiable in the editor.
    */
    public void Initialise(string pitch, float yPos, Color colour, float perfect_time)
    {
        m_Pitch = pitch;

        //Calculate appropriate timings for selection
        m_Timing = perfect_time;
        m_EarliestTime = perfect_time - s_AllowedVariance;
        m_LatestTime = perfect_time + s_AllowedVariance;

        //Change colour based on parameter
        m_Image = GetComponent<Image>();
        m_Image.color = colour;
        
        //Modify object size and position
        RectTransform transform = GetComponent<RectTransform>();
        transform.localPosition = new Vector2(s_StartOffsetX, yPos); //start at static preset X and parameter supplied Y
        transform.sizeDelta = new Vector2(s_WidthOfOneSecond * (m_LatestTime - m_EarliestTime), transform.sizeDelta.y); //Stretch in X axis based on calculated timings
    }

    /*
    Update function called each frame.
    */
    public void UpdateNote(float current_time)
    {
        //Calculate time until this note would be perfect and change note's X position to match 
        float time_to_note = m_Timing - current_time;
        float xPos = time_to_note * s_WidthOfOneSecond;
        transform.localPosition = new Vector2(xPos, transform.localPosition.y);

        //If current time is within this note's selection range, mark the note as active
        if ((current_time > m_EarliestTime) && (current_time < m_LatestTime))
            m_Active = true;
        else m_Active = false;

        //If this note has not been selected before the allowable time has passed
        if (current_time > m_LatestTime)
        {
            m_Passed = true; //Mark as passed

            //Fade this note to transparent based on how much time has elapsed since it was unselectable
            float time_difference = current_time - m_LatestTime;
            Color newColor = m_Image.color;
            newColor.a = 1f - (time_difference / (s_SongDelay / 2)); //Time to fade should be half the duration of the note's time on screen
            m_Image.color = newColor;

            //If note has become totally transparent, mark it as faded (thus allowing deletion by game manager)
            if (m_Image.color.a <= 0f)
                m_Faded = true;
        }
        else m_Passed = false;
    }
}
