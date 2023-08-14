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
public class ProfessionInfo 
{
    private String m_Name;
    public String GetProfessionName() { return m_Name; }
    
    private String m_Title;
    public String GetProfessionTitle() { return m_Title; }
    
    private Vector<EnumsAndConstants.eSkills> m_RequiredSkills = new Vector<EnumsAndConstants.eSkills>();
    public Vector<EnumsAndConstants.eSkills> GetSkills() { return m_RequiredSkills; }
    
    private ProfessionInfo(String name, String title, EnumsAndConstants.eSkills s1, EnumsAndConstants.eSkills s2)
    {
        m_Name = name;
        m_Title = title;
        m_RequiredSkills.add(s1);
        m_RequiredSkills.add(s2);
    }
    
    private static Vector<ProfessionInfo> GetAllProfessions()
    {
        Vector<ProfessionInfo> ret = new Vector<ProfessionInfo>();
        
        ret.add(new ProfessionInfo("Doctor", "Dr. ", EnumsAndConstants.eSkills.INVESTIGATION, EnumsAndConstants.eSkills.MEDICINE));
        ret.add(new ProfessionInfo("Ghost Hunter", "", EnumsAndConstants.eSkills.PERCEPTION, EnumsAndConstants.eSkills.MYSTICISM));
        ret.add(new ProfessionInfo("Engineer", "", EnumsAndConstants.eSkills.TECHNOLOGY, EnumsAndConstants.eSkills.SCIENCE));
        ret.add(new ProfessionInfo("Detective", "", EnumsAndConstants.eSkills.INVESTIGATION, EnumsAndConstants.eSkills.STEALTH));
        ret.add(new ProfessionInfo("Photographer", "", EnumsAndConstants.eSkills.PERCEPTION, EnumsAndConstants.eSkills.DEXTERITY));
        ret.add(new ProfessionInfo("Mechanic", "", EnumsAndConstants.eSkills.TECHNOLOGY, EnumsAndConstants.eSkills.STRENGTH));
        ret.add(new ProfessionInfo("Journalist", "", EnumsAndConstants.eSkills.INVESTIGATION, EnumsAndConstants.eSkills.PERSUASION));
        ret.add(new ProfessionInfo("Police Officer", "Con. ", EnumsAndConstants.eSkills.PERCEPTION, EnumsAndConstants.eSkills.COMMAND));
        ret.add(new ProfessionInfo("Inventor", "", EnumsAndConstants.eSkills.TECHNOLOGY, EnumsAndConstants.eSkills.WILL));
        ret.add(new ProfessionInfo("Army Medic", "Lt. ", EnumsAndConstants.eSkills.MEDICINE, EnumsAndConstants.eSkills.STRENGTH));
        ret.add(new ProfessionInfo("Apostate", "", EnumsAndConstants.eSkills.MYSTICISM, EnumsAndConstants.eSkills.STEALTH));
        ret.add(new ProfessionInfo("Scientist", "", EnumsAndConstants.eSkills.SCIENCE, EnumsAndConstants.eSkills.DEXTERITY));
        ret.add(new ProfessionInfo("Psychologist", "Dr. ", EnumsAndConstants.eSkills.MEDICINE, EnumsAndConstants.eSkills.PERSUASION));
        ret.add(new ProfessionInfo("Priest", "Vicar ", EnumsAndConstants.eSkills.MYSTICISM, EnumsAndConstants.eSkills.WILL));
        ret.add(new ProfessionInfo("Professor", "Prof. ", EnumsAndConstants.eSkills.SCIENCE, EnumsAndConstants.eSkills.COMMAND));
        ret.add(new ProfessionInfo("Con Artist", "", EnumsAndConstants.eSkills.DEXTERITY, EnumsAndConstants.eSkills.PERSUASION));
        ret.add(new ProfessionInfo("Homeless", "", EnumsAndConstants.eSkills.STEALTH, EnumsAndConstants.eSkills.WILL));
        ret.add(new ProfessionInfo("Army Officer", "Cpt. ", EnumsAndConstants.eSkills.STRENGTH, EnumsAndConstants.eSkills.COMMAND));
        
        return ret;
    }
    
    public static Vector<ProfessionInfo> GetAllAvailableProfessions(EnumsAndConstants.eSkills unavailable_skill_1,
                                                                    EnumsAndConstants.eSkills unavailable_skill_2,
                                                                    EnumsAndConstants.eSkills unavailable_skill_3)
    {
        Vector<ProfessionInfo> ret = new Vector<ProfessionInfo>();
        
        for (ProfessionInfo p : GetAllProfessions())
        {
            if (!p.m_RequiredSkills.contains(unavailable_skill_1) && !p.m_RequiredSkills.contains(unavailable_skill_2) && !p.m_RequiredSkills.contains(unavailable_skill_3))
                ret.add(p);
        }
        
        return ret;
    }
    
    
}
