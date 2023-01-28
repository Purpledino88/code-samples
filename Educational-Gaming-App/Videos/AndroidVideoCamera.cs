using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
Script to handle playing video clips on Android systems.
Attached to the empty scene AndroidVideo.
NOTE: scene switches are necessary to enforce data persistance by switching back to the main menu following end of video.
*/
public class AndroidVideoCamera : MonoBehaviour
{
    /*
    Called on entering the scene.
    Given that this is an empty scene, immediately start playing the video.
    */
    void Start()
    {
        Debug.Log("UNITY: Starting video controller");

        //Get the game manager, which is storing the video filepath to be played.
        GameObject obj = GameObject.Find("Game Manager");
        if (obj != null)
        {
            GameManager gm = obj.GetComponent<GameManager>();
            if (gm != null)
            {
                Debug.Log("UNITY: Found " + gm + " and trying to play: " + gm.m_CurrentVideoClip.ToString());
                StartCoroutine(PlayVideoCoroutine(gm.m_CurrentVideoClip));
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
    Function to play video using Android's inbuilt functions.
    This function will begin the video playback, wait for a few frames to ensure it is working, then return to the main menu.
    Since Android plays videos externally to the Unity application, video will be visible over the top of the application and close itself when ended.
    */
    IEnumerator PlayVideoCoroutine(string videoPath)
    {
        Handheld.PlayFullScreenMovie(videoPath, Color.black, FullScreenMovieControlMode.Minimal, FullScreenMovieScalingMode.Fill);
        yield return new WaitForEndOfFrame();
        Debug.Log("UNITY: Video playback started.");
        yield return new WaitForEndOfFrame();
        Debug.Log("UNITY: Video playback ongoing.");
        SceneManager.LoadScene("MainMenuScene");
    }
}
