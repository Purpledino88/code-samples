using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Script to attach to a prefab of a playing card which shows an image.
Prefab will contain a button with functions linked in the editor.
*/
public class PairedCard : MonoBehaviour
{
    //Image links set in the editor
    public Image image;

    //Static sprite of the back of the card, linked in the editor/prefab
    public Sprite cardBack;

    //Sprite for this card's image, set on construction
    private Sprite m_VocabImage;

    //Manager for this game
    private MatchingPairsManager m_GameManager;

    //Spoken pronunciation of this piece of vocabulary, set on construction
    private AudioClip m_ChineseAudio;
    private AudioClip m_EnglishAudio;

    //String for this word, supplied on construction and used to load other private variables
    private string m_VocabWord;
    //Returns the string for this word, used by game manager for comparison
    public string GetID() { return m_VocabWord; }

    //Boolean showing whether this card is face-up or face-down
    private bool m_IsSelected;

    /*
    'Constructor' for this class, called by the game manager on creation.
    Load all required resources and place card face-downn.
    */
    public void Load(MatchingPairsManager mpm, string vocab)
    {
        m_GameManager = mpm;
        m_VocabWord = vocab;
        m_IsSelected = false;

        m_VocabImage = Resources.Load<Sprite>("Images/" + m_VocabWord);
        m_ChineseAudio = Resources.Load<AudioClip>("Chinese Spoken/" + m_VocabWord);
        m_EnglishAudio = Resources.Load<AudioClip>("English Spoken/" + m_VocabWord);

        image.sprite = cardBack;
    }

    /*
    Called on prefab button press, show the card and warn game manager that this card has been chosen.
    */
    public void OnSelect()
    {
        if (!m_IsSelected && !m_GameManager.IsLocked() && m_GameManager.IsPlayable()) //game manager may lock the selections to prevent double pushes
        {
            image.sprite = m_VocabImage;    //Show face-up image
            m_IsSelected = true; //Prevent being selected again
            m_GameManager.SelectCard(this); //Inform game manager
        }
    }

    /*
    Function will be called by game manager if two selected cards do not match.
    */
    public void OnDeselect()
    {
        image.sprite = cardBack;    //Show face-down card
        m_IsSelected = false;       //Allow being selected again
    }

    /*
    Functions to play pronunciation on selection using game manager's audio source.
    Game manager will choose which audio to play based on game state.
    */
    public void PlayEnglishAudio(AudioSource source) { source.PlayOneShot(m_EnglishAudio); }
    public void PlayChineseAudio(AudioSource source) { source.PlayOneShot(m_ChineseAudio); }
}
