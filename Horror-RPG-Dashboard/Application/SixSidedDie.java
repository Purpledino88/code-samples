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
public class SixSidedDie 
{    
    int m_Value = 0;
    public void RollDie() { m_Value = Randomiser.GetRandomInteger(1, 6); }
    public int GetValue() { return m_Value; }
}
