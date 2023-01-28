using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
The specific game manager for the 'Conveyor Belt' memory game.
This game features a conveyor belt upon which various items will pass in a consistent order.
Game will increase in a number of rounds, with one item first, followed by two, then three, up to seven.
Players must watch as the items pass and then select the items in the correct order to earn points.
*/
public class GameConveyorBelt : SpecificManager
{
    //Constant maximum number of rounds to be played
    private const int MAX_ROUNDS = 7;
    //Constant maximum time allowed for each guess (measured in seconds)
    private const float BASE_TIME = 5f;

    //Small panel linked to editor showing which objects the player has already selected for this round
    public GameObject choicesPanel;
    //Large panel linked to editor showing options for the player to choose
    public GameObject optionsPanel;

    //Clock timer showing time remaining for player selection linked to editor
    public Image timerFill;

    //Link to a green tick image
    public Sprite correctSprite;
    //Link to a red cross image
    public Sprite incorrectSprite;

    //Prefab for the mobile image showing a picture of the object
    public GameObject conveyedObjectPrefab;

    //Link to the spawn point for our object off the right edge of the screen from our conveyor
    public RectTransform objectStartPosition;

    //Links to the buttons the player will use to select
    public OptionButtonConveyor[] optionsArray;

    //Links to the image holders for player choice, set in editor in a horizontal grid layout
    public Image[] choicesBaseArray;
    //Links to the images to output player selections
    public Image[] choicesImageArray;

    //Simple state system to manage gameflow
    private enum eGameState
    {
        SHOWCASE,   //Objects move slowly along the conveyor belt for players to memorise
        GUESS,      //Players make their selections from memory
        REVIEW      //Objects move quickly along the conveyor belt to compare to player choices
    }
    private eGameState m_CurrentState;

    //Simple struct to hold associated vocabulary resources
    private class ObjectData
    {
        public Sprite picture;
        public AudioClip english_audio;
        public AudioClip chinese_audio;
    }
    private List<ObjectData> m_VocabData = new List<ObjectData>();

    //Class to handle object movement on the conveyor belt
    private ConveyerShowcase m_Showcase = new ConveyerShowcase();

    //Index pointer to the current round of the game
    private int m_CurrentRound;
    //Each round will alternate between both languages (English and Chinese)
    private bool m_RoundIsEnglish = true;

    //Time allowed for guessing this object in this round (in seconds)
    private float m_CurrentTime;
    //Max time allowed for guesses which scales down based on difficulty setting (in seconds)
    private float m_MaxTime;

    //Array to hold the indices for vocabulary, fills up as players guess 
    private int[] m_GuessesArray;
    //Number of guesses made this round
    private int m_GuessesMade;

    //Number of points earned from each correct selection
    private float m_CorrectAnswerScore;

