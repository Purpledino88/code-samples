/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.Randomiser;
import astonblades.EnumsAndConstants;
import java.util.Vector;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;

/**
 *
 * @author hp
 */
public class ActionPanel extends VBox
{
    private Vector<EnumsAndConstants.eConsequence> m_PrioritisedConsequences = new Vector<>();
    private final int mf_NumberOfPossibleConsequences = 3;
    
    private FortuneRollPanel m_FortunePanel = null;
    
    public ActionPanel()
    {
        NoActionInProgress();
        
        Vector<EnumsAndConstants.eConsequence> start_list = new Vector<>();
        for (EnumsAndConstants.eConsequence con : EnumsAndConstants.eConsequence.values())
        {
            start_list.add(con);
        }
        
        while (start_list.size() > 0)
        {
            EnumsAndConstants.eConsequence selected = start_list.elementAt(Randomiser.GetRandomInteger(0, start_list.size() - 1));
            m_PrioritisedConsequences.add(selected);
            start_list.remove(selected);
        }
    }
    
    public void Update()
    {
        this.getChildren().clear();
        if (ActionAttempt.IsInProgress())
        {
            switch (ActionAttempt.GetActionType())
            {
                case FORTUNE:
                {
                    FortuneRollInProgress();
                    break;
                }
                
                case SKILL:
                {
                    SkillRollInProgress();
                    break;
                }
                
                case SANITY:
                {
                    SanityRollInProgress();
                    break;
                }
            }
        }
        else
        {
            NoActionInProgress();
        }
    }
    
    private void NoActionInProgress()
    {
        Button bFortune = new Button("Fortune Roll");
        bFortune.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bFortune.setOnMouseEntered(e -> bFortune.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bFortune.setOnMouseExited(e -> bFortune.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bFortune.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {                
                m_FortunePanel = new FortuneRollPanel();
                ActionAttempt.FortuneRoll(m_FortunePanel);
            }
        });
        Label lSummary = new Label("No action in progress");
        lSummary.setTextFill(Color.WHITE);
        this.getChildren().addAll(lSummary, bFortune);
    }
    
