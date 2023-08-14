/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.CharacterData;
import astonblades.Application.SkillInfo;
import astonblades.EnumsAndConstants;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.image.ImageView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.TilePane;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Circle;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.TextAlignment;

/**
 *
 * @author hp
 */
public class CharacterPanel extends VBox
{
    private final CharacterData m_Character;
    private DicePoolPanel m_DicePoolPanel = null;
    
    public CharacterPanel(double height, double width, CharacterData character, AllCharactersPanel parent)
    {
        this.setPrefSize(width, height);
        this.setMinSize(width, height);
        this.setStyle(EnumsAndConstants.cBoxStyle);
        
        m_Character = character;
        
        this.getChildren().addAll(RenderSummary(), RenderStatus());
    }
    
    private void RenderDicePool(DicePoolPanel panel)
    {
        if (m_DicePoolPanel == null)
        {
            m_DicePoolPanel = panel;
            this.getChildren().add(m_DicePoolPanel);
        }
        m_DicePoolPanel.RenderPanel();
    }
    
    private VBox RenderStatus()
    {
        VBox vb = new VBox();
        
        switch (m_Character.GetCurrentStatus())
        {
            case ACTIVE:
            {
                vb.getChildren().addAll(RenderHealthAndStress(), RenderAllSkills());              
                return vb;
            }
            case DEAD:
            {
                ImageView im = ImageProcessor.GetImage("STATUS_DEAD");
                im.setFitHeight(600);
                im.setFitWidth(350);
                vb.getChildren().addAll(im);
                return vb;
            }
            case INSANE:
            {
                ImageView im = ImageProcessor.GetImage("STATUS_INSANE");
                im.setFitHeight(600);
                im.setFitWidth(350);
                vb.getChildren().addAll(im);
                return vb;
            }
        }
        
        return vb;
    }
    
    private TilePane RenderAllSkills()
    {
        TilePane tpAllSkills = new TilePane();
        tpAllSkills.setPrefColumns(2);
        for (EnumsAndConstants.eSkills s : EnumsAndConstants.eSkills.values())
        {
            tpAllSkills.getChildren().add(RenderSkill(m_Character.GetSkill(s)));
        }
        return tpAllSkills;
    }
    
    private HBox RenderSkill(SkillInfo skill)
    {
        HBox hbSkillBox = new HBox();
        hbSkillBox.setStyle(EnumsAndConstants.cBoxStyle);
        
        Button bRollSkill = new Button(skill.GetName());
        bRollSkill.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bRollSkill.setOnMouseEntered(e -> bRollSkill.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bRollSkill.setOnMouseExited(e -> bRollSkill.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bRollSkill.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                SkillCheckPanel scp = new SkillCheckPanel(m_Character);
                if (ActionAttempt.SkillRoll(m_Character, scp, skill.GetSkillType()))
                    RenderDicePool(scp);
            }
        });        
        hbSkillBox.getChildren().add(bRollSkill);
        
        double radius = 6;
        for (int i = 1; i < 4; i++)
        {
            Circle circle = new Circle(radius);
            if (skill.GetExpertise() >= i)
                circle.setFill(Color.WHITE);
            else circle.setFill(null);
            circle.setStroke(Color.WHITE);
            circle.setStrokeWidth(1);            
            hbSkillBox.getChildren().add(circle);
        }
        
