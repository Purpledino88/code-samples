using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Data structure to be used in loading/saving for data persistence.
*/
[System.Serializable]
public class SaveData
{
    //Today's date
    public string Date;

    //Current number of stars earned
    public int GoldStars;
    public int SilverStars;
    public int BronzeStars;

    //Current difficulty and available stars for each game
    public List<GameStateStructure> Games = new List<GameStateStructure>();

    //Current order of available vocabulary (string format)
    public List<string> Vocab = new List<string>();

    //Current order of available video names seperated by level
    public List<string> GoldMovies = new List<string>();
    public List<string> SilverMovies = new List<string>();
    public List<string> BronzeMovies = new List<string>();

    /*
    Function populating this structure with all relevant data from the game manager
    */
    public SaveData(GameManager manager)
    {
        Debug.Log("UNITY: Collecting game data");

        Date = System.DateTime.Today.ToString().Split(' ')[0]; //time is irrelevant, so only save the date

        GoldStars = manager.m_GoldStars;
        SilverStars = manager.m_SilverStars;
        BronzeStars = manager.m_BronzeStars;

        foreach (GameStateStructure gss in manager.m_CurrentStates)
            Games.Add(gss);

        foreach (string s in manager.m_VocabularyList)
            Vocab.Add(s);

        foreach (VideoMetaData vmd in manager.m_GoldVideoClips)
            GoldMovies.Add(vmd.name);

        foreach (VideoMetaData vmd in manager.m_SilverVideoClips)
            SilverMovies.Add(vmd.name);

        foreach (VideoMetaData vmd in manager.m_BronzeVideoClips)
            BronzeMovies.Add(vmd.name);      
    }
}
