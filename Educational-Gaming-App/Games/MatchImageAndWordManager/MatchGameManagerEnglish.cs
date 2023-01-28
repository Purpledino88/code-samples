using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Simple subclass of the MatchGameManager class, overriding abstract function in that class to load English language.
*/
public class MatchGameManagerEnglish : MatchGameManager
{
    protected override void LoadAudioAndWord(string word, WordData wd)
    {
        wd.audio = Resources.Load<AudioClip>("English Spoken/" + word);
        wd.word = Resources.Load<Sprite>("English Written/" + word);
    }
}