        return hbSkillBox;
    }
    
    private HBox RenderSummary()
    {
        HBox hbCharSummary = new HBox();
        
        VBox vbSummary = new VBox();
        
        Label lName = new Label(m_Character.GetName());
        lName.setTextFill(Color.WHITE);
        Label lDesc = new Label(m_Character.GetDescription());
        lDesc.setTextFill(Color.WHITE);
        Label lFears = new Label("Fears:");
        lFears.setTextFill(Color.WHITE);
        vbSummary.getChildren().addAll(lName, lDesc, lFears);
        for (EnumsAndConstants.ePhobia p : m_Character.GetPhobiasList())
        {
            Label lPhobia = new Label("   " + EnumsAndConstants.EnumToString(p));
            lPhobia.setTextFill(Color.WHITE);
            vbSummary.getChildren().add(lPhobia);
        }            
        
        if (m_Character.GetCurrentStatus() == EnumsAndConstants.eCharacterStatus.ACTIVE)
        {
            Button bRollSanity = new Button("Sanity Check");
            bRollSanity.setStyle(EnumsAndConstants.cButtonIdleStyle);
            bRollSanity.setOnMouseEntered(e -> bRollSanity.setStyle(EnumsAndConstants.cButtonHoveredStyle));
            bRollSanity.setOnMouseExited(e -> bRollSanity.setStyle(EnumsAndConstants.cButtonIdleStyle));
            bRollSanity.setOnAction(new EventHandler<ActionEvent>() {
                @Override public void handle(ActionEvent e) 
                {
                    SanityCheckPanel scp = new SanityCheckPanel(m_Character);
                    if (ActionAttempt.SanityRoll(m_Character, scp))
                        RenderDicePool(scp);
                }
            });        
            vbSummary.getChildren().add(bRollSanity);
        }
        
        StackPane im_stack = new StackPane();
        
        String str = "CHARACTER_" + m_Character.GetDescription();
        str = str.replace(" ", "_");
        str = str.toUpperCase();
        ImageView im_char = ImageProcessor.GetImage(str);
        im_char.setFitWidth(120);
        im_char.setFitHeight(180);
        im_stack.getChildren().add(im_char);
        
        ImageView im_frame = ImageProcessor.GetImage("CHARACTER_PORTRAIT_FRAME");
        im_frame.setFitWidth(180);
        im_frame.setFitHeight(240);
        im_stack.getChildren().add(im_frame);
        
        hbCharSummary.getChildren().addAll(im_stack, vbSummary);
        return hbCharSummary;
    }
    
    private HBox RenderHealthAndStress()
    {
        return new HBox(RenderHealthBar(), RenderStress());
    }
    
    private VBox RenderStress()
    {
        VBox vbStressBar = new VBox();
        
        Label lTitle = new Label("Stress / Insanity");
        lTitle.setTextFill(Color.WHITE);
        HBox hbLabels = new HBox(lTitle);
        vbStressBar.getChildren().add(hbLabels);
        
        HBox hbBoxes = new HBox();
        for (int i = 0; i < 10; i++)
        {
            Rectangle rect = new Rectangle(16, 77);
            
            if (i < m_Character.GetCurrentStress())
                rect.setFill(Color.RED);
            else 
            {
                if ((9 - i) < m_Character.GetCurrentInsanity())
                    rect.setFill(Color.MIDNIGHTBLUE);
                else rect.setFill(Color.BLUE);
            }
            
            rect.setStroke(Color.BLACK);
            rect.setStrokeWidth(1);            
            hbBoxes.getChildren().add(rect);
        }
        vbStressBar.getChildren().add(hbBoxes);
        
        return vbStressBar;
    }
    
    private VBox RenderHealthBar()
    {
        VBox hbHealthBar = new VBox();
        hbHealthBar.setPrefWidth(180);
        Label lStatus = new Label();
        lStatus.setTextFill(Color.WHITE);
        lStatus.setTextAlignment(TextAlignment.CENTER);
        switch (m_Character.GetHarmLevel())
        {
            case 0:
            {
                lStatus.setText("Healthy");
                break;
            }
            case 1:
            {
                lStatus.setText("Hurt");
                break;
            }
            case 2:
            {
                lStatus.setText("Injured");
                break;
            }
            case 3:
            {
                lStatus.setText("Near-Death");
                break;
            }
        }        
        hbHealthBar.getChildren().add(lStatus);
        
        double healthWidth = 180;
        VBox vbHealthStack = new VBox();
        
        int health_index = m_Character.GetCurrentHealth().length - 1;
        for (int i = 1; i < 4; i++)
        {
            HBox level = new HBox();
            for (int j = 0; j < i; j++)
            {
                double boxWidth = (healthWidth - ((i - 1) * 2)) / i;
                Rectangle harm = new Rectangle(boxWidth, 25);
                if (m_Character.GetCurrentHealth()[health_index])
                    harm.setFill(Color.GREEN);
                else harm.setFill(Color.RED);
                health_index--;
                harm.setStroke(Color.BLACK);
                harm.setStrokeWidth(1);            
                level.getChildren().add(harm);
            }
            vbHealthStack.getChildren().add(level);
        }
        hbHealthBar.getChildren().add(vbHealthStack);
        
        return hbHealthBar;
    }
}
