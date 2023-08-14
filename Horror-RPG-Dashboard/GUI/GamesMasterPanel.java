/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.CharacterData;
import astonblades.Application.ProgressClock;
import astonblades.EnumsAndConstants;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.RadioButton;
import javafx.scene.control.ToggleGroup;
import javafx.scene.image.ImageView;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;

/**
 *
 * @author hp
 */
public class GamesMasterPanel extends VBox
{
    private ClockPanel m_ClockPanel = new ClockPanel();
    private ActionPanel m_ActionPanel = new ActionPanel();
    private AllCharactersPanel m_CharactersPanel;
    
    public GamesMasterPanel(double height, double width, AllCharactersPanel acp)
    {
        this.setPrefSize(width, height);
        this.setStyle(EnumsAndConstants.cBoxStyle);
        
        m_CharactersPanel = acp;
        
        RenderBrandImage();
        this.getChildren().addAll(RenderControlPanel(), m_ClockPanel, m_ActionPanel);
        ActionAttempt.Setup(m_ActionPanel, m_CharactersPanel, m_ClockPanel);
        m_ClockPanel.Update();
    }
    
    private void RenderBrandImage()
    {
        ImageView im = ImageProcessor.GetImage("BACKGROUND");
        im.setFitHeight(200); 
        im.setFitWidth(360);
        this.getChildren().add(im);
    }
    
    private GridPane RenderControlPanel()
    {
        GridPane gpControls = new GridPane();
        double buttonWidth = 180;//this.getWidth() / 2;
        
        Button bLoad = new Button("Load Game");
        bLoad.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bLoad.setOnMouseEntered(e -> bLoad.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bLoad.setOnMouseExited(e -> bLoad.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bLoad.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                CharacterData.LoadGame();
                m_CharactersPanel.ShowCharacters();
            }
        });
        bLoad.setPrefWidth(buttonWidth);
        
        Button bSave = new Button("Save Game");
        bSave.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bSave.setOnMouseEntered(e -> bSave.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bSave.setOnMouseExited(e -> bSave.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bSave.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                CharacterData.SaveGame();
            }
        });
        bSave.setPrefWidth(buttonWidth);
        
        Button bRest = new Button("Rest");
        bRest.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bRest.setOnMouseEntered(e -> bRest.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bRest.setOnMouseExited(e -> bRest.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bRest.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                if (!ActionAttempt.IsInProgress())
                {
                    CharacterData.RestCharacters();
                    m_CharactersPanel.Update();
                }
            }
        });
        bRest.setPrefWidth(buttonWidth);
        
        Button bAdd = new Button("Add Character");
        bAdd.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bAdd.setOnMouseEntered(e -> bAdd.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bAdd.setOnMouseExited(e -> bAdd.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bAdd.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_CharactersPanel.AddCharacter();
            }
        });
        bAdd.setPrefWidth(buttonWidth);
        
        gpControls.add(bLoad, 0, 0);
        gpControls.add(bSave, 1, 0);
        gpControls.add(bRest, 0, 1);
        gpControls.add(bAdd, 1, 1);
        
        return gpControls;
    }
    
    private HBox RenderActionPanel()
    {
        HBox hbActionPanel = new HBox();
        
        VBox vbRisk = new VBox();
        vbRisk.setStyle(EnumsAndConstants.cBoxStyle);
        ToggleGroup tgRisk = new ToggleGroup();
        for (EnumsAndConstants.eActionRisk r : EnumsAndConstants.eActionRisk.values())
        {
            RadioButton rb = new RadioButton(r.toString());
            rb.setToggleGroup(tgRisk);
            vbRisk.getChildren().add(rb);
        }
        
        VBox vbConsequence = new VBox();
        vbConsequence.setStyle(EnumsAndConstants.cBoxStyle);
        for (EnumsAndConstants.eConsequence c : EnumsAndConstants.eConsequence.values())
        {
            Label lConsequence = new Label(c.toString());
            Button bApply = new Button("Apply");
            bApply.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bApply.setOnMouseEntered(e -> bApply.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bApply.setOnMouseExited(e -> bApply.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bApply.setOnAction(new EventHandler<ActionEvent>() {
		    @Override public void handle(ActionEvent e) 
		    {
                        ProgressClock.ApplyEscalation(EnumsAndConstants.eActionRisk.CONTROLLED);
                        m_ClockPanel.Update();
		    }
		});
            vbConsequence.getChildren().add(new HBox(lConsequence, bApply));
        }
        
        hbActionPanel.getChildren().addAll(vbRisk, vbConsequence);
        return hbActionPanel;
    }
}
