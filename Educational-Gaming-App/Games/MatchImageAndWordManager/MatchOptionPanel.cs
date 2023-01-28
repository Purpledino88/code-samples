using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Simple panel to display one master image with four additional options to select.
Two possible layouts:
    -Master picture with four written word options (Chinese or English)
    -Master written word (Chinese or English) with four picture options
*/
public class MatchOptionPanel : MonoBehaviour
{
    //Set in editor, linked to editor images/buttons
    public Image masterImage;
    public Image option1Image;
    public Button option1Button;
    public Image option2Image;
    public Button option2Button;
    public Image option3Image;
    public Button option3Button;
    public Image option4Image;
    public Button option4Button;

    //Called on each round of the game to update all images to match new options supplied by game manager
    public void Load(Sprite m, Sprite o1, Sprite o2, Sprite o3, Sprite o4)
    {
        //Set master
        masterImage.sprite = m;

        //Reactivate any previously selected buttons from the last round
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);
        option3Button.gameObject.SetActive(true);
        option4Button.gameObject.SetActive(true);

        //Set images on buttons
        option1Image.sprite = o1;
        option2Image.sprite = o2;
        option3Image.sprite = o3;
        option4Image.sprite = o4;
    }
}
