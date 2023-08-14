/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.Application;

import java.util.Random;

/**
 *
 * @author hp
 */
public class Randomiser 
{
    static Random s_Random;
    public static void InitialiseRandomGenerator() { s_Random = new Random(); }
    
    public static int GetRandomInteger(int min, int max)
    {
        int range = max + 1 - min;
        return s_Random.nextInt(range) + min;
    }
}
