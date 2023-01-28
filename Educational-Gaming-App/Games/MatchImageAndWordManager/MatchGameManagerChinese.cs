using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Simple subclass of the MatchGameManager class, overriding abstract function in that class to load Chinese language.
*/
public class MatchGameManagerChinese : MatchGameManager
{
    protected override void LoadAudioAndWord(string word, WordData wd)
    {
        wd.audio = Resources.Load<AudioClip>("Chinese Spoken/" + word);
        wd.word = Resources.Load<Sprite>("Chinese Written/" + word);
    }
}
