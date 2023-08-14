/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.Application;

import astonblades.EnumsAndConstants;
import astonblades.GUI.ActionPanel;
import astonblades.GUI.AllCharactersPanel;
import astonblades.GUI.ClockPanel;
import astonblades.GUI.DicePoolPanel;
import astonblades.GUI.FortuneRollPanel;
import astonblades.GUI.SanityCheckPanel;
import astonblades.GUI.SkillCheckPanel;
import java.util.HashMap;

/**
 *
 * @author hp
 */
public class ActionAttempt 
{
    static private EnumsAndConstants.eActionRisk s_Risk = null;
    static public EnumsAndConstants.eActionRisk GetActionRisk() { return s_Risk; }
    static public void SetRisk(EnumsAndConstants.eActionRisk r) 
    { 
        s_Risk = r; 
        for (DicePoolPanel dpp : s_DicePools.keySet())
        {
            dpp.RenderPanel();
        }
    }
    
    static private EnumsAndConstants.eRollType s_Type = null;
    static public EnumsAndConstants.eRollType GetActionType() { return s_Type; }
    
    static private EnumsAndConstants.eSkills s_Skill = null;
    static public EnumsAndConstants.eSkills GetActionSkill() { return s_Skill; }
    
    //static private Vector<DicePoolPanel> s_DicePools = new Vector<>();
    static private HashMap<DicePoolPanel, CharacterData> s_DicePools = new HashMap<>();
    static private ActionPanel s_ActionPanel;
    static private AllCharactersPanel s_CharactersPanel;
    static private ClockPanel s_ClockPanel;
    
    static private boolean s_InProgress = false;
    static public boolean IsInProgress() { return s_InProgress; }
    
    static private boolean s_Rolled = false;
    static public boolean HasBeenRolled() { return s_Rolled; }
    
    static private boolean s_HasConsequence = false;
    static public boolean HasConsequences() { return s_HasConsequence; }
    
    static public void Setup(ActionPanel ap, AllCharactersPanel acp, ClockPanel cp) 
    { 
        s_ActionPanel = ap; 
        s_CharactersPanel = acp;
        s_ClockPanel = cp;
    }        
    
    static public boolean FortuneRoll(FortuneRollPanel dice_pool)
    {
        if (!s_InProgress)
        {
            s_InProgress = true;
            s_Type = EnumsAndConstants.eRollType.FORTUNE;
            s_DicePools.put(dice_pool, null);
            s_ActionPanel.Update();
            return true;
        }
        return false;
    }
    
    static public boolean SkillRoll(CharacterData affected_character, SkillCheckPanel dice_pool, EnumsAndConstants.eSkills skill)
    {
        if (!s_InProgress || (s_Type == EnumsAndConstants.eRollType.SKILL && skill == s_Skill))
        {
            s_InProgress = true;
            s_Type = EnumsAndConstants.eRollType.SKILL;
            s_Skill = skill;
            s_DicePools.put(dice_pool, affected_character);
            s_ActionPanel.Update();
            return true;
        }
        return false;
    }
    
    static public boolean SanityRoll(CharacterData affected_character, SanityCheckPanel dice_pool)
    {
        if (!s_InProgress || s_Type == EnumsAndConstants.eRollType.SANITY)
        {
            s_InProgress = true;
            s_Type = EnumsAndConstants.eRollType.SANITY;
            s_DicePools.put(dice_pool, affected_character);
            s_ActionPanel.Update();
            return true;
        }
        return false;
    }
    
    static public void RollDice()
    {
        for (DicePoolPanel dpp : s_DicePools.keySet())
        {
            dpp.RollDice();        
            dpp.RenderPanel();
        }        
        s_Rolled = true;
        s_HasConsequence = CheckForConsequences();
        s_ActionPanel.Update();
    }
    
    static private boolean CheckForConsequences()
    {
        switch (s_Type)
        {
            case FORTUNE:
            {
                return false;
            }
            case SKILL:
            {
                for (DicePoolPanel dpp : s_DicePools.keySet())
                {
                    if (dpp.GetResult() < 6)
                        return true;
                }  
                return false;
            }
            case SANITY:
            {
                for (DicePoolPanel dpp : s_DicePools.keySet())
                {
                    if (dpp.GetResult() > 0)
                        return true;
                }  
                return false;
            }
        }
        return false;
    }
    
    static public void ApplyConsequence(EnumsAndConstants.eConsequence consequence)
    {
        switch (s_Type)
        {
            case SKILL:
            {
                ApplyConsequenceForSkillCheck(consequence);
                break;
            }
            
            case SANITY:
            {
                ApplyConsequenceForSanityCheck();
                break;
            }
            
            case FORTUNE:
            {
                ClearRoll();
                break;
            }
        }
    }
    
    static private void ApplyConsequenceForSkillCheck(EnumsAndConstants.eConsequence consequence)
    {
        if (consequence == null)
        {
            ClearRoll();
        }
        else
        {
            switch (consequence)
            {
                case ESCALATION:
                {
                    ProgressClock.ApplyEscalation(s_Risk);
                    s_ClockPanel.Update();
                    ClearRoll();
                    break;
                }
                case HARM:
                {
                    for (DicePoolPanel dpp : s_DicePools.keySet())
                    {
                        ApplyHarm(dpp);
                    }
                    ClearRoll();
                    break;
                }
                case SHOCK:
                {
                    for (DicePoolPanel dpp : s_DicePools.keySet())
                    {
                        ApplyShock(dpp);
                    }
                    ClearRoll();
                    break;
                }
                default:
                {
                    ClearRoll();
                    break;
                }
            }
        }
    }
        
    static private void ApplyHarm(DicePoolPanel dpp)
    {
        switch (s_Risk)
        {
            case CONTROLLED:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeHarm(1);
                break;
            }
            
            case RISKY:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeHarm(2);
                else if (dpp.GetResult() <= 5)
                    s_DicePools.get(dpp).TakeHarm(1);
                break;
            }
            
            case DESPERATE:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeHarm(3);
                else if (dpp.GetResult() <= 5)
                    s_DicePools.get(dpp).TakeHarm(2);
                break;
            }
        }
    }
    
    static private void ApplyShock(DicePoolPanel dpp)
    {
        switch (s_Risk)
        {
            case CONTROLLED:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeStress(1, false);
                break;
            }
            
            case RISKY:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeStress(2, false);
                else if (dpp.GetResult() <= 5)
                    s_DicePools.get(dpp).TakeStress(1, false);
                break;
            }
            
            case DESPERATE:
            {
                if (dpp.GetResult() <= 3)
                    s_DicePools.get(dpp).TakeStress(3, false);
                else if (dpp.GetResult() <= 5)
                    s_DicePools.get(dpp).TakeStress(2, false);
                break;
            }
        }
    }
    
    static private void ApplyConsequenceForSanityCheck()
    {
        for (DicePoolPanel dpp : s_DicePools.keySet())
        {
            if (dpp.GetResult() > 0)
                s_DicePools.get(dpp).TakeStress(dpp.GetResult(), false);
        }  
        ClearRoll();
    }
    
    static private void ClearRoll()
    {
        s_DicePools.clear();
        s_InProgress = false;
        s_Rolled = false;
        s_Risk = null;
        s_Type = null;
        s_Skill = null;
        s_ActionPanel.Update();
        s_CharactersPanel.Update();
    }
}
