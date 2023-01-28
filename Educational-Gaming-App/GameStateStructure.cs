using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Simple saveable structure to hold today's progress in each game.
*/
[System.Serializable]
public class GameStateStructure
{
    public string name;
    public int stars_earned;
    public int difficulty;
}
