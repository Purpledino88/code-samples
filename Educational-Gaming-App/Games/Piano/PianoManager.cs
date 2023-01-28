using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
The specific game manager for the 'Piano' rhythm/dexterity game.
A rhythm action game where notes to a song (composed in the editor) proceed across the top of the screen.
A single scale of piano keys are arranged along the bottom of the screen, and will light up in different colours as the appropriate note reaches a target line on the screen.
Players must click the piano keys at the right time (as they pass the target line) to play the song correctly.
*/
public class PianoManager : SpecificManager
{
    //Allowed variance in seconds, how much can the player miss the perfect timing for a note without losing points
    private const float MAX_VARIANCE = 0.35f;
    private const float MIN_VARIANCE = 0.05f;
    //Difficulty at which the minimum variance is enforced
    private const int LOWEST_VARIANCE_DIFFICULTY = 50;

    //Prefab object for our note rectangles
    public SongNote notePrefab;

    //Location of the target line
    public RectTransform noteTargetPosition;

    //Delay before the first note reaches the target line, to allow players to see it coming (in seconds)
    public float songStartDelay;

    //Images/buttons for our piano keys
    public Image lowCKey;   //Do
    public Image dKey;      //Re
    public Image eKey;      //Mi
    public Image fKey;      //Fa
    public Image gKey;      //So
    public Image aKey;      //La
    public Image bKey;      //Ti
    public Image highCKey;  //Do

    //Struct for each note in a composition
    [System.Serializable]
    public class Note
    {
        public string pitch;
        public float timing;
    }
    public List<Note> introNotes; //A simple introduction to allow players to see how the game works (do-re-mi-fa-so-la-ti-do)
    public List<Note> tuneNotes;  //The actual composition which players will be assessed against

    //Struct to contain graphical and auditory data for each note
    [System.Serializable]
    public class NoteData
    {
        public string pitch;
        public Color colour; //Different colours for each note
        public AudioClip audio; //Audio clips for each note/key
        public float yValue; //Vertical location on our musical notation
    }
    public List<NoteData> noteData; //Created in editor, master list of each available note on our piano

    //Various dictionaries to be populated from the above editor list of structs, and to be used by our program to speed up lookup times
    //The key for all of these dictionaries will be the pitch of the note
    private Dictionary<string, Color> m_NoteColours = new Dictionary<string, Color>();
    private Dictionary<string, AudioClip> m_NoteAudioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, float> m_NoteYValues = new Dictionary<string, float>();
    private Dictionary<string, Image> m_Keys = new Dictionary<string, Image>(); //NOTE: piano keys, not dictionary keys!

    private List<SongNote> m_ComingNotes = new List<SongNote>(); //List of remaining notes in our composition
    private List<SongNote> m_MissedNotes = new List<SongNote>(); //List of missed notes from our composition (player didn't click the right button)

    //Flag to show if the simple introductory composition has finished playing
    private bool m_IntroFinished;

    //Timers for our introductory composition
    private float m_IntroTime;
    private float m_IntroEndTime;

    //Timers for our assessable composition
    private float m_GameTime;
    private float m_GameEndTime;

