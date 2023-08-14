/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.util.HashMap;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;

/**
 *
 * @author hp
 */
public class ImageProcessor 
{
    private final static String s_ImageDirectory = "Images\\";
    private final static String s_ImageSuffix = ".jpg";
    private final static String s_ImageSuffixTransparency = ".png";
    private final static String[] s_ImageFilenames  = { "BACKGROUND", "CHARACTER_PLACEHOLDER_1", "CHARACTER_PLACEHOLDER_2", "CHARACTER_PLACEHOLDER_3", "CHARACTER_PLACEHOLDER_4", 
                                                        "SANITY_DICE_0", "SANITY_DICE_1", "SANITY_DICE_2", "SANITY_DICE_3", "SANITY_DICE_4", "SANITY_DICE_5", "SANITY_DICE_6", 
                                                        "FORTUNE_DICE_0", "FORTUNE_DICE_1", "FORTUNE_DICE_2", "FORTUNE_DICE_3", "FORTUNE_DICE_4", "FORTUNE_DICE_5", "FORTUNE_DICE_6",
                                                        "SKILL_DICE_0", "SKILL_DICE_1", "SKILL_DICE_2", "SKILL_DICE_3", "SKILL_DICE_4", "SKILL_DICE_5", "SKILL_DICE_6", 
                                                        "SKILL_DICE_0_ALT", "SKILL_DICE_1_ALT", "SKILL_DICE_2_ALT", "SKILL_DICE_3_ALT", "SKILL_DICE_4_ALT", "SKILL_DICE_5_ALT", "SKILL_DICE_6_ALT", 
                                                        "CHARACTER_MALE_DOCTOR", "CHARACTER_FEMALE_DOCTOR", "CHARACTER_MALE_GHOST_HUNTER", "CHARACTER_FEMALE_GHOST_HUNTER",
                                                        "CHARACTER_MALE_ENGINEER", "CHARACTER_FEMALE_ENGINEER", "CHARACTER_MALE_DETECTIVE", "CHARACTER_FEMALE_DETECTIVE",
                                                        "CHARACTER_MALE_PHOTOGRAPHER", "CHARACTER_FEMALE_PHOTOGRAPHER", "CHARACTER_MALE_MECHANIC", "CHARACTER_FEMALE_MECHANIC",
                                                        "CHARACTER_MALE_JOURNALIST", "CHARACTER_FEMALE_JOURNALIST", "CHARACTER_MALE_POLICE_OFFICER", "CHARACTER_FEMALE_POLICE_OFFICER",
                                                        "CHARACTER_MALE_INVENTOR", "CHARACTER_FEMALE_INVENTOR", "CHARACTER_MALE_ARMY_MEDIC", "CHARACTER_FEMALE_ARMY_MEDIC",
                                                        "CHARACTER_MALE_APOSTATE", "CHARACTER_FEMALE_APOSTATE", "CHARACTER_MALE_SCIENTIST", "CHARACTER_FEMALE_SCIENTIST", 
                                                        "CHARACTER_MALE_PSYCHOLOGIST", "CHARACTER_FEMALE_PSYCHOLOGIST", "CHARACTER_MALE_PRIEST", "CHARACTER_FEMALE_PRIEST",
                                                        "CHARACTER_MALE_PROFESSOR", "CHARACTER_FEMALE_PROFESSOR", "CHARACTER_MALE_CON_ARTIST", "CHARACTER_FEMALE_CON_ARTIST",
                                                        "CHARACTER_MALE_HOMELESS", "CHARACTER_FEMALE_HOMELESS", "CHARACTER_MALE_ARMY_OFFICER", "CHARACTER_FEMALE_ARMY_OFFICER",
                                                        "CHARACTER_PORTRAIT_FRAME", "STATUS_DEAD", "STATUS_INSANE"
                                                      };
    private static HashMap<String, Image> s_ImageMap = new HashMap<>();
    
    public static void LoadAllImages()
    {
        for (String str : s_ImageFilenames)
        {
            try 
            {
                s_ImageMap.put(str, new Image(new FileInputStream(s_ImageDirectory + str + s_ImageSuffix)));
            } 
            catch (FileNotFoundException ex) 
            {
                try 
                {
                    s_ImageMap.put(str, new Image(new FileInputStream(s_ImageDirectory + str + s_ImageSuffixTransparency)));
                } 
                catch (FileNotFoundException ex2) 
                {
                    System.out.println("IMAGE PROCESSOR - Cannot find file : " + str + s_ImageSuffix + "/" + str + s_ImageSuffixTransparency);
                    //Logger.getLogger(ImageProcessor.class.getName()).log(Level.SEVERE, null, ex2);
                }
            }
        }
    }
    
    public static ImageView GetImage(String image_name)
    {
        return new ImageView(s_ImageMap.get(image_name));
    }
}