    private void FortuneRollInProgress()
    {
        Label lSummary = new Label("Fortune Roll");
        lSummary.setTextFill(Color.WHITE);
        this.getChildren().add(lSummary);
        
        if (!ActionAttempt.HasBeenRolled())
        {
            Button bRoll = new Button("Roll");
            bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bRoll.setOnMouseEntered(e -> bRoll.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bRoll.setOnMouseExited(e -> bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bRoll.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    ActionAttempt.RollDice();
                }
            });
            this.getChildren().add(bRoll);
        }
        else
        {
            Button bClear = new Button("Clear");
            bClear.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bClear.setOnMouseEntered(e -> bClear.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bClear.setOnMouseExited(e -> bClear.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bClear.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    ActionAttempt.ApplyConsequence(null);                    
                    m_FortunePanel = null;
                }
            });
            this.getChildren().add(bClear);
        }        
        
        this.getChildren().add(m_FortunePanel);
        m_FortunePanel.RenderPanel();
    }
    
    private void SkillRollInProgress()
    {
        Label lSummary = new Label(EnumsAndConstants.EnumToString(ActionAttempt.GetActionSkill()) + " Check");
        lSummary.setTextFill(Color.WHITE);
        this.getChildren().add(lSummary);
        
        if (ActionAttempt.GetActionRisk() == null)
        {
            for (EnumsAndConstants.eActionRisk risk : EnumsAndConstants.eActionRisk.values())
            {
                Button bRisk = new Button(EnumsAndConstants.EnumToString(risk));
                bRisk.setStyle(EnumsAndConstants.cButtonIdleStyle);
                bRisk.setOnMouseEntered(e -> bRisk.setStyle(EnumsAndConstants.cButtonHoveredStyle));
                bRisk.setOnMouseExited(e -> bRisk.setStyle(EnumsAndConstants.cButtonIdleStyle));
                bRisk.setOnAction(new EventHandler<ActionEvent>() {
                    @Override public void handle(ActionEvent e) 
                    {
                        ActionAttempt.SetRisk(risk);
                        Update();
                    }
                });    
                this.getChildren().add(bRisk);
            }            
        }
        else if (!ActionAttempt.HasBeenRolled())
        {
            Button bRoll = new Button("Roll");
            bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bRoll.setOnMouseEntered(e -> bRoll.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bRoll.setOnMouseExited(e -> bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bRoll.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    ActionAttempt.RollDice();
                }
            });
            this.getChildren().add(bRoll);
        }
        else
        {
            if (ActionAttempt.HasConsequences())
            {
                for (int i = 0; i < mf_NumberOfPossibleConsequences; i++)
                {
                    EnumsAndConstants.eConsequence con = m_PrioritisedConsequences.elementAt(i);
                    Button bApply = new Button(EnumsAndConstants.EnumToString(con));
                    bApply.setStyle(EnumsAndConstants.cButtonIdleStyle);
                    bApply.setOnMouseEntered(e -> bApply.setStyle(EnumsAndConstants.cButtonHoveredStyle));
                    bApply.setOnMouseExited(e -> bApply.setStyle(EnumsAndConstants.cButtonIdleStyle));
                    bApply.setOnAction(new EventHandler<ActionEvent>() {
                        @Override public void handle(ActionEvent e) 
                        {
                            ActionAttempt.ApplyConsequence(con);
                            m_PrioritisedConsequences.remove(con);
                            m_PrioritisedConsequences.add(con);
                        }
                    });
                    this.getChildren().add(bApply);
                }
            }
            else
            {
                Button bClear = new Button("Clear");
                bClear.setStyle(EnumsAndConstants.cButtonIdleStyle);
                bClear.setOnMouseEntered(e -> bClear.setStyle(EnumsAndConstants.cButtonHoveredStyle));
                bClear.setOnMouseExited(e -> bClear.setStyle(EnumsAndConstants.cButtonIdleStyle));
                bClear.setOnAction(new EventHandler<ActionEvent>() {
                    @Override public void handle(ActionEvent e) 
                    {
                        ActionAttempt.ApplyConsequence(null);
                    }
                });
                this.getChildren().add(bClear);
            }
        }
    }
    
    private void SanityRollInProgress()
    {
        Label lSummary = new Label("Sanity Check");
        lSummary.setTextFill(Color.WHITE);
        this.getChildren().add(lSummary);
        
        if (ActionAttempt.GetActionRisk() == null)
        {
            for (EnumsAndConstants.eActionRisk risk : EnumsAndConstants.eActionRisk.values())
            {
                Button bRisk = new Button(EnumsAndConstants.EnumToString(risk));
                bRisk.setStyle(EnumsAndConstants.cButtonIdleStyle);
                bRisk.setOnMouseEntered(e -> bRisk.setStyle(EnumsAndConstants.cButtonHoveredStyle));
                bRisk.setOnMouseExited(e -> bRisk.setStyle(EnumsAndConstants.cButtonIdleStyle));
                bRisk.setOnAction(new EventHandler<ActionEvent>() {
                    @Override public void handle(ActionEvent e) 
                    {
                        ActionAttempt.SetRisk(risk);
                        Update();
                    }
                });    
                this.getChildren().add(bRisk);
            }            
        }
        else if (!ActionAttempt.HasBeenRolled())
        {
            Button bRoll = new Button("Roll");
            bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bRoll.setOnMouseEntered(e -> bRoll.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bRoll.setOnMouseExited(e -> bRoll.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bRoll.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    ActionAttempt.RollDice();
                }
            });
            this.getChildren().add(bRoll);
        }
        else
        {
            Button bApply = new Button();
            bApply.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bApply.setOnMouseEntered(e -> bApply.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bApply.setOnMouseExited(e -> bApply.setStyle(EnumsAndConstants.cButtonIdleStyle));
            if (ActionAttempt.HasConsequences())
                bApply.setText("Apply Stress");
            else bApply.setText("Clear");
            bApply.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    ActionAttempt.ApplyConsequence(null);
                }
            });
            this.getChildren().add(bApply);
        }
    }
}
