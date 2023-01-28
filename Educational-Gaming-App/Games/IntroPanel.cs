using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Simple UI class to display a countdown to the start of the game.
*/
public class IntroPanel : MonoBehaviour
{
    //UI objects linked in the editor
    public GameObject circle1;
    public GameObject circle2;
    public GameObject circle3;

    /*
    Function called by SpecificManager subclass which controls the game.
    Parameter is a float countdown which is stored in the SpecificManager.
    */
    public void UpdateCountdown(float cd)
    {
        int updated = 4;
        if (cd < 3.0f)
            updated = 3;
        if (cd < 2.0f)
            updated = 2;
        if (cd < 1.0f)
            updated = 1;
        if (cd < 0.0f)
            this.gameObject.SetActive(false);
        
        //Display appropriate mono-objects based on parameter supplied
        switch (updated)
        {
            case 1:
                circle1.SetActive(true);
                circle2.SetActive(false);
                circle3.SetActive(false);
                break;
            case 2:
                circle1.SetActive(false);
                circle2.SetActive(true);
                circle3.SetActive(false);
                break;
            case 3:
                circle1.SetActive(false);
                circle2.SetActive(false);
                circle3.SetActive(true);
                break;
        }
    }
}
