/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.CharacterData;
import astonblades.Application.Randomiser;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;

/**
 *
 * @author hp
 */
public class AllCharactersPanel extends VBox
{
    private boolean m_AddingCharacter = false;
    private String[] m_RandomisedImages = { "CHARACTER_PLACEHOLDER_1", "CHARACTER_PLACEHOLDER_2", "CHARACTER_PLACEHOLDER_3", "CHARACTER_PLACEHOLDER_4" };
    private double m_Width;
    private double m_Height;

    public AllCharactersPanel(double height, double width)
    {
        m_Width = width;
        m_Height = height;
        int iterations = Randomiser.GetRandomInteger(5, 10);
        String temp;
        for (int i = 0; i < iterations; i++)
        {
            for (int predicted_index = 0; predicted_index < 4; predicted_index++)
            {
                int random_index = Randomiser.GetRandomInteger(0, 3);
                temp = m_RandomisedImages[random_index];
                m_RandomisedImages[random_index] = m_RandomisedImages[predicted_index];
                m_RandomisedImages[predicted_index] = temp;
            }
        }
        Update();
    }
    
    public void AddCharacter()
    {
        m_AddingCharacter = true;
        Update();
    }
    
    public void ShowCharacters()
    {
        m_AddingCharacter = false;
        Update();
    }
    
    public void Update()
    {
        this.getChildren().clear();
        
        if (m_AddingCharacter)
        {
            this.getChildren().add(new CreateCharacterPanel(this));
        }
        else
        {
            HBox hb = new HBox();
            for (int i = 0; i < 4; i++)
            {
                if (i < CharacterData.GetAllCharacters().size())
                    hb.getChildren().add(new CharacterPanel(m_Height, m_Width / 4, CharacterData.GetAllCharacters().elementAt(i), this));
                else
                    hb.getChildren().add(ImageProcessor.GetImage(m_RandomisedImages[i]));
            }
            this.getChildren().add(hb);
        }
    }
}
