using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
Script attached to the main menu object in the opening scene.
Main menu object will contain lists of all of the data to be used in the current iteration of the game.
This script will handle game initialisation and data persistence between sessions.
Saving and loading should only occur in the main menu scene, as we return to this scene after each game and video.
*/
public class MainMenu : MonoBehaviour
{
    //Prefab to initialise the game manager, which will be our consistent backbone of the application
    public GameManager gameManagerPrefab;

    //Reference to the initialised game manager
    private GameManager m_GameManagerInstance = null;

    //Editor variables to show current save state
    public Image saveStartedImage;
    public Image saveEndedImage;

    //Games currently allowed in the application
    public List<string> gameList;

    //Words currently allowed in the application
    public List<string> vocabularyList;

    //Videos currently allowed in the application, seperated by level
    public List<VideoMetaData> goldVideoClips;
    public List<VideoMetaData> silverVideoClips;
    public List<VideoMetaData> bronzeVideoClips;

    /*
    Called on application start, this function will handle game manager initialisation.
    Called following scene change, this function will save the current state of the application.
    */
    void Awake()
    {
        Debug.Log("UNITY: Main menu has woken up");

        //Hide save state images. On opening, there have been no saves this session. On scene change, images are handled by the SaveGame function.
        saveStartedImage.gameObject.SetActive(false);
        saveEndedImage.gameObject.SetActive(false);

        //Get the game manager class, either creating it on first opening the application, or finding the existing gameObject following scene change
        GameObject gm = GameObject.Find("Game Manager");
        if (gm == null) //On first opening
        {
            Debug.Log("UNITY: Instantiating Game Manager using prefab " + gameManagerPrefab);
            m_GameManagerInstance = Instantiate(gameManagerPrefab);
            m_GameManagerInstance.gameObject.name = "Game Manager";

            //Attempt to find a previous save file and pass it to the game manager along with our allowed games/videos/vocab
            Debug.Log("UNITY: -----Pre-creation of initial data into " + m_GameManagerInstance);
            SaveData data = LoadGame();
            m_GameManagerInstance.LoadData(data, gameList, vocabularyList, bronzeVideoClips, silverVideoClips, goldVideoClips);
            Debug.Log("UNITY: -----Post-creation of initial data");
        }
        else    //Following scene change
        {
            Debug.Log("UNITY: Using existing Game Manager");
            m_GameManagerInstance = gm.GetComponent<GameManager>();
            SaveGame();
        }
    }

    /*
    Function to find load data from previous session (json format) or null if there isn't any
    */
    private SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null;
    }

    /*
    Function to write current application state to device memory in json format
    */
    private void SaveGame()
    {
        //provide visual indication of save attempt
        saveStartedImage.gameObject.SetActive(true);
        saveEndedImage.gameObject.SetActive(false);

        SaveData data = new SaveData(m_GameManagerInstance);

        Debug.Log("UNITY: Creating savegame string");

        string json = JsonUtility.ToJson(data);

        Debug.Log("UNITY: Writing to save file: " + Application.persistentDataPath + "/savefile.json");

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        Debug.Log("UNITY: Game save completed");

        //provide visual indication of save success
        saveStartedImage.gameObject.SetActive(false);
        saveEndedImage.gameObject.SetActive(true);
    }
}
