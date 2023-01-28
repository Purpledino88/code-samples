using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Script attached to our vocabulary menu, allowing player to review allowable vocabulary outside of games.
This menu will create a VocabReviewButton instance for every word in our application.
If these buttons call for more information, writing and audio clips for that word will be displayed in a popup.
*/
public class VocabReviewMenu : MonoBehaviour
{
    //Prefab for the individual vocabulary buttons, which will be images
    public VocabReviewButton vocabButtonPrefab;

    //Pointers to our popup display's gameObjects
    public GameObject detailsPopup;
    public Image detailsImage;
    public Image detailsChinese; //This will be an image on a button which calls PlayChineseAudio on click
    public Image detailsEnglish; //This will be an image on a button which calls PlayEnglishAudio on click

    //Handlers for audio clips
    private AudioSource m_AudioSource;
    private AudioClip m_ChineseAudio;
    private AudioClip m_EnglishAudio;

    //Layout our buttons in a grid panel
    public GridLayoutGroup gridPanel;

    /*
    On loading the menu, instantiate all of our vocabulary buttons into our grid panel
    */
    void Start()
    {
        GameObject obj = GameObject.Find("Game Manager"); //ordered vocabulary list is stored in the game manager
        if (obj != null)
        {
            GameManager gm = obj.GetComponent<GameManager>();
            if (gm != null)
            {
                m_AudioSource = GetComponent<AudioSource>(); //audio source is vocab irrelevant and unchangeable so get it once and store it

                foreach (string s in gm.m_VocabularyList)
                {
                    VocabReviewButton word_button = Instantiate(vocabButtonPrefab); //create button from prefab
                    word_button.SetVocab(s, this);  //supply word and pointer to this menu
                    word_button.transform.SetParent(gridPanel.transform, false); //let grid panel handle the location
                }
            }
            else
            {
                Debug.Log("Game manager object doesn't have GameManager component");
            }
        }
        else
        {
            Debug.Log("Could not find game manager");
        }
    }

    /*
    Function to make the popup window visible and load necessary resources.
    Disabling the popup window is handled in the editor via an exit button which sets the popup to inactive.
    */
    public void ShowVocabDetails(string word)
    {
        //show popup
        detailsPopup.SetActive(true);

        //load resources 
        detailsImage.sprite = Resources.Load<Sprite>("Images/" + word);
        m_ChineseAudio = Resources.Load<AudioClip>("Chinese Spoken/" + word);
        m_EnglishAudio = Resources.Load<AudioClip>("English Spoken/" + word);
        detailsChinese.sprite = Resources.Load<Sprite>("Chinese Written/" + word);
        detailsEnglish.sprite = Resources.Load<Sprite>("English Written/" + word);
    }

    //Simple functions to play relevant audio when images are clicked
    public void PlayEnglishAudio() { m_AudioSource.PlayOneShot(m_EnglishAudio); }
    public void PlayChineseAudio() { m_AudioSource.PlayOneShot(m_ChineseAudio); }
}
