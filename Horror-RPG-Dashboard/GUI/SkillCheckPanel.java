/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.CharacterData;
import astonblades.Application.SixSidedDie;
import astonblades.EnumsAndConstants;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.layout.HBox;
import javafx.scene.paint.Color;

/**
 *
 * @author hp
 */
public class SkillCheckPanel extends CharacterDicePoolPanel
{    
    private boolean m_Pushed = false;
    private int m_SkillLevel;
    
    public SkillCheckPanel(CharacterData cd)
    {
        m_Character = cd;
    }
    
    @Override
    public void ResetPanel()
    {
        super.ResetPanel();
        m_Pushed = false;
    }

    @Override
    protected void ResolveResult() 
    {
        if (m_Pushed)
            m_Character.TakeStress(2, false);
        
        m_Character.TakeStress(Math.max(0, m_Character.GetHarmLevel() - 1), false);
        
        if (m_SkillLevel > 0)
        {
            m_Result = 0;
            for (SixSidedDie d : m_IndividualDice)
            {
                if (d.GetValue() > m_Result)
                    m_Result = d.GetValue();
            }
        }
        else
        {
            m_Result = 7;
            for (SixSidedDie d : m_IndividualDice)
            {
                if (d.GetValue() < m_Result)
                    m_Result = d.GetValue();
            }
        }
    }

    @Override
    public void RenderPanel() 
    {        
        this.getChildren().clear();
        
        HBox hbTitle = new HBox();
        
        Label lTitle = new Label();
        lTitle.setTextFill(Color.WHITE);
        hbTitle.getChildren().add(lTitle);
               
        this.getChildren().add(hbTitle);
        
        if (!m_Rolled)            
        {
            lTitle.setText("Skill Check (" + EnumsAndConstants.EnumToString(ActionAttempt.GetActionSkill()) + ") - Unrolled");
            if (ActionAttempt.GetActionRisk() != null)
            {
                Button bPush = new Button("Push yourself!");
                bPush.setStyle(EnumsAndConstants.cButtonIdleStyle);
                bPush.setOnMouseEntered(e -> bPush.setStyle(EnumsAndConstants.cButtonHoveredStyle));
                bPush.setOnMouseExited(e -> bPush.setStyle(EnumsAndConstants.cButtonIdleStyle));
                bPush.setDisable(m_Pushed);
                bPush.setOnAction(new EventHandler<ActionEvent>() {
                    @Override public void handle(ActionEvent e) 
                    {
                        m_Pushed = true;
                        RenderPanel();
                    }
                });  
                hbTitle.getChildren().add(bPush); 

                m_SkillLevel = m_Character.GetSkill(ActionAttempt.GetActionSkill()).GetExpertise();
                if (m_Pushed)
                    m_SkillLevel++;
                if (m_Character.GetHarmLevel() == 3)
                    m_SkillLevel--;

                m_IndividualDice.clear();
                if (m_SkillLevel > 0)
                {
                    for (int i = 1; i <= m_SkillLevel; i++)
                        AddDie();
                    RenderDicePool(false);
                }
                else
                {
                    for (int i = 1; i >= m_SkillLevel; i--)
                        AddDie();
                    RenderDicePool(true);
                }            
            }
        }
        else 
        {
            lTitle.setText("Skill Check (" + EnumsAndConstants.EnumToString(ActionAttempt.GetActionSkill()) + ") - Result: " + m_Result);
            
            if (m_SkillLevel > 0)
                RenderDicePool(false);
            else 
                RenderDicePool(true);
        }        
    }    
}
