using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
General UI object to be displayed at the top of each game area, to show current progress/state of the game.
Progress is a general score which may represent time remaining/elapsed, points scored, etc depending on the specific game
*/
public class GameProgressPanel : MonoBehaviour
{
    //Linked to images supplied in the editor for all non-static images 
    public Image bronzeStarImage;
    public Image bronzeStarLine;
    public Image silverStarImage;
    public Image silverStarLine;
    public Image goldStarImage;
    public Image goldStarLine;

    //Slidebar to display the current progress, linked from editor
    public RectTransform modifiableBar;

    //Float to store the current progress in this game so far
    private float m_Progress;

    //Floats to hold the progress required to get a certain level of star on game completion
    private float m_BronzeThreshold;
    private float m_SilverThreshold;
    private float m_GoldThreshold;

    //Booleans to hold whether this game can give a certain level of star.
    //Each level can only be achieved once per day per game, so it may have already been achieved in an earlier playthrough
    private bool m_CanEarnBronze;
    private bool m_CanEarnSilver;
    private bool m_CanEarnGold;

    /*
    Function called before game start to set up the values and position the non-static images correctly on the slider.
    Parameters are the starting value as well as the thresholds for rewards which are set by each specific game.
    */
    public void SetStartValues(float start, float bronze, float silver, float gold)
    {
        m_Progress = start;

        /*
        modifiableBar width is set to 1240 in the editor and starting positions for moveable images are set at the left point of the editor
        Use the thresholds to calculate the true x positions of the images on the modifiableBar slider.
        TODO: Change to get slider width from modifiableBar directly without using magic numbers.
        */
        m_BronzeThreshold = bronze;
        RectTransform brt = bronzeStarLine.GetComponent<RectTransform>();
        brt.anchoredPosition = new Vector2(brt.anchoredPosition.x + (1240 * m_BronzeThreshold), brt.anchoredPosition.y);

        m_SilverThreshold = silver;
        RectTransform srt = silverStarLine.GetComponent<RectTransform>();
        srt.anchoredPosition = new Vector2(srt.anchoredPosition.x + (1240 * m_SilverThreshold), srt.anchoredPosition.y);

        m_GoldThreshold = gold;
        RectTransform grt = goldStarLine.GetComponent<RectTransform>();
        grt.anchoredPosition = new Vector2(grt.anchoredPosition.x + (1240 * m_GoldThreshold), grt.anchoredPosition.y);

        //Call the update function with 0 parameter to correctly show which stars are available at game start.
        UpdateProgress(0.0f);
    }

    /*
    Simple function to store which stars are available in this instance of the game.
    Should be called from SpecificManager before game start which gets the info from the GameState stored in the GameManager.
    */
    public void SetPotentialPrizes(bool bronze, bool silver, bool gold)
    {
        m_CanEarnBronze = bronze;
        m_CanEarnSilver = silver;
        m_CanEarnGold = gold;
    }

    /*
    Continuously called function throughout gameplay to update this object to display current progress.
    parameter 'delta' is the change in general progress (which may be points for a correct answer, time elapsed, time survived, etc depending on the specific game being played).
    Return value will be used by SpecificManager to know that max/min score is reached and is one way to trigger a game over (usually for time-related scoring)
    */
    public bool UpdateProgress(float delta)
    {
        //Update the progress
        m_Progress += delta;

        var scale = modifiableBar.localScale;
        scale.x = Mathf.Min(1.0f, Mathf.Max(0.0f, m_Progress)); //clamp progress to a 0-1 range
        modifiableBar.localScale = scale; //resize the sliderBar depending on the progress achieved

        //Update the non-static images (if they can be earned and our progress is good enough, show the star; otherwise, hide it)
        if (m_CanEarnBronze && (m_Progress > m_BronzeThreshold)) 
            bronzeStarImage.gameObject.SetActive(true);
        else bronzeStarImage.gameObject.SetActive(false);

        if (m_CanEarnSilver && (m_Progress > m_SilverThreshold))
            silverStarImage.gameObject.SetActive(true);
        else silverStarImage.gameObject.SetActive(false);

        if (m_CanEarnGold && (m_Progress > m_GoldThreshold))
            goldStarImage.gameObject.SetActive(true);
        else goldStarImage.gameObject.SetActive(false);

        //If progress has exceeded the 0-1 range, return false to show that the game has concluded
        if ((m_Progress < 0.0f) || (m_Progress > 1.0f))
        {
            return false;
        }
        else return true;
    }

    //Since progress is stored in this object, these functions allow the EndGamePanel to question which stars have been achieved this game
    public bool EarnedBronzeStar() { return (m_CanEarnBronze && (m_Progress > m_BronzeThreshold)); }
    public bool EarnedSilverStar() { return (m_CanEarnSilver && (m_Progress > m_SilverThreshold)); }
    public bool EarnedGoldStar() { return (m_CanEarnGold && (m_Progress > m_GoldThreshold)); }
}
