/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades;

/**
 *
 * @author hp
 */
public class EnumsAndConstants 
{
    public final static String cBoxStyle = 
            "-fx-background-color: black;\n" +
            "-fx-border-color: white;\n" +
            "-fx-border-width: 1;\n" +
            "-fx-border-style: solid;\n";
    
    public final static String cButtonIdleStyle = 
            "-fx-background-color: black;\n" +
            "-fx-text-fill: white;\n" +
            "-fx-border-color: white;\n" +
            "-fx-border-width: 1;\n" +
            "-fx-border-style: solid;\n";
    public final static String cButtonHoveredStyle = 
            "-fx-background-color: midnightblue;\n" +
            "-fx-text-fill: white;\n" +
            "-fx-border-color: white;\n" +
            "-fx-border-width: 1;\n" +
            "-fx-border-style: solid;\n";
    
    private static String EnumToString(String input)
    {
        String str = input;
        str = str.replace("_", " ");
        String[] splitStr = str.split(" ");
        String ret = "";
        for (String s : splitStr)
            ret += ((s.substring(0, 1).toUpperCase()) + s.substring(1).toLowerCase() + " ");
        return ret.strip();
    }
    public static String EnumToString(eSkills e) { return EnumToString(e.toString()); }
    public static String EnumToString(eGender e) { return EnumToString(e.toString()); }
    public static String EnumToString(eActionRisk e) { return EnumToString(e.toString()); }
    public static String EnumToString(eConsequence e) { return EnumToString(e.toString()); }
    public static String EnumToString(eCharacterStatus e) { return EnumToString(e.toString()); }
    public static String EnumToString(eRollType e) { return EnumToString(e.toString()); }
    public static String EnumToString(ePhobia e) { return EnumToString(e.toString()); }
    
    public enum eSkills
    {        
        COMMAND,
        DEXTERITY,
        INVESTIGATION,
        MEDICINE,
        MYSTICISM,
        PERCEPTION,
        PERSUASION,
        SCIENCE,
        STEALTH,
        STRENGTH,
        TECHNOLOGY,
        WILL
    }
    
    public enum eGender
    {
        MALE,
        FEMALE
    }
    
    public enum eActionRisk
    {
        CONTROLLED,
        RISKY,
        DESPERATE
    }
    
    public enum eConsequence
    {
        LOW_EFFECT,
        COMPLICATION,
        LOST_OPPORTUNITY,
        ESCALATION,
        HARM,
        SHOCK
    }
    
    public enum eCharacterStatus
    {
        ACTIVE,
        DEAD,
        INSANE
    }
    
    public enum eRollType
    {
        SKILL,
        FORTUNE,
        SANITY
    }
    
    public enum ePhobia
    {
        BLOOD,
        HEIGHTS,
        SPIDERS,
        DARKNESS,
        GHOSTS,
        WATER,
        LONELINESS,
        ENCLOSED_SPACES,
        BUGS,
        DOGS,
        SNAKES
    }
}
