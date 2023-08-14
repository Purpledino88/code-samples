/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.Application;

import astonblades.EnumsAndConstants;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;
import java.util.Vector;

/**
 *
 * @author hp
 */
public class CharacterData 
{
    private static Vector<CharacterData> s_AllCharacters = new Vector<CharacterData>();
    public static Vector<CharacterData> GetAllCharacters() { return s_AllCharacters; }
    public static void AddNewCharacter(CharacterData cd)
    {
        if (s_AllCharacters.size() < 4)
            s_AllCharacters.add(cd);
    }
    public static void RestCharacters()
    {
        Vector<CharacterData> vec = new Vector<>();
        for (CharacterData cd : s_AllCharacters)
        {
            if (cd.m_Status == EnumsAndConstants.eCharacterStatus.ACTIVE)
            {
                cd.RestCharacter();
                vec.add(cd);
            }
        }
        s_AllCharacters = vec;
    }
    public static void SaveGame()
    {
        try 
        {
            System.out.println("SAVING GAME...");
            FileWriter writer = new FileWriter("Saved.txt");
            BufferedWriter bufferedWriter = new BufferedWriter(writer);
            for (CharacterData cd : s_AllCharacters)
            {
                if (cd.GetCurrentStatus() == EnumsAndConstants.eCharacterStatus.ACTIVE)
                {
                    String saveStr = new String(
                            cd.m_Name + ";" + 
                            cd.m_Description + ";" + 
                            cd.m_Fears.toString() + ";");
                    for (SkillInfo si : cd.m_Skills)
                    {
                        saveStr += (si.GetExpertise() + "/");
                    }
                    saveStr += ";";
                    for (int i = 0; i < 6; i++)
                    {
                        saveStr += (cd.m_Health[i] + (i != 5 ? "/" : ""));
                    }
                    saveStr += ";";
                    saveStr += (cd.m_Stress + ";");
                    saveStr += (cd.m_Insanity + ";");

                    System.out.println(saveStr);
                    bufferedWriter.write(saveStr);
                    bufferedWriter.newLine();
                }
            }
            bufferedWriter.close();
            System.out.println("COMPLETED SAVE.");
        }
        catch (IOException e) 
        {
                System.out.print("IO exception: ");
                e.printStackTrace();
        }   
    }
    public static void LoadGame()
    {
        try 
        {
            System.out.println("LOADING GAME...");
            FileReader reader = new FileReader("Saved.txt");
            BufferedReader bufferedReader = new BufferedReader(reader);
            
            String loaded = new String();
            String line;

            while ((line = bufferedReader.readLine()) != null) 
            {
                loaded += line;
                loaded += "\n";
            }
            reader.close();
            
            //Clear existing and new, unsaved characters
            s_AllCharacters.clear();
            
            //Add characters based on savefile
            String[] splitStrs = loaded.split("\n");
            for (String character : splitStrs)
            {
                String splitCharacterStrs[] = character.split(";");
                
                String charName = splitCharacterStrs[0];
                String charDescription = splitCharacterStrs[1];
                
                Vector<EnumsAndConstants.ePhobia> charFears = new Vector<>();
                for (EnumsAndConstants.ePhobia p : EnumsAndConstants.ePhobia.values())
                {
                    if (splitCharacterStrs[2].contains(p.toString()))
                        charFears.add(p);
                }
                
                Vector<SkillInfo> charSkills = SkillInfo.GenerateSkillList();
                String splitCharacterAbilityStrs[] = splitCharacterStrs[3].split("/");
                for (int i = 0; i < EnumsAndConstants.eSkills.values().length; i++)
                {  
                    int expertise = Integer.parseInt(splitCharacterAbilityStrs[i]);
                    for (int j = 0; j < expertise; j++)
                        charSkills.get(i).IncrementExpertise();
                }
                
                CharacterData loaded_character = new CharacterData(charName, charDescription, charSkills, charFears);
                
                String charHarm[] = splitCharacterStrs[4].split("/");
                for (int i = 0; i < 6; i++)
                {
                    if (charHarm[i].contains("false"))
                    {
                        if (i < 3)
                            loaded_character.TakeHarm(1);
                        else if (i < 5)
                            loaded_character.TakeHarm(2);
                        else 
                            loaded_character.TakeHarm(3);                            
                    }
                }
                
                int total_stress_taken = Integer.parseInt(splitCharacterStrs[5]);
                for (int i = 0; i < Integer.parseInt(splitCharacterStrs[6]); i++)
                    total_stress_taken += (10 - i);
                loaded_character.TakeStress(total_stress_taken, true);
                
                AddNewCharacter(loaded_character);
            }
            System.out.println("COMPLETED LOAD.");
        }
        catch (FileNotFoundException e) 
        {
                System.out.print("Missing File: ");
                e.printStackTrace();
        } 
        catch (IOException e) 
        {
                System.out.print("IO exception: ");
                e.printStackTrace();
        }
    }
    
