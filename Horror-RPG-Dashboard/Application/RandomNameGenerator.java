/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.Application;

/**
 *
 * @author hp
 */
public class RandomNameGenerator 
{
    static String[] s_MaleNames =   { 
                                        "Arthur", "Benjamin", "Christopher", "Daniel", "Edward",
                                        "Felix", "Gregory", "Ian", "James", "Kingsley",
                                        "Liam", "Matthew", "Nathaniel", "Oscar", "Peter",
                                        "Quentin", "Richard", "Stephen", "Tobias", "Ulysses",
                                        "Vincent", "William", "Xavier", "Harold", "Zachary"
                                    };
    
    static String[] s_FemaleNames =   { 
                                        "Angela", "Beatrice", "Clara", "Diana", "Esther",
                                        "Fiona", "Geraldine", "Hermione", "Isabella", "Jessica",
                                        "Katherine", "Lena", "Maria", "Naomi", "Oprah",
                                        "Patricia", "Quinna", "Rebecca", "Sarah", "Teri",
                                        "Una", "Victoria", "Zarah", "Willow", "Yasmine"
                                    };
    
    static String[] s_FamilyNames =   { 
                                        "Arlington", "Bell", "Charleston", "Davis", "Elderwood",
                                        "Fitzpatrick", "Graves", "Harlon", "???", "???",
                                        "Killian", "Leon", "McDonald", "???", "O'Brien",
                                        "Partridge", "???", "Robinson", "Smythe", "???",
                                        "Underhill", "Venturi", "Winterbottom", "???", "???",
                                        "???", "???", "???", "???", "???",
                                        "???", "???", "???", "???", "???",
                                        "???", "???", "???", "???", "???",
                                        "???", "???", "???", "???", "???",
                                        "???", "???", "???", "???", "???"
                                    };
    
    private static String GetRandom(String[] array)
    {
        return array[Randomiser.GetRandomInteger(0, array.length - 1)];
    }
    
    public static String GetMaleName()
    {
        String ret = GetRandom(s_MaleNames) + " " + GetRandom(s_FamilyNames);
        return ret;
    }    
    
    public static String GetFemaleName()
    {
        String ret = GetRandom(s_FemaleNames) + " " + GetRandom(s_FamilyNames);
        return ret;
    }
}