    //Simple float which scales up as game difficulty increases.
    //Difficulty will affect object speed on the conveyor and time allowed to make a guess.
    private float m_DifficultyFactor;

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected override void LoadVocabulary()
    {
        //For each round we are going to complete
        for (int i = 0; i < MAX_ROUNDS; i++)
        {
            //Load word from consistent game manager 
            string word = m_GameManager.RecycleVocabulary();

            //Create data structure and load it with resources for this piece of vocabulary
            ObjectData data = new ObjectData();
            data.picture = Resources.Load<Sprite>("Images/" + word);
            data.english_audio = Resources.Load<AudioClip>("English Spoken/" + word);
            data.chinese_audio = Resources.Load<AudioClip>("Chinese Spoken/" + word);

            //Add structure to our list for this game
            m_VocabData.Add(data);
        }
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected override void SetupGame()
    {
        //Increase the difficulty based on the difficulty stored in the game manager's game state
        m_DifficultyFactor = 1f + (m_Difficulty * 0.01f);

        //We begin with a showcase
        m_CurrentState = eGameState.SHOWCASE;

        //Create a list of images to load into our showcase instance
        List<Sprite> ordered_pictures = new List<Sprite>();
        foreach (ObjectData od in m_VocabData)
            ordered_pictures.Add(od.picture);

        m_Showcase.InitialiseShowcase(conveyedObjectPrefab, ordered_pictures, this, objectStartPosition);

        //Create our list of guess with number of elements equal to the last round
        m_GuessesArray = new int[MAX_ROUNDS];

        //Deactivate player input panels in preparation for showcase
        choicesPanel.SetActive(false);
        optionsPanel.SetActive(false);

        //Set initial values for start of game
        m_GuessesMade = 0;
        m_CurrentRound = 0;
        m_MaxTime = BASE_TIME / m_DifficultyFactor; //time for guessing decreases as difficulty increases

        //Calculate the sum of positive integers between 1 and our number of rounds. 
        //The inverse will be our progress score for each correct answer.
        //Therefore, all correct answers will result in a score of 1.
        m_CorrectAnswerScore = 1 / ((MAX_ROUNDS * (MAX_ROUNDS + 1) * 0.5f));
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected override void UpdateGame(float dt)
    {
        //Simply call the appropriate update function for the current state of the game
        switch (m_CurrentState)
        {
            case eGameState.GUESS:
                {
                    UpdateGuesses(dt);
                    break;
                }
            case eGameState.REVIEW:
                {
                    UpdateReview(dt);
                    break;
                }
            case eGameState.SHOWCASE:
                {
                    UpdateShowcase(dt);
                    break;
                }
        }
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected override void SetThresholds()
    {
        //Incremental game, start at zero and gain points for correct answers
        //Reward thresholds set at 30%, 50% and 70%.
        progressPanel.SetStartValues(0.0f, 0.3f, 0.5f, 0.7f);
    }

    /*
    Function to play audio for a piece of vocabulary, called when the vocab's picture passes the midpoint of the screen.
    During review this can also evaluate the player's guesses.
    */
    public void PlayAudio(int object_index)
    {
        if (m_CurrentState == eGameState.SHOWCASE)
        {   //During our showcase, simply play the audio in the current rounds language
            if (m_RoundIsEnglish)
                m_AudioSource.PlayOneShot(m_VocabData[object_index].english_audio);
            else m_AudioSource.PlayOneShot(m_VocabData[object_index].chinese_audio);
        }
        else if (m_CurrentState == eGameState.REVIEW)
        {
            //During the review, check if the player's guess was right
            if (m_GuessesArray[m_GuessesMade] == m_GuessesMade) //Check guess made against index, since vocab will progress incrementally
            {   //If guess was right, play a success audio clip, add progress points and change the image to a green tick.
                choicesImageArray[m_GuessesMade].sprite = correctSprite;
                progressPanel.UpdateProgress(m_CorrectAnswerScore);
                m_AudioSource.PlayOneShot(m_CorrectAudio);
            }
            else
            {   //If guess was wrong, change image to a red cross
                choicesImageArray[m_GuessesMade].sprite = incorrectSprite;
            }
            m_GuessesMade++; //Increment in preparation for the next evaluation
        }
    }

    /*
    Function linked to the buttons in the editor and called when they are clicked
    Button will provide the index for the picture displayed on said button
    */
    public void MakeSelection(int button_index)
    {
        //Save our guess into the array
        m_GuessesArray[m_GuessesMade] = button_index;

        //Reset the time for the next guess
        m_CurrentTime = m_MaxTime;

        //Load our new selection into the player choices bar to show what the player has already chosen this round
        LoadChoices();

        if (m_GuessesMade < m_CurrentRound)
        {   //If there are more objects to be remembered, increment number of guesses and load some new images for the player to choose from
            m_GuessesMade++;
            LoadOptionImages();
        }
        else
        {   //If we've reached the end of the round, go into the review state, deactivate the user input panel and reset number of guesses
            m_CurrentState = eGameState.REVIEW;
            m_GuessesMade = 0;
            optionsPanel.SetActive(false);
        }
    }

    /*
    Function called to erase all choices made by the player to prepare for a new round
    */
    private void ResetChoices()
    {
        m_GuessesMade = 0;
        for (int i = 0; i < m_GuessesArray.Length; i++)
            m_GuessesArray[i] = -1; //Index of first element in any array is 0, so use negative numbers to indicate that no answer was given
    }

    /*
    Function to display the choices already made by the player in this round
    */
    private void LoadChoices()
    {
        for (int i = 0; i < MAX_ROUNDS; i++)
        {   //For each displayable image, check which round we are currently in and how many boxes we need.
            if (i <= m_CurrentRound) 
            {
                choicesBaseArray[i].gameObject.SetActive(true);
                //If the contents of the guesses array is positive a guess was made, so display the appropriate picture
                if (m_GuessesArray[i] >= 0)
                    choicesImageArray[i].sprite = m_VocabData[m_GuessesArray[i]].picture;
                else choicesImageArray[i].sprite = null;//If the contents of the guesses array is negative no guess was made, so display a blank image
            }
            else choicesBaseArray[i].gameObject.SetActive(false); //Disable any unneeded boxes
        }
    }

    /*
    Function to display four images for the player to choose from, one of which is the correct answer
    */
    private void LoadOptionImages()
    {
        //Create a shuffle list to load our elements into
        List<int> shuffle = new List<int>();
        //Pictures are shown in sequential order, so the number of guesses made will equal the index of the next correct answer
        shuffle.Add(m_GuessesMade);
        //To avoid duplicate images, use some random ranges added to the correct answer to generate three wrong answers. Use 'mod MAX_ROUNDS' to loop back to the front of the list.
        shuffle.Add((m_GuessesMade + Random.Range(1, 3)) % MAX_ROUNDS);
        shuffle.Add((m_GuessesMade + Random.Range(3, 5)) % MAX_ROUNDS);
        shuffle.Add((m_GuessesMade + Random.Range(5, MAX_ROUNDS)) % MAX_ROUNDS);

        //Remove random elements from the shuffle list and place them into selectable buttons
        int random_int;
        random_int = shuffle[Random.Range(0, shuffle.Count)];
        optionsArray[0].Load(this, m_VocabData[random_int].picture, random_int);
        shuffle.Remove(random_int);
        random_int = shuffle[Random.Range(0, shuffle.Count)];
        optionsArray[1].Load(this, m_VocabData[random_int].picture, random_int);
        shuffle.Remove(random_int);
        random_int = shuffle[Random.Range(0, shuffle.Count)];
        optionsArray[2].Load(this, m_VocabData[random_int].picture, random_int);
        shuffle.Remove(random_int);
        random_int = shuffle[Random.Range(0, shuffle.Count)];
        optionsArray[3].Load(this, m_VocabData[random_int].picture, random_int);
    }

    /*
    The update function to show frame-by-frame changes during the SHOWCASE state of the game.
    SHOWCASE will be the starting state of the game, otherwise it will always follow the REVIEW state.
    */
    private void UpdateShowcase(float dt)
    {
        //If showcase has not started yet
        if (m_Showcase.ReadyToStart())
        {
            if (m_CurrentRound < MAX_ROUNDS) //Game is not finished yet, start the showcase
                m_Showcase.StartShowcase(m_CurrentRound, m_DifficultyFactor);
            else    //Final round has been reviewed, so end the game
                EndGame();
        }
        else if (m_Showcase.HasEnded())
        {   //If the showcase has ended, switch to the GUESS state
            Debug.Log("switching to guesses");
            m_CurrentState = eGameState.GUESS;

            ResetChoices(); //Erase any choices from any previous rounds

            //Prepare the timer for player guesses
            m_CurrentTime = m_MaxTime;

            //Activate and load player input and feedback panels
            choicesPanel.SetActive(true);
            optionsPanel.SetActive(true);
            LoadOptionImages();
            LoadChoices();

            //Reset showcase in preparation for the review phase
            m_Showcase.ResetShowcase();
        }
        else
        {
            //If showcase is ongoing, keep updating it
            m_Showcase.UpdateShowcase(dt);
        }
    }

    /*
    The update function to show frame-by-frame changes during the GUESS state of the game
    GUESS will always follow the SHOWCASE state.
    */
    private void UpdateGuesses(float dt)
    {
        //Decrease time remaining, adjust in-editor clock to match, and add a blank selection if time is up
        m_CurrentTime -= dt;
        if (m_CurrentTime > 0f)
            timerFill.fillAmount = m_CurrentTime / m_MaxTime;
        else MakeSelection(-1); //Timeout
    }

    /*
    The update function to show frame-by-frame changes during the REVIEW state of the game
    REVIEW will always follow the GUESS state.
    */
    private void UpdateReview(float dt)
    {
        //If review has not been started yet
        if (m_Showcase.ReadyToStart())
        {
            Debug.Log("Starting review " + m_CurrentRound);
            m_Showcase.StartShowcase(m_CurrentRound, m_DifficultyFactor * 2f); //Review showcase will be twice as fast
        }
        else if (m_Showcase.HasEnded())
        {
            //If review has ended
            Debug.Log("switching to showcase");
            //Reset showcase in preparation for the SHOWCASE state
            m_Showcase.ResetShowcase(); 
            
            //Switch to showcase state
            m_CurrentState = eGameState.SHOWCASE;

            //Go to the next round
            m_CurrentRound++;

            //Switch the language for the next round
            m_RoundIsEnglish = !m_RoundIsEnglish;

            //Deactivate the player feedback panel
            choicesPanel.SetActive(false);

            //Start the general countdown used for all games before each showcase
            if (m_CurrentRound < MAX_ROUNDS)
            {
                m_IsStarting = true;
                m_Countdown = 3.0f;
                introPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            //If review is ongoing, keep updating it
            m_Showcase.UpdateShowcase(dt);
        }        
    }
}
