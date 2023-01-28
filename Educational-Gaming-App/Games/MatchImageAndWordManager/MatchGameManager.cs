using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
The specific game manager for the 'Match Picture and Word' language game.
This game features multiple rounds, requiring players to match pictures with the correct words.
Each round may be:
    - match a picture to the correct word out of four choices
    - match a written word to the correct picture (again, four choices)
Rewards are determined by speed, with incorrect choices reducing potential gains.
NOTE: This class is abstract and intended to be overloaded for each language.
*/
public abstract class MatchGameManager : SpecificManager
{
    //Every 20 points of difficulty is one round of difficulty
    //Difficulty may be additional words or less forgiving timers
    private const int DIFFICULTY_ROUND = 20;

    //Seperate panels created in the editor, one for words and one for pictures
    public MatchOptionPanel matchWordPanel;
    public MatchOptionPanel matchPicturePanel;

    //Circular 'clock' timer to show time remaining, linked from editor
    public Image timerFill;

    //Time variables for time per round
    private float m_CurrentTime;
    private float m_MaxTime;

    //Potential score to be earned each round (usually 1f / number of rounds)
    private float m_PotentialScore;
    //Punishment for making an incorrect guess, dividing potential score by this amount
    private float m_IncorrectDivider;

    //Boolean flag to show when game has started
    private bool m_HasStarted = false;

    //Internal struct to hold vocab details (since this game will be monoligual, we can just store one audioclip)
    protected class WordData
    {
        public Sprite picture;
        public Sprite word;
        public AudioClip audio;
    }
    private List<WordData> m_VocabData = new List<WordData>();

    //Rounds will be precalculated to ensure each pice of vocabulary is used an equal number of times
    private class Round
    {
        public int master;
        public bool isWordMaster;   //Flag showing true (one word/four pictures) or false (one picture/four words)
        public int option1;
        public int option2;
        public int option3;
        public int option4;
    }
    private List<Round> m_Rounds = new List<Round>();

    //Index of current round in above list
    private int m_CurrentRound;

