/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ProgressClock;
import astonblades.EnumsAndConstants;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.RadioButton;
import javafx.scene.control.TextField;
import javafx.scene.control.Toggle;
import javafx.scene.control.ToggleGroup;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.Font;

/**
 *
 * @author hp
 */
public class ClockPanel extends VBox
{
    public void Update()
    {
        this.setStyle(EnumsAndConstants.cBoxStyle);
        this.getChildren().clear();
        
        HBox hbControl = new HBox();
        Label lPoints = new Label(String.valueOf(ProgressClock.GetEscalationPoints()));
        lPoints.setFont(new Font("Arial", 64));
        lPoints.setTextFill(Color.WHITE);
        hbControl.getChildren().add(lPoints);
        
        ToggleGroup tgLength = new ToggleGroup();
        RadioButton rb4 = new RadioButton("Short");
        rb4.setTextFill(Color.WHITE);
        rb4.setToggleGroup(tgLength);
        RadioButton rb6 = new RadioButton("Medium");
        rb6.setTextFill(Color.WHITE);
        rb6.setToggleGroup(tgLength);
        RadioButton rb8 = new RadioButton("Long");
        rb8.setTextFill(Color.WHITE);
        rb8.setToggleGroup(tgLength);
        VBox vbLength = new VBox(rb4, rb6, rb8);
        
        TextField tfClockName = new TextField();
        
        
        Button bAddClock = new Button("Add Clock");
        bAddClock.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bAddClock.setOnMouseEntered(e -> bAddClock.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bAddClock.setOnMouseExited(e -> bAddClock.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bAddClock.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                Toggle t = tgLength.getSelectedToggle();
                if (t != null && tfClockName.getText() != "")
                {
                    if (tgLength.getSelectedToggle().equals(rb4))
                        ProgressClock.AddNewClock(tfClockName.getText(), 4);
                    if (tgLength.getSelectedToggle().equals(rb6))
                        ProgressClock.AddNewClock(tfClockName.getText(), 6);
                    if (tgLength.getSelectedToggle().equals(rb8))
                        ProgressClock.AddNewClock(tfClockName.getText(), 8);
                    
                    Update();
                }
            }
        });
        
        Button bEscalate = new Button("Escalate");
        bEscalate.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bEscalate.setOnMouseEntered(e -> bEscalate.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bEscalate.setOnMouseExited(e -> bEscalate.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bEscalate.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                ProgressClock.ApplyEscalation(EnumsAndConstants.eActionRisk.CONTROLLED);
                Update();
            }
        });
        
        HBox hbButtons = new HBox(bEscalate, bAddClock);
        VBox vbClockInfo = new VBox(tfClockName, hbButtons);
        
        hbControl.getChildren().add(new HBox(vbClockInfo, vbLength));
        
        this.getChildren().add(hbControl);
        
        for (ProgressClock pc : ProgressClock.GetAllClocks())
            RenderClock(pc);
    }
    
    private void RenderClock(ProgressClock pc)
    {
        HBox hbClock = new HBox();
        hbClock.setStyle(EnumsAndConstants.cBoxStyle);
        
        Label lName = new Label(pc.GetClockName());
        lName.setTextFill(Color.WHITE);
        hbClock.getChildren().add(lName);
        
        for (int i = 0; i < pc.GetClockLength(); i++)
        {
            Rectangle rect = new Rectangle(16, 16);
            if (i < pc.GetClockProgress())
                rect.setFill(Color.RED);
            else rect.setFill(null);
            rect.setStroke(Color.WHITE);
            rect.setStrokeWidth(1);            
            hbClock.getChildren().add(rect);
        }
        
        if (pc.GetClockProgress() == pc.GetClockLength())
        {
            Button bDeleteClock = new Button("X");
            bDeleteClock.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bDeleteClock.setOnMouseEntered(e -> bDeleteClock.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bDeleteClock.setOnMouseExited(e -> bDeleteClock.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bDeleteClock.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    pc.DeleteClock();
                    Update();
                }
            });
            hbClock.getChildren().add(bDeleteClock);
        }
        else
        {
            Button bIncrementClock = new Button("+");
            bIncrementClock.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bIncrementClock.setOnMouseEntered(e -> bIncrementClock.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bIncrementClock.setOnMouseExited(e -> bIncrementClock.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bIncrementClock.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    pc.IncrementClock();
                    Update();
                }
            });            
            hbClock.getChildren().add(bIncrementClock);
        }
        
        this.getChildren().add(hbClock);
    }
}
