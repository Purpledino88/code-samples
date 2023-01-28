using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
An abstract class providing base functionality needed by all of the games used.
Each game can provided it's own rules using the abstract functions.
*/
[RequireComponent(typeof(AudioSource))]
public abstract class SpecificManager : MonoBehaviour
{
    //Various panels designed and linked in the editor
    public GameProgressPanel progressPanel;
    public IntroPanel introPanel;
    public EndgamePanel endgamePanel;

    //Store the Game Manager class consistent throughout the program, which gives us access to the Game States for each game
    protected GameManager m_GameManager;

    //Timekeeping variables to show us when our game can be played
    protected bool m_IsStarting = false;
    protected bool m_IsEnded = false;
    protected float m_Countdown = 0.0f;
    public bool IsPlayable() { return (!m_IsEnded && !m_IsStarting); }

    //Store the difficulty which each game will use, but in different ways
    protected int m_Difficulty;

    //Almost all games will have some usable vocabulary, so put it in here (games without words can leave this empty)
    protected List<string> m_VocabList = new List<string>();

    //All games will use consistent audio for correct answers and earning stars, plus maybe some game-specific audio
    protected AudioSource m_AudioSource;
    protected AudioClip m_CorrectAudio;
    protected AudioClip m_VictoryAudio;

    /*
    Called on entering the scene, this function will setup everything the game needs before it begins
    */
    void Start()
    {
        //Default will be showing the countdown screen before the beginning of the game
        Debug.Log("UNITY: Starting manager for specific game...");
        introPanel.gameObject.SetActive(true);
        endgamePanel.gameObject.SetActive(false);

        //Setup audio sources and consistent sound effects
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource != null)
            Debug.Log("UNITY: Found audio source");
        else Debug.Log("UNITY: Could not find audio source");
        m_CorrectAudio = Resources.Load<AudioClip>("bling");
        m_VictoryAudio = Resources.Load<AudioClip>("victory");

        //Get the game manager, which should have been loaded in the main menu and is not deztroyed on scene shift
        GameObject gm = GameObject.Find("Game Manager");
        if (gm != null)
        {
            Debug.Log("UNITY: -----Found game manager " + gm);

            m_GameManager = gm.GetComponent<GameManager>();
            Debug.Log("UNITY: ----------Getting relevant component " + m_GameManager);

            //Poll game state for this game via the game manager to get the difficulty for today
            m_Difficulty = m_GameManager.m_GameInProgress.difficulty;

            //Poll game state for this game via the game manager to get the number of stars earned today, and pass this on to the progress panel
            //Remember, stars can only be earned in sequence (i.e. a silver star can never be earned unless a bronze is/has been earned)
            Debug.Log("UNITY: -----Setting potential prizes");
            progressPanel.SetPotentialPrizes(m_GameManager.m_GameInProgress.stars_earned < 1, m_GameManager.m_GameInProgress.stars_earned < 2, m_GameManager.m_GameInProgress.stars_earned < 3);

            //Load the words to be used, overridden by each game depending upon it's specific needs
            Debug.Log("UNITY: -----Loading vocabulary");
            LoadVocabulary();
        }
        else
        {
            Debug.Log("UNITY: -----Could not find game manager");
        }

        //Setup the countdown to start next frame
        Debug.Log("UNITY: -----Setting initial values");
        m_IsStarting = true;
        m_Countdown = 3.0f;

        //Game specific setup, overridden by each game depending upon it's specific needs
        Debug.Log("UNITY: -----Setting up game values");
        SetupGame();

        //Set the thresholds for rewards in this game, overridden by each game depending upon it's specific needs
        Debug.Log("UNITY: -----Setting thresholds for game");
        SetThresholds();

        Debug.Log("UNITY: Manager for specific game started...");
    }

    /*
    Update function called once per frame to handle the general gameflow or call game-specific updates
    */
    void Update()
    {
        float dt = Time.deltaTime;

        if (m_IsStarting) //If game is starting
        {
            if (m_Countdown > 0.0f) //If countdown has not ended, update the countdown and pass it to the IntroPanel
            {
                m_Countdown -= dt;
                introPanel.UpdateCountdown(m_Countdown);
            }
            else    //If countdown has ended, turn off isStarting flag
            {
                Debug.Log("Beginning Game!");
                m_IsStarting = false;
            }
        }
        else if (!m_IsEnded) // Iff game has not ended, call the overridded, game-specific Update
        {
            UpdateGame(dt);
        }
    }

    /*
    This will end the game, and may be called in different ways.
    The progress panel will call this if the games progress reaches either 0 or 1 (usually representing time running out or having survived a set time).
    Games may call this based on their own rules (too many wrong answers, all vocabulary has been shown, etc.).
    */
    protected void EndGame()
    {
        m_IsEnded = true; //Set endgame flag to stop the Update loop

        //Calculate how many stars have been earned in the instance of this game
        int stars = 0;
        if (progressPanel.EarnedBronzeStar()) 
        {
            if (m_GameManager.m_GameInProgress.stars_earned < 1)
                m_GameManager.m_BronzeStars++; //Update the game manager which handles data consistency and stores earned stars
            stars = 1;
        }
        if (progressPanel.EarnedSilverStar())
        {
            if (m_GameManager.m_GameInProgress.stars_earned < 2)
                m_GameManager.m_SilverStars++; //Update the game manager which handles data consistency and stores earned stars
            stars = 2;
        }
        if (progressPanel.EarnedGoldStar())
        {
            if (m_GameManager.m_GameInProgress.stars_earned < 3)
                m_GameManager.m_GoldStars++; //Update the game manager which handles data consistency and stores earned stars
            stars = 3;
        }

        if (stars > 0)
            m_AudioSource.PlayOneShot(m_VictoryAudio);

        //Update the game state (via game manager) to remember the current number of stars this game has earned today
        //MAX against previous playthroughs to avoid a bad game resetting the game state
        m_GameManager.m_GameInProgress.stars_earned = Mathf.Max(m_GameManager.m_GameInProgress.stars_earned, stars); 

        //Activate the endgame panel which contains a button to transit back to the main menu
        endgamePanel.Activate(progressPanel);
    }

    /*
    Abstract function to be overridden by each specific game.
    Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected abstract void LoadVocabulary();

    /*
    Abstract function to be overridden by each specific game.
    Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected abstract void SetupGame();

    /*
    Abstract function to be overridden by each specific game.
    Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected abstract void UpdateGame(float dt);

    /*
    Abstract function to be overridden by each specific game.
    Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected abstract void SetThresholds();
}