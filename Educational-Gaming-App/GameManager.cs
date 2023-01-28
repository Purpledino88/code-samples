using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
The Game Manager class is our high level singleton-esque class.
Started with the beginning of the application and continues running throughout.
*/
public class GameManager : MonoBehaviour
{
    //Current number of stars earned
    public int m_GoldStars;
    public int m_SilverStars;
    public int m_BronzeStars;

    //Each game will have an associated game state, which will be used for the full day
    public List<GameStateStructure> m_CurrentStates;

    //Vocabulary changes for each game, so we store the list in the game manager and update as games use these words
    public List<string> m_VocabularyList;

    //Video clips will be consistent across multiple uses of the application, so are stored in order here and updated after being watched
    public List<VideoMetaData> m_GoldVideoClips;
    public List<VideoMetaData> m_SilverVideoClips;
    public List<VideoMetaData> m_BronzeVideoClips;

    //The game which is currently being played, which will be modified at game end
    public GameStateStructure m_GameInProgress = null;

    //The local filepath to a video to be played
    public string m_CurrentVideoClip;

    /*
    Function to load the most recent save file, or to use the default options in the main menu object in the editor.
    Should be called on Awake by the main menu object in the starting scene.
    Variables passed in come from the main menu object and represent the current games in the application.
    */
    public void LoadData(SaveData saved, List <string> games, List<string> vocab, List<VideoMetaData> bronze_videos, List<VideoMetaData> silver_videos, List<VideoMetaData> gold_videos)
    {
        bool is_new_day = false;

        //This manager must be available throughout the lifetime of the application in multiple scenes
        DontDestroyOnLoad(gameObject);

        if (saved != null) //save data found
        {
            Debug.Log("UNITY: Loading savefile from " + saved.Date + " (today is " + System.DateTime.Today.ToString().Split(' ')[0] + ")");
            LoadFromSavedFile(saved, games, vocab, bronze_videos, silver_videos, gold_videos);
            is_new_day = !System.DateTime.Today.ToString().Split(' ')[0].Equals(saved.Date); //compare today's date with the date of the save
        }
        else Debug.Log("UNITY: Cannot find savefile, loading all from main menu"); //usually first use of application or manual deletion of savefile

        //Even if savedata has been found, check against the main menu presets for differences (new or obselete games/videos/vocab)
        LoadFromMainMenu(games, vocab, bronze_videos, silver_videos, gold_videos);

        //if the data was saved before today, reset all game states to default
        if (is_new_day)
            ResetGameStates();
    }

    /*
    Function to update game difficulty and allow new stars to be earned.
    Called when save date is not today, allowing daily updates.
    */
    private void ResetGameStates()
    {
        Debug.Log("UNITY: It's a new dawn, it's a new day...");

        foreach (GameStateStructure gss in m_CurrentStates)
        {
            Debug.Log("UNITY: Resetting and updating game state for " + gss.name + " game.");
            int difficulty_increase = gss.stars_earned - 1; //bronze stars are expected, make game harder if silver or gold were earned, make easier if no stars were earned
            gss.difficulty = Mathf.Max(gss.difficulty + difficulty_increase, 0); //minimum difficulty is zero
            gss.stars_earned = 0;   //allow all stars to be earned
        }
    }

    /*
    Load saved data from pre-existing file.
    Variables come from the main menu object and represent the current games in the application.
    */
    private void LoadFromSavedFile(SaveData saved, List<string> games, List<string> vocab, List<VideoMetaData> bronze_videos, List<VideoMetaData> silver_videos, List<VideoMetaData> gold_videos)
    {
        //update game manager's stars to match savefile
        m_GoldStars = saved.GoldStars;
        m_SilverStars = saved.SilverStars;
        m_BronzeStars = saved.BronzeStars;
        Debug.Log("UNITY: Loading stars from file - " + m_GoldStars + " x Gold, " + m_SilverStars + " x Silver, " + m_BronzeStars + " x Bronze.");
        
        //Check each game and associated state in the savefile against the main menu
        foreach (GameStateStructure gss in saved.Games)
        {
            if (games.Contains(gss.name))
            {
                Debug.Log("UNITY: Loading game from file: " + gss.name);
                m_CurrentStates.Add(gss);
                games.Remove(gss.name); //remove items from the main menu list to prevent LoadFromMainMenu from adding again
            }
            else Debug.Log("UNITY: Game " + games + " from previous save is no longer in use, removing from application...");
        }

        //Check each piece of vocabulary in the savefile against the main menu
        foreach (string word in saved.Vocab)
        {
            if (vocab.Contains(word))
            {
                Debug.Log("UNITY: Loading word from file: " + word);
                m_VocabularyList.Add(word);
                vocab.Remove(word); //remove items from the main menu list to prevent LoadFromMainMenu from adding again
            }
            else Debug.Log("UNITY: Word " + word + " from previous save is no longer in use, removing from application...");
        }

        //Check each gold video referenced in the savefile against the main menu
        foreach (string gold_movie in saved.GoldMovies)
        {
            VideoMetaData vmd = null;
            foreach (VideoMetaData video in gold_videos)
            {
                if (video.name.Equals(gold_movie))
                    vmd = video;
            }

            if (vmd != null)
            {
                Debug.Log("UNITY: Loading video from file: " + gold_movie);
                m_GoldVideoClips.Add(vmd);
                gold_videos.Remove(vmd); //remove items from the main menu list to prevent LoadFromMainMenu from adding again
            }
            else Debug.Log("UNITY: Video " + gold_movie + " from previous save is no longer in use, removing from application...");
        }

        //Check each silver video referenced in the savefile against the main menu
        foreach (string silver_movie in saved.SilverMovies)
        {
            VideoMetaData vmd = null;
            foreach (VideoMetaData video in silver_videos)
            {
                if (video.name.Equals(silver_movie))
                    vmd = video;
            }

            if (vmd != null)
            {
                Debug.Log("UNITY: Loading video from file: " + silver_movie);
                m_SilverVideoClips.Add(vmd);
                silver_videos.Remove(vmd); //remove items from the main menu list to prevent LoadFromMainMenu from adding again
            }
            else Debug.Log("UNITY: Video " + silver_movie + " from previous save is no longer in use, removing from application...");
        }

        //Check each bronze video referenced in the savefile against the main menu
        foreach (string bronze_movie in saved.BronzeMovies)
        {
            VideoMetaData vmd = null;
            foreach (VideoMetaData video in bronze_videos)
            {
                if (video.name.Equals(bronze_movie))
                    vmd = video;
            }

            if (vmd != null)
            {
                Debug.Log("UNITY: Loading video from file: " + bronze_movie);
                m_BronzeVideoClips.Add(vmd);
                bronze_videos.Remove(vmd); //remove items from the main menu list to prevent LoadFromMainMenu from adding again
            }
            else Debug.Log("UNITY: Video " + bronze_movie + " from previous save is no longer in use, removing from application...");
        }
    }

    /*
    Function to add all data included in the editor which was not already handled by the LoadFromSavedFile function.
    These will usually be new games/videos/vocabulary added to the app.
    Variables come from the main menu object and represent the current games in use in the application, minus anything which was removed by the LoadFromSavedFile function.
    */
    private void LoadFromMainMenu(List<string> games, List<string> vocab, List<VideoMetaData> bronze_videos, List<VideoMetaData> silver_videos, List<VideoMetaData> gold_videos)
    {
        //Any games remaining in this list are not accounted for in the save file, so must be new to the application (with default settings)
        foreach (string game in games)
        {
            Debug.Log("UNITY: Loading new game: " + game);
            GameStateStructure gss = new GameStateStructure();
            gss.name = game;
            gss.stars_earned = 0;
            gss.difficulty = 0;
            m_CurrentStates.Add(gss);
        }

        //Any words remaining in this list are not accounted for in the save file, so must be new to the application
        foreach (string word in vocab)
        {
            Debug.Log("UNITY: Loading new word: " + word);
            m_VocabularyList.Add(word);
        }
        
        //Any videos remaining in these lists are not accounted for in the save file, so must be new to the application
        foreach (VideoMetaData video in bronze_videos)
        {
            Debug.Log("UNITY: Loading new video: " + video.name);
            m_BronzeVideoClips.Add(video);
        }

        foreach (VideoMetaData video in silver_videos)
        {
            Debug.Log("UNITY: Loading new video: " + video.name);
            m_SilverVideoClips.Add(video);
        }

        foreach (VideoMetaData video in gold_videos)
        {
            Debug.Log("UNITY: Loading new video: " + video.name);
            m_GoldVideoClips.Add(video);
        }
    }

    /*
    Simple function to return the first word in the vocab list while sending that word to the back of the list.
    Called by games on start to populate their tested vocabulary, allowing subsequent games to use other vocab.
    NOTE: some games will not test vocabulary!
    */
    public string RecycleVocabulary()
    {
        string ret = m_VocabularyList[0];
        m_VocabularyList.RemoveAt(0);
        m_VocabularyList.Add(ret);
        return ret;
    }

    /*
    Spend a bronze star to play a bronze level video. Index provided by selected UI button.
    */
    public void PlayBronzeVideo(int index) 
    {
        m_BronzeStars--;
        PlayVideo("Bronze/", m_BronzeVideoClips, index); 
    }

    /*
    Spend a silver star to play a silver level video. Index provided by selected UI button.
    */
    public void PlaySilverVideo(int index)
    {
        m_SilverStars--;
        PlayVideo("Silver/", m_SilverVideoClips, index); 
    }

    /*
    Spend a gold star to play a gold level video. Index provided by selected UI button.
    */
    public void PlayGoldVideo(int index)
    {
        m_GoldStars--;
        PlayVideo("Gold/", m_GoldVideoClips, index); 
    }

    /*
    General function to build filepath to a video clip and open scene which handles Android's video software.
    List is passed by PlayBronzeVideo, PlaySilverVideo or PlayGoldVideo depending on level of video.
    NOTE: Scene change is necessary to force saving on return to the main menu scene, to update the ordered video list and avoid being able to watch the same video endlessly.
    */
    private void PlayVideo(string prefix, List<VideoMetaData> vc, int index)
    {
        VideoMetaData clip = vc[index];

        //Recycle chosen clip
        vc.Remove(clip);
        vc.Add(clip);

        //Build filepath prior to scene change
        m_CurrentVideoClip = prefix + clip.name + ".mp4";

        Debug.Log("UNITY: Changing scene from Main to Videoplayer to play video " + m_CurrentVideoClip);
        SceneManager.LoadScene("AndroidVideo");
    }

    /*
    Debug clarity function to provide output in event of Android video pausing the application
    */
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("Unity - Game has been paused");
        }
        else
        {
            Debug.Log("Unity - Game has been restarted");
        }
    }

    /*
    Debug clarity and logging function.
    */
    void OnApplicationQuit()
    {
        Debug.Log("Unity - Game has exited");
    }
}
