using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
UI class attached to a prebuilt video menu.
Class should show number of stars available to be spent, and a selection of videos to choose from.
*/
public class VideoSelectionMenu : MonoBehaviour
{
    //How many choices are available for each level of video. 
    private const int NUMBER_OF_VIDEO_CHOICES = 3;

    //Showcase for bronze videos (links to editor monoobjects to be modified)
    public TextMeshProUGUI bronzeStarCounter;
    public GameObject bronzeVideoPanel;
    public Image[] bronzeButtons;
    public GameObject bronzePlaceholderPanel;

    //Showcase for silver videos (links to editor monoobjects to be modified)
    public TextMeshProUGUI silverStarCounter;
    public GameObject silverVideoPanel;
    public Image[] silverButtons;
    public GameObject silverPlaceholderPanel;

    //Showcase for gold videos (links to editor monoobjects to be modified)
    public TextMeshProUGUI goldStarCounter;
    public GameObject goldVideoPanel;
    public Image[] goldButtons;
    public GameObject goldPlaceholderPanel;

    //pointer to the game manager, which handle the actual playing and management of videos and scene changes
    private GameManager m_GameManager;

    /*
    On menu selection, update the current numbers of stars and show either a collection of videos (if player has appropriate stars) 
    or a placeholder (if they don't have any stars of that level).
    NOTE: indexes for the video choices are stored in the buttons on the panel, and handled in editor.
    */
    void Start()
    {
        Debug.Log("UNITY: Starting video select menu");

        //Get the game manager and store it (should have been initialised on application start)
        GameObject obj1 = GameObject.Find("Game Manager");
        if (obj1 != null)
        {
            GameManager gm = obj1.GetComponent<GameManager>();
            if (gm != null)
            {
                m_GameManager = gm;
                Debug.Log("UNITY: Found game manager " + gm);

                bronzeStarCounter.text = m_GameManager.m_BronzeStars.ToString();
                if (m_GameManager.m_BronzeStars > 0) //if player has bronze stars, populate the panel with choices (in the form of buttons)
                {
                    bronzeVideoPanel.SetActive(true);
                    for (int i = 0; i < NUMBER_OF_VIDEO_CHOICES; i++)
                    {
                        bronzeButtons[i].sprite = m_GameManager.m_BronzeVideoClips[i].thumbnail;
                    }
                    bronzePlaceholderPanel.SetActive(false);
                }
                else // if they don't have stars, show an encouraging message
                {
                    bronzeVideoPanel.SetActive(false);
                    bronzePlaceholderPanel.SetActive(true);
                }

                silverStarCounter.text = m_GameManager.m_SilverStars.ToString();
                if (m_GameManager.m_SilverStars > 0) //if player has silver stars, populate the panel with choices (in the form of buttons)
                {
                    silverVideoPanel.SetActive(true);
                    for (int i = 0; i < NUMBER_OF_VIDEO_CHOICES; i++)
                    {
                        silverButtons[i].sprite = m_GameManager.m_SilverVideoClips[i].thumbnail;
                    }
                    silverPlaceholderPanel.SetActive(false);
                }
                else // if they don't have stars, show an encouraging message
                {
                    silverVideoPanel.SetActive(false);
                    silverPlaceholderPanel.SetActive(true);
                }

                goldStarCounter.text = m_GameManager.m_GoldStars.ToString();
                if (m_GameManager.m_GoldStars > 0) //if player has gold stars, populate the panel with choices (in the form of buttons)
                {
                    goldVideoPanel.SetActive(true);
                    for (int i = 0; i < NUMBER_OF_VIDEO_CHOICES; i++)
                    {
                        goldButtons[i].sprite = m_GameManager.m_GoldVideoClips[i].thumbnail;
                    }
                    goldPlaceholderPanel.SetActive(false);
                }
                else // if they don't have stars, show an encouraging message
                {
                    goldVideoPanel.SetActive(false);
                    goldPlaceholderPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("UNITY: Game manager object doesn't have GameManager component");
            }
        }
        else
        {
            Debug.Log("UNITY: Could not find game manager");
        }
    }

    /*
    Simple functions called by the appropriate buttons on click. Indexes are stored in the buttons and modifiable in the editor.
    */
    public void PlayBronzeVideo(int index) { m_GameManager.PlayBronzeVideo(index); }
    public void PlaySilverVideo(int index) { m_GameManager.PlaySilverVideo(index); }
    public void PlayGoldVideo(int index) { m_GameManager.PlayGoldVideo(index); }
}
