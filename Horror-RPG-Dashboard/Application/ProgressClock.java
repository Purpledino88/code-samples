/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.Application;

import astonblades.EnumsAndConstants;
import java.util.Vector;

/**
 *
 * @author hp
 */
public class ProgressClock 
{
    static private Vector<ProgressClock> s_AllClocks = new Vector<ProgressClock>();
    static public Vector<ProgressClock> GetAllClocks() { return s_AllClocks; }
    
    static private int s_EscalationPoints = 0;
    static public int GetEscalationPoints() { return s_EscalationPoints; }
    
    public static void ApplyEscalation(EnumsAndConstants.eActionRisk risk)
    {
        switch (risk)
        {
            case CONTROLLED:
                s_EscalationPoints += 1;
                return;
                
            case RISKY:
                s_EscalationPoints += 2;
                return;
                
            case DESPERATE:
                s_EscalationPoints += 3;
                return;
        }
    }
    
    public static void AddNewClock(String name, int length)
    {
        if (s_EscalationPoints > 0)
            s_AllClocks.add(new ProgressClock(name, length));
    }
    
    private String m_ClockName;
    public String GetClockName() { return m_ClockName; }
    private int m_ClockLength;
    public int GetClockLength() { return m_ClockLength; }
    private int m_ClockProgress = 0;
    public int GetClockProgress() { return m_ClockProgress; }
    
    public ProgressClock(String name, int length)
    {
        m_ClockName = name;
        m_ClockLength = length;
        m_ClockProgress = 0;
        s_EscalationPoints--;
    }
    
    public void IncrementClock()
    {
        if (s_EscalationPoints > 0)
        {
            m_ClockProgress++;
            s_EscalationPoints--;
        }
    }
    
    public boolean IsFinished()
    {
        return (m_ClockProgress == m_ClockLength);
    }
    
    public void DeleteClock()
    {
        s_AllClocks.remove(this);
    }
}
