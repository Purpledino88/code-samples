using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*
Class applied to each panel of a game in the 'Games' submenu of the main menu.
Panels are created as a prefab, allowing name and scene changes in editor to load games.
Displays information relevent to this game, and allows the option to begin play.
*/
public class GamePanelMarker : MonoBehaviour
{
    //Short name of the game, used to check today's difficulty and achievements
    public string m_GameName;

    //Which scene to load if we want to play this game
    public string sceneName;

    //Images representing stars available to get today, linked in editor
    public Image bronzeStar;
    public Image silverStar;
    public Image goldStar;

    //Shows the current difficulty of the game
    public TextMeshProUGUI difficultyTextBox;

    //Holds the Game Manager and current Game State which are required across multiple functions
    private GameManager m_GameManager = null;
    private GameStateStructure m_GameState = null;

    void Start()
    {
        Debug.Log("UNITY: Loading game panel for " + m_GameName);

        //Find the game manager, which should have been created at application start
        GameObject obj = GameObject.Find("Game Manager");
        if (obj != null)
        {
            GameManager gm = obj.GetComponent<GameManager>();
            if (gm != null)
            {
                m_GameManager = gm;

                //Find the gamestate related to this game from the manager
                foreach (GameStateStructure gss in m_GameManager.m_CurrentStates)
                {
                    if (gss.name == m_GameName)
                    {
                        m_GameState = gss;

                        //Update the panel to show accurate difficulty
                        difficultyTextBox.text = m_GameState.difficulty.ToString();

                        //Show stars available depending on how many have already been earned today.
                        //Star progression will always be bronze->silver->gold. It's impossible to earn gold without already having earned silver and bronze.
                        switch (m_GameState.stars_earned)
                        {
                            case 3:
                                {
                                    goldStar.gameObject.SetActive(false);
                                    silverStar.gameObject.SetActive(false);
                                    bronzeStar.gameObject.SetActive(false);
                                    break;
                                }
                            case 2:
                                {
                                    goldStar.gameObject.SetActive(true);
                                    silverStar.gameObject.SetActive(false);
                                    bronzeStar.gameObject.SetActive(false);
                                    break;
                                }
                            case 1:
                                {
                                    goldStar.gameObject.SetActive(true);
                                    silverStar.gameObject.SetActive(true);
                                    bronzeStar.gameObject.SetActive(false);
                                    break;
                                }
                            default:
                                {
                                    goldStar.gameObject.SetActive(true);
                                    silverStar.gameObject.SetActive(true);
                                    bronzeStar.gameObject.SetActive(true);
                                    break;
                                }
                        }
                    }
                }

                //Debug statements if relevant info cannot be found
                if (m_GameState == null)
                {
                    Debug.Log("Could not find GameStateStructure with name " + m_GameName);
                }
                Debug.Log("UNITY: -----" + m_GameName + " panel loaded");
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
    Simple function to load the scene related to this panel. Connected to the 'Play' button in the editor.
    Game state will be passed to Game Manager which is not destroyed on scene change, and modified at the end of the game.
    */
    public void BeginGame()
    {
        m_GameManager.m_GameInProgress = m_GameState;
        SceneManager.LoadScene(sceneName);
    }
}
