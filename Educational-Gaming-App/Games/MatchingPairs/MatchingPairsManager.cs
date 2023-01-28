using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
The specific game manager for the 'Matching Pairs' memory game.
This game features a shuffled array of paired face-down cards which players must match.
Players must match all pairs before time is up. Speed and accuracy determine rewards achieved.
*/
public class MatchingPairsManager : SpecificManager
{
    //Prefab object for our card
    public PairedCard cardPrefab;

    //Arrange cards in a grid layout
    public GridLayoutGroup gridPanel;

    //Lock all selections after choosing a card, to prevent double selections or 'button-bashing' (encourage thoughtful choices not spam-selecting every card)
    private float m_Lock;
    public bool IsLocked() { return m_Lock > 0.0f; }

    //Counter of remaining used to determine when game is complete
    private int m_CardsRemaining;

    //Holders for our cards to enable comparison after every two selections
    private PairedCard m_FirstCard = null;
    private PairedCard m_SecondCard = null;

    //Variables used in our difficulty and reward calculations
    private float m_MaxTime;
    private float m_MaxAttempts; 
    
    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected override void LoadVocabulary()
    {
        //As games are completed over multiple days, increase the number of cards (starting with four pairs/eight cards)
        int number_of_words = (m_Difficulty / 20) + 4;

        for (int i = 0; i < number_of_words; i++)
        {
            m_VocabList.Add(m_GameManager.RecycleVocabulary());
        }
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected override void SetupGame()
    {
        int number_of_cards = m_VocabList.Count * 2;
        m_CardsRemaining = number_of_cards;

        //Create a list with a pair of cards for each tested word
        List<string> doubled = new List<string>();
        foreach (string s in m_VocabList)
        {
            doubled.Add(s);
            doubled.Add(s);
        }

        //'Shuffle' the previous list by drawing a random element and create a card for that word until the list is empty
        for (int y = 0; y < number_of_cards; y++)
        {
            PairedCard card = Instantiate(cardPrefab);
            card.transform.SetParent(gridPanel.transform, false);

            int r = Random.Range(0, doubled.Count);
            string word = doubled[r];
            card.Load(this, word);
            doubled.RemoveAt(r);
        }

        //Disable the selection lock in preparation for the start of the game
        m_Lock = 0.0f;
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected override void SetThresholds()
    {
        //As difficulty increases decrease the amount of time allowed for the game. 
        //Every 20 points of difficulty will add another pair, so reset to max time.
        m_MaxTime = 90.0f - (m_Difficulty % 20);

        int number_of_cards = m_VocabList.Count * 2;

        //the perfect game would have a worst case of the number of cards used:
        //one pass to view every card, then another pass to match them
        int memorisedAttempts = number_of_cards;

        //an algorithmic game would start with the first card and procede through
        //the remaining cards until it found a match, and then repeat in that fashion.
        //the below iteration represents the worst possible case
        int algorithmicAttempts = 0;
        int i = (number_of_cards - 1);
        while (i > 0)
        {
            algorithmicAttempts += i;
            i -= 2;
        }

        //let us set double the algorithmic worst case to be the maximum number of attempts
        m_MaxAttempts = algorithmicAttempts * 2.0f;

        //a gold game can be the average of the algorithmic worst case and the worst perfect game
        float gold_attempts = (algorithmicAttempts + memorisedAttempts) / 2;
        //convert that to an inverted 0 to 1 float (since player will start at 1.0f and deprecate) 
        float gold_threshold = 1 - (gold_attempts / m_MaxAttempts);

        //a silver game can be half of a gold game, and bronze is simply completing the game before timeout
        float silver_threshold = gold_threshold / 2;
        float bronze_threshold = 0.0f;

        //start the game at full progress, and progress will decrease based on time and incorrect matchings
        progressPanel.SetStartValues(1.0f, bronze_threshold, silver_threshold, gold_threshold);
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected override void UpdateGame(float dt)
    { 
        if (m_Lock > 0.0f) //If cards are locked (player is not allowed to select)
        {
            m_Lock -= dt; //lower the lock based on frame duration

            //if the lock is ended and two cards have been selected, check if they match
            if (m_Lock < 0.0f)
            {
                if ((m_FirstCard != null) && (m_SecondCard != null))
                    EvaluateSelections();
            }
        }
        else    //If cards are unlocked (player is allowed to select)
        {
            //Decrease the progress bar and check if it reads as false (which should occur if the bar reaches zero)
            if (!progressPanel.UpdateProgress(-dt / m_MaxTime))
            {
                EndGame(); //End the game if time is up
            }
        }
    }

    /*
    This function will be called by the PairedCard when their button component has been clicked.
    */
    public void SelectCard(PairedCard c)
    {
        //If no cards have been selected, store this one for future comparisons and play English pronunciation
        if (m_FirstCard == null)
        {
            m_FirstCard = c;
            m_FirstCard.PlayEnglishAudio(m_AudioSource);
        }
        else //If one card has been selected, lock selections and store this second card while playing Chinese pronunciation
        {
            //The lock allows the player to view the cards before evaluation (otherwise an incorrect match would immediately show both card backs)
            //it also gives the audio source chance to finish playing before the correct sound effect plays if the cards do match
            m_Lock = 0.75f; 
            m_SecondCard = c;
            m_SecondCard.PlayChineseAudio(m_AudioSource);            
        }
    }

    /*
    Function called when the selection lock expires and two cards are current selected/face-up
    */
    private void EvaluateSelections()
    {
        if (m_FirstCard.GetID() == m_SecondCard.GetID()) //check if the string IDs of the cards match
        {
            m_CardsRemaining -= 2; //Lower the number of cards left in the game
            m_AudioSource.PlayOneShot(m_CorrectAudio); //Play an audio reward
            if (m_CardsRemaining == 0) //If all cards are matched, end the game
                EndGame();
        }
        else    //if the string IDs don't match, deselect both cards and penalise the players progress for being incorrect, based on our calculated difficulty values
        {
            m_FirstCard.OnDeselect();
            m_SecondCard.OnDeselect();
            progressPanel.UpdateProgress(-1 / m_MaxAttempts);
        }

        //Matched or not, nullify our selected values to allow another attempt
        m_FirstCard = null;
        m_SecondCard = null;
    }    
}
