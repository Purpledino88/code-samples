using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
Simple UI class to be displayed at the end of the game to show which stars have been earned
*/
public class EndgamePanel : MonoBehaviour
{
    //Images set in editor for each possible reward
    public Image bronzeStar;
    public Image silverStar;
    public Image goldStar;

    /*
    Called by SpecificManager on game end.
    */
    public void Activate(GameProgressPanel progress_panel)
    {
        //Show the panel
        this.gameObject.SetActive(true);

        //Check with GameProgressPanel which stars have been achieved and show them
        bronzeStar.gameObject.SetActive(progress_panel.EarnedBronzeStar());
        silverStar.gameObject.SetActive(progress_panel.EarnedSilverStar());
        goldStar.gameObject.SetActive(progress_panel.EarnedGoldStar());
    }

    /*
    Linked to a button on the panel, send us back to the main menu
    */
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
