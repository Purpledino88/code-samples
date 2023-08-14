/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.CharacterData;
import astonblades.Application.SixSidedDie;
import javafx.scene.control.Label;
import javafx.scene.paint.Color;

/**
 *
 * @author hp
 */
public class SanityCheckPanel extends CharacterDicePoolPanel
{
    public SanityCheckPanel(CharacterData cd)
    {
        m_Character = cd;
    }
    
    @Override
    protected void ResolveResult() 
    {
        m_Result = 0;
        for (SixSidedDie d : m_IndividualDice)
        {
            if (d.GetValue() <= (m_Character.GetCurrentInsanity() + 1))
                m_Result += d.GetValue();
        }
    }

    @Override
    public void RenderPanel() 
    {
        this.getChildren().clear();
        Label lTitle = new Label();
        lTitle.setTextFill(Color.WHITE);
        this.getChildren().add(lTitle);
        
        if (!m_Rolled)
        {            
            lTitle.setText("Sanity Check - Unrolled");
            if (ActionAttempt.GetActionRisk() != null)
            {
                m_IndividualDice.clear();
                switch (ActionAttempt.GetActionRisk())
                {
                    case DESPERATE:
                        AddDie();
                    case RISKY:
                        AddDie();
                    case CONTROLLED:
                        AddDie();
                }
                
                if (m_Character.GetHarmLevel() == 3)
                    AddDie();                
            }
        }
        else 
        {
            lTitle.setText("Sanity Check - Result: " + m_Result);
        }
        RenderDicePool(false);
    }
    
}