    /*
    Abstract function to load language appropriate audio and images of written words
    Will be overloaded in two subclasses (English and Chinese)
    */
    protected abstract void LoadAudioAndWord(string word, WordData wd);

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected override void LoadVocabulary()
    {
        Debug.Log("UNITY - Loading vocabulary");
        //Calculate number of words and available time based on the difficulty
        int number_of_words = (m_Difficulty / 10) + 4;
        m_IncorrectDivider = 2 + (m_Difficulty % 10);

        for (int i = 0; i < number_of_words; i++)
        {
            string word = m_GameManager.RecycleVocabulary();    //Get vocab from consistent game manager

            //Create word data structure with picture and subclass specific writing/pronunciation
            WordData data = new WordData();
            data.picture = Resources.Load<Sprite>("Images/" + word);
            LoadAudioAndWord(word, data);

            //Add word data to local list
            m_VocabData.Add(data);
        }
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected override void SetupGame()
    {
        Debug.Log("UNITY - Setting up game");

        //For each piece of vocabulary, set up one round with a master picture and one round with a master written word into the variable list
        for (int i = 0; i < m_VocabData.Count; i++)
        {
            SetupRound(i, true);
            SetupRound(i, false);
        }

        //Create a second list to shuffle our rounds
        List<Round> shuffle = new List<Round>();

        //Remove a random round from our variable list and put it into the shuffle list
        while (m_Rounds.Count > 0)
        {
            Round r = m_Rounds[Random.Range(0, m_Rounds.Count)];
            shuffle.Add(r);
            m_Rounds.Remove(r);
        }


        //Remove a random round from our shuffle list and put it back into the variable list
        while (shuffle.Count > 0)
        {
            Round r = shuffle[Random.Range(0, shuffle.Count)];
            m_Rounds.Add(r);
            shuffle.Remove(r);
        }

        //Scale time for guesses down based on difficulty (measured in seconds)
        m_MaxTime = 15.0f - (0.5f * (m_Difficulty % 10));

        //Set pointer to the first index of the round list and deactivate both panels as we don't know which will be used first
        m_CurrentRound = 0;
        matchWordPanel.gameObject.SetActive(false);
        matchPicturePanel.gameObject.SetActive(false);
    }

    /*
    Function called during the SetupGame function for each round.
    Master index is the correct picture/word to be shown.
    Master word will be true if the correct word to be matched with a picture, or false if there is a correct picture to be matched with one of four words.
    */
    private void SetupRound(int master_index, bool master_word)
    {
        Round r = new Round();
        r.master = master_index;
        List<int> options_list = new List<int>(); //Create an ordered list of options (to be shuffled later)
        options_list.Add(master_index); //Add correct answer to our list of choices

        if (master_word)
        {
            r.isWordMaster = true;
            for (int i = 1; i < 4; i++) //Iterate 1 -> 3 to match indexes for options list of size 4
                options_list.Add((master_index + i) % m_VocabData.Count);   //Add the next three pictures in our vocabulary list
        }
        else
        {
            r.isWordMaster = false;
            for (int i = 1; i < 4; i++) //Iterate 1 -> 3 to match indexes for options list of size 4
                options_list.Add((m_VocabData.Count + master_index - i) % m_VocabData.Count); //Add the previous three words in our vocabulary list
        }

        //Shuffle our choices into the round's member list from our local ordered list
        r.option1 = options_list[Random.Range(0, options_list.Count)];
        options_list.Remove(r.option1);
        r.option2 = options_list[Random.Range(0, options_list.Count)];
        options_list.Remove(r.option2);
        r.option3 = options_list[Random.Range(0, options_list.Count)];
        options_list.Remove(r.option3);
        r.option4 = options_list[Random.Range(0, options_list.Count)];
        options_list.Remove(r.option4);

        //Add the round to the variable list
        m_Rounds.Add(r);
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected override void UpdateGame(float dt)
    {
        if (m_HasStarted) 
        {
            m_CurrentTime -= dt;
            //If we still have time in this round, modify the timer clock to show remaining time. Otherwise, load the next round.
            if (m_CurrentTime > 0f)
                timerFill.fillAmount = m_CurrentTime / m_MaxTime;
            else LoadRound(m_CurrentRound + 1); //out-of-bounds is handled in the LoadRound function
        }
        else
        {
            //Game has not started yet, so load the first round and change this flag to true
            LoadRound(0);
            m_HasStarted = true;
        }
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected override void SetThresholds()
    {
        //Incremental game, start from zero and gain points based on speed of answers
        //Bronze star at 30%, silver at 50%, gold at 70%
        progressPanel.SetStartValues(0.0f, 0.3f, 0.5f, 0.7f);
    }

    /*
    Function to update our progress based on speed of answers, with lower difficulties being more forgiving
    At zero difficulty, speed weight becomes 0 and only correctness is important (correct answers give full potential score).
    As difficulty trends to DIFFICULTY_ROUND, speed is given a higher weight and will affect the score more.
    */
    private void UpdateScore()
    {
        Debug.Log("UNITY - Updating score");
        float speed_weight = (m_Difficulty % DIFFICULTY_ROUND) / (DIFFICULTY_ROUND * 2f); //Weight to give answer speed (0.0 to 0.5), score for being correct will be 1.0 to 0.5.
        float speed_bonus = (m_CurrentTime / m_MaxTime) * speed_weight; //Speed score
        float score = (m_PotentialScore * (1 - speed_weight)) + (speed_weight * speed_bonus); //Correctness score + Speed score

        //Update the progress panel with our score for this round
        //Remember, progress is marked from 0 to 1, so each round will give 1/number of rounds as a maximum score
        progressPanel.UpdateProgress(score / m_Rounds.Count);
    }

    /*
    Function to load the next round (either at game start or after timeout/correct guess) or end the game after this final round
    */
    private void LoadRound(int round_index)
    {
        Debug.Log("UNITY - Loading round " + round_index);
        //Reset time and potential score
        m_CurrentTime = m_MaxTime; 
        m_PotentialScore = 1f;

        //Deactivate panels until we know which will be needed next
        matchWordPanel.gameObject.SetActive(false);
        matchPicturePanel.gameObject.SetActive(false);

        //Update variable to match new index
        m_CurrentRound = round_index;

        //If we still have rounds to go
        if (m_CurrentRound < m_Rounds.Count)
        {
            Round round = m_Rounds[m_CurrentRound];
            if (round.isWordMaster)
            {   //1 word/4 pictures: Load the matching word panel and activate it
                matchWordPanel.Load(m_VocabData[round.master].word, m_VocabData[round.option1].picture, m_VocabData[round.option2].picture, m_VocabData[round.option3].picture, m_VocabData[round.option4].picture);
                matchWordPanel.gameObject.SetActive(true);
            }
            else
            {   //1 picture/4 words: Load the matching picture panel and activate it
                matchPicturePanel.Load(m_VocabData[round.master].picture, m_VocabData[round.option1].word, m_VocabData[round.option2].word, m_VocabData[round.option3].word, m_VocabData[round.option4].word);
                matchPicturePanel.gameObject.SetActive(true);
            }
        }
        else //If we have finished all of the rounds
        {
            EndGame();
        }
    }

    /*
    Function to be called by the buttons from our editor on click
    */
    public void MakeSelection(int button_index)
    {
        Round round = m_Rounds[m_CurrentRound]; //Get data for this round

        switch (button_index)
        {
            case 0:
                {
                    m_AudioSource.PlayOneShot(m_VocabData[round.option1].audio); //Play appropriate pronunciation for this rounds first choice
                    if (round.option1 == round.master)  //If index of the first choice matches the target index
                    {
                        //Update the game score, load the next round and play our victory noise
                        UpdateScore(); 
                        LoadRound(m_CurrentRound + 1);
                        m_AudioSource.PlayOneShot(m_CorrectAudio);
                    }
                    else m_PotentialScore /= m_IncorrectDivider; //If it doesn't match, reduce the potential score for this round
                    break;
                }
            case 1:
                {
                    m_AudioSource.PlayOneShot(m_VocabData[round.option2].audio); //Play appropriate pronunciation for this rounds second choice
                    if (round.option2 == round.master)  //If index of the second choice matches the target index
                    {
                        //Update the game score, load the next round and play our victory noise
                        UpdateScore();
                        LoadRound(m_CurrentRound + 1);
                        m_AudioSource.PlayOneShot(m_CorrectAudio);
                    }
                    else m_PotentialScore /= m_IncorrectDivider; //If it doesn't match, reduce the potential score for this round
                    break;
                }
            case 2:
                {
                    m_AudioSource.PlayOneShot(m_VocabData[round.option3].audio); //Play appropriate pronunciation for this rounds third choice
                    if (round.option3 == round.master)  //If index of the third choice matches the target index
                    {
                        //Update the game score, load the next round and play our victory noise
                        UpdateScore();
                        LoadRound(m_CurrentRound + 1);
                        m_AudioSource.PlayOneShot(m_CorrectAudio);
                    }
                    else m_PotentialScore /= m_IncorrectDivider; //If it doesn't match, reduce the potential score for this round
                    break;
                }
            case 3:
                {
                    m_AudioSource.PlayOneShot(m_VocabData[round.option4].audio); //Play appropriate pronunciation for this rounds fourth choice
                    if (round.option4 == round.master)  //If index of the fourth choice matches the target index
                    {
                        //Update the game score, load the next round and play our victory noise
                        UpdateScore();
                        LoadRound(m_CurrentRound + 1);
                        m_AudioSource.PlayOneShot(m_CorrectAudio);
                    }
                    else m_PotentialScore /= m_IncorrectDivider; //If it doesn't match, reduce the potential score for this round
                    break;
                }
        }
    }
}