    //Score penalty for missing a note or pressing the wrong key
    private float m_MistakePenalty;

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected override void LoadVocabulary()
    {
        //No vocabulary needed for this game
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected override void SetupGame()
    {
        //Populate our piano keys dictionary with the correct buttons
        m_Keys.Add("LowC", lowCKey);
        m_Keys.Add("D", dKey);
        m_Keys.Add("E", eKey);
        m_Keys.Add("F", fKey);
        m_Keys.Add("G", gKey);
        m_Keys.Add("A", aKey);
        m_Keys.Add("B", bKey);
        m_Keys.Add("HighC", highCKey);

        //Set times equal to the negative of the delay set by the editor (measured in seconds)
        m_IntroTime = -songStartDelay;
        m_GameTime = -songStartDelay;

        //Populate each dictionary for each available pitch from the editor set list
        foreach (NoteData nd in noteData)
        {
            m_NoteColours.Add(nd.pitch, nd.colour);
            m_NoteAudioClips.Add(nd.pitch, nd.audio);
            m_NoteYValues.Add(nd.pitch, nd.yValue);
        }

        //Disable the usual pre-game countdown for our games, it should instead play after the introductory composition has finished
        m_IsStarting = false;

        //Setup our introductory composition
        SetupIntro();
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected override void UpdateGame(float dt)
    {
        //Call the appropriate function depending on if we are in the introduction or the main game
        if (m_IntroFinished)
            UpdatePlayerGame(dt);
        else UpdateIntro(dt);
    }

    /*
    Function to spawn all of the note rectangles for our introductory composition and place them in the correct positions
    */
    private void SetupIntro()
    {
        //Our prefab's standard width will be equal to one second of game time
        float second_width = notePrefab.GetComponent<RectTransform>().sizeDelta.x;

        //Call SongNote class to set up the values used by all of our introductory notes for this game session (minimum size)
        SongNote.SetStaticValues(second_width, songStartDelay, MIN_VARIANCE);

        //Flag the introduction as unfinished
        m_IntroFinished = false;

        if (introNotes.Count > 0) //If we have an introductory composition
        {
            //Spawn a note rectangle of the correct colour and position for every note in the composition
            foreach (Note note in introNotes)
            {
                SongNote song_note = Instantiate(notePrefab, noteTargetPosition);
                song_note.transform.SetParent(noteTargetPosition, false); //Having note rects be children of the target line means we can use the timing of the note as a local transform from the parent

                float y_pos;
                m_NoteYValues.TryGetValue(note.pitch, out y_pos);
                Color colour;
                m_NoteColours.TryGetValue(note.pitch, out colour);
                song_note.Initialise(note.pitch, y_pos, colour, note.timing);

                m_ComingNotes.Add(song_note); //Add this note to our list to be evaluated during playtime
            }

            //Calculate the end time of our introduction as equal to the editor-set delay after our last note has passed the target line
            m_IntroEndTime = introNotes[introNotes.Count - 1].timing + songStartDelay;
        }
        else
        {
            Debug.Log("UNITY - No intro notes found, returning to main menu");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    /*
    Frame by frame update loop for our introductory composition (to be called from the overridden Update function from SpecificManager)
    */
    private void UpdateIntro(float dt)
    {
        //Update the time
        m_IntroTime += dt;

        //Call each note to update itself
        foreach (SongNote note in m_ComingNotes)
            note.UpdateNote(m_IntroTime);

        //If there are still notes to come in the song
        if (m_ComingNotes.Count > 0)
        {
            //Since notes should be played in a musical, ordered sequence, we only have to evaluate the first oncoming note
            SongNote first_coming_note = m_ComingNotes[0];

            //Prepare a colour change for the piano key from our dictionary values
            Image key;
            m_Keys.TryGetValue(first_coming_note.GetPitch(), out key);
            Color key_color;
            m_NoteColours.TryGetValue(first_coming_note.GetPitch(), out key_color);

            if (first_coming_note.IsActive()) //Notes are active if the current time is within their acceptable range to be played
                key.color = key_color; //If active, light up the key with the correct colour, otherwise, leave it white
            else key.color = Color.white;

            //For our intro, the audio will be pressed automatically to play the composition even without player input
            if (m_IntroTime > first_coming_note.GetTiming())
            {
                KeyPressed(first_coming_note.GetPitch());
            }
        }

        //If the intro has exceeded it's calculated time
        if (m_IntroTime > m_IntroEndTime)
        {
            SetupPlayerGame();
            m_IntroFinished = true;

            //Reenable the standard game countdown to show the player that now is the time for them to prepare
            m_IsStarting = true;
            m_Countdown = 3.0f;
            introPanel.gameObject.SetActive(true);
        }
    }

    /*
    Function to spawn all of the note rectangles for our main gameplay composition and place them in the correct positions
    */
    private void SetupPlayerGame()
    {
        //Calculate the allowed variance of our notes for this session
        float variance_percentage = 1 - (Mathf.Min(m_Difficulty, LOWEST_VARIANCE_DIFFICULTY) / (float)LOWEST_VARIANCE_DIFFICULTY);
        float variance_range = MAX_VARIANCE - MIN_VARIANCE;
        float allowed_variance = MIN_VARIANCE + (variance_percentage * variance_range);

        //Our prefab's standard width will be equal to one second of game time
        float second_width = notePrefab.GetComponent<RectTransform>().sizeDelta.x;

        //Call SongNote class to set up the values used by all of our notes for this game session of the correct size
        SongNote.SetStaticValues(second_width, songStartDelay, allowed_variance);

        if (tuneNotes.Count > 0) //If a song has been provided in the editor
        {
            //Spawn a note rectangle of the correct colour and position for every note in the composition
            foreach (Note note in tuneNotes)
            {
                SongNote song_note = Instantiate(notePrefab, noteTargetPosition);

                //Having note rects be children of the target line means we can use the timing of the note as a local transform from the parent
                song_note.transform.SetParent(noteTargetPosition, false);

                float y_pos;
                m_NoteYValues.TryGetValue(note.pitch, out y_pos);
                Color colour;
                m_NoteColours.TryGetValue(note.pitch, out colour);
                song_note.Initialise(note.pitch, y_pos, colour, note.timing);

                //Add this note to our list to be evaluated during playtime
                m_ComingNotes.Add(song_note);
            }

            //Calculate the end time of our composition as equal to the editor-set delay after our last note has passed the target line
            m_GameEndTime = tuneNotes[tuneNotes.Count - 1].timing + songStartDelay;

            //Calculate the penalty for incorrect notes as equal to 1/number of notes (so missing all of the notes will result in a score of zero)
            m_MistakePenalty = 1f / tuneNotes.Count;
        }
        else
        {
            Debug.Log("UNITY - No song notes found, returning to main menu");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    /*
    Frame by frame update loop for our main gameplay composition (to be called from the overridden Update function from SpecificManager)
    */
    private void UpdatePlayerGame(float dt)
    {
        //Update the time
        m_GameTime += dt;

        //Call each remaining note to update itself
        foreach (SongNote note in m_ComingNotes)
            note.UpdateNote(m_GameTime);

        
        //Call each missed note to update itself
        foreach (SongNote note in m_MissedNotes)
            note.UpdateNote(m_GameTime);
        
        //If there are still notes to come in the song
        if (m_ComingNotes.Count > 0)
        {
            //Since notes should be played in a musical, ordered sequence, we only have to evaluate the first oncoming note
            SongNote first_coming_note = m_ComingNotes[0];

            //Prepare a colour change for the piano key from our dictionary values
            Image key;
            m_Keys.TryGetValue(first_coming_note.GetPitch(), out key);
            Color key_color;
            m_NoteColours.TryGetValue(first_coming_note.GetPitch(), out key_color);

            if (first_coming_note.IsActive()) //Notes are active if the current time is within their acceptable range to be played
                key.color = key_color; //If active, light up the key with the correct colour, otherwise, leave it white
            else key.color = Color.white;

            //If the player has missed the range allowed by this note without pressing the correct key
            if (first_coming_note.IsPassed())
            {
                //Lower the player's score and move the note into the missed notes list
                progressPanel.UpdateProgress(-m_MistakePenalty);
                m_ComingNotes.Remove(first_coming_note);
                m_MissedNotes.Add(first_coming_note);
            }
        }

        //If there are still missed notes visible on the screen
        if (m_MissedNotes.Count > 0)
        {
            //Again, all notes move at the same speed so we only need to evaluate the earliest note
            SongNote first_missed_note = m_MissedNotes[0];

            //If the note has faded into transparency, remove it from the list and destroy the gameObject
            if (first_missed_note.IsFaded())
            {
                m_MissedNotes.Remove(first_missed_note);
                Destroy(first_missed_note.gameObject);
            }
        }

        //If the game has exceeded it's calculated time (after all notes have passed the target line)
        if (m_GameTime > m_GameEndTime)
            EndGame();
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected override void SetThresholds()
    { //Decremental game, start at full score and lose points for mistakes
        //Star reward values set at quarterly intervals of the progress bar
        progressPanel.SetStartValues(1.0f, 0.25f, 0.5f, 0.75f);
    }

    /*
    Function to be called by our piano key buttons on click
    */
    public void KeyPressed(string pitch)
    {
        //Play the audio file for this pitch
        AudioClip audio;
        m_NoteAudioClips.TryGetValue(pitch, out audio);
        if (audio != null)
            m_AudioSource.PlayOneShot(audio);

        //If we still have notes in the song to play
        if (m_ComingNotes.Count > 0)
        {
            //Check against the first note
            SongNote first_coming_note = m_ComingNotes[0];

            if (first_coming_note.IsActive())
            {
                if (pitch.Equals(first_coming_note.GetPitch()))
                {
                    //If this note is within the acceptable range and is the right pitch, reset the key and destroy the note GameObject
                    Image key;
                    m_Keys.TryGetValue(first_coming_note.GetPitch(), out key);
                    key.color = Color.white;

                    m_ComingNotes.Remove(first_coming_note);
                    Destroy(first_coming_note.gameObject);
                }
                else
                {   //If it's the wrong pitch, apply the penalty
                    progressPanel.UpdateProgress(-m_MistakePenalty);
                }
            }
            else
            {   //If it's not within the correct time range, apply the penalty
                progressPanel.UpdateProgress(-m_MistakePenalty);
            }
        }
        else
        {   //If the song has ended, apply the penalty
            progressPanel.UpdateProgress(-m_MistakePenalty);
        }
    }
}