    private String m_Name;
    public String GetName() { return m_Name; }
    private String m_Description;
    public String GetDescription() { return m_Description; }
    
    private EnumsAndConstants.eCharacterStatus m_Status = EnumsAndConstants.eCharacterStatus.ACTIVE;
    public EnumsAndConstants.eCharacterStatus GetCurrentStatus() { return m_Status; }
    
    private Vector<EnumsAndConstants.ePhobia> m_Fears = new Vector<EnumsAndConstants.ePhobia>();
    public Vector<EnumsAndConstants.ePhobia> GetPhobiasList() { return m_Fears; }
    
    private int m_Stress;
    public int GetCurrentStress() { return m_Stress; }
    private int m_Insanity;
    public int GetCurrentInsanity() { return m_Insanity; }
    
    private boolean[] m_Health = new boolean[6];
    public boolean[] GetCurrentHealth() { return m_Health; }
    private int m_HarmLevel;
    public int GetHarmLevel() { return m_HarmLevel; }
    
    private Vector<SkillInfo> m_Skills;
    public SkillInfo GetSkill(EnumsAndConstants.eSkills skill)
    {
        for (SkillInfo si : m_Skills)
        {
            if (si.GetSkillType().equals(skill))
                return si;
        }
        System.out.println("Cannot find skill " + skill.toString() + " for character " + m_Name);
        return null;
    }
    
    public CharacterData(String name, String description, Vector<SkillInfo> skills, Vector<EnumsAndConstants.ePhobia> fears)
    {
        m_Name = name;
        m_Description = description;
        m_Skills = skills;
        m_Fears = fears;
        
        m_Health[0] = true;
        m_Health[1] = true;
        m_Health[2] = true;
        m_Health[3] = true;
        m_Health[4] = true;
        m_Health[5] = true;
        m_HarmLevel = 0;
        
        m_Stress = 0;
        m_Insanity = 0;
    }
    
    public void RestCharacter()
    {
        m_Health[0] = m_Health[3];
        m_Health[1] = m_Health[4];
        m_Health[2] = true;
        m_Health[3] = m_Health[5];
        m_Health[4] = true;
        m_Health[5] = true;
        
        CalculateHarmLevel();
        
        m_Stress = Math.max(0, m_Stress - (6 - m_Insanity));
    }
    
    public void TakeStress(int stress_taken, boolean on_load)
    {
        m_Stress += stress_taken;
        int maxStress = 10 - m_Insanity;
        if (m_Stress >= maxStress)
        {
            m_Stress -= maxStress;
            m_Insanity++;
            
            if (!on_load)
            {
                //Add a new random phobia
                EnumsAndConstants.ePhobia fear = EnumsAndConstants.ePhobia.values()[Randomiser.GetRandomInteger(0, EnumsAndConstants.ePhobia.values().length - 1)];
                while (m_Fears.contains(fear))
                    fear = EnumsAndConstants.ePhobia.values()[Randomiser.GetRandomInteger(0, EnumsAndConstants.ePhobia.values().length - 1)];
                m_Fears.add(fear);
            }
            
            if (m_Insanity >= 5)
                m_Status = EnumsAndConstants.eCharacterStatus.INSANE;
        }
    }
    
    public void TakeHarm(int harm_level)
    {
        int index = 0;
        switch (harm_level)
        {
            case 1: //level one harm (index 0, 1 or 2)
                index = 0;
                break;
                
            case 2: //level two harm (index 3 or 4)
                index = 3;
                break;
                
            case 3: //level three harm (index 5)
                index = 5;
                break;
        }
        
        while (index < 6)
        {
            if (m_Health[index])
            {
                m_Health[index] = false;
                CalculateHarmLevel();
                return;
            }
            else index++;
        }        
        
        //If we reach this point, no appropriate harm slots are available, so the character dies
        m_Status = EnumsAndConstants.eCharacterStatus.DEAD;
    }
    
    private void CalculateHarmLevel()
    {
        m_HarmLevel = 0;
        if (!m_Health[0] || !m_Health[1] || !m_Health[2])
            m_HarmLevel = 1;
        if (!m_Health[3] || !m_Health[4])
            m_HarmLevel = 2;
        if (!m_Health[5])
            m_HarmLevel = 3;
    }
}
