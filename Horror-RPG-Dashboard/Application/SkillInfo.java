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
public class SkillInfo 
{    
    static public Vector<SkillInfo> GenerateSkillList()
    {
        Vector<SkillInfo> vec = new Vector<SkillInfo>();
        for (EnumsAndConstants.eSkills s : EnumsAndConstants.eSkills.values())
            vec.add(new SkillInfo(s));
        return vec;
    }
    
    private EnumsAndConstants.eSkills m_Skill;
    public EnumsAndConstants.eSkills GetSkillType() { return m_Skill; }
    
    private String m_Name;
    public String GetName() {return m_Name; }
    
    private int m_Expertise;
    public void IncrementExpertise() { m_Expertise++; }
    public int GetExpertise() { return m_Expertise; }
    
    private boolean m_IsTrainable;
    public boolean GetTrainable() { return m_IsTrainable; }
    public void SetTrainable(boolean b) { m_IsTrainable = b; }
    
    private SkillInfo(EnumsAndConstants.eSkills skill)
    {
        m_Skill = skill;
        m_Name = EnumsAndConstants.EnumToString(skill);
        m_Expertise = 0;
        m_IsTrainable = true;
    }
}
