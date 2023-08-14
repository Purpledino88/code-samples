/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.CharacterData;
import astonblades.Application.ProfessionInfo;
import astonblades.Application.RandomNameGenerator;
import astonblades.Application.SkillInfo;
import astonblades.EnumsAndConstants;
import java.util.Vector;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.image.ImageView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Circle;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.Font;

/**
 *
 * @author hp
 */
public class CreateCharacterPanel extends VBox
{
    private enum eSkillGroups
    {
        INSIGHT,
        KNOWLEDGE,
        PROWESS,
        RESOLVE
    }
    
    private int m_CurrentStage = 0;
    
    private EnumsAndConstants.eGender m_Gender = null;
    private eSkillGroups m_Strength = null;
    private eSkillGroups m_Weakness = null;
    private ProfessionInfo m_Profession = null;
    private Vector<SkillInfo> m_Skills;
    private Vector<EnumsAndConstants.ePhobia> m_Phobias;
    
    private AllCharactersPanel m_ParentPanel;
    
    public CreateCharacterPanel(AllCharactersPanel acp)
    {
        this.setStyle(EnumsAndConstants.cBoxStyle);
        m_Skills = SkillInfo.GenerateSkillList();        
        m_Phobias = new Vector<EnumsAndConstants.ePhobia>();
        m_ParentPanel = acp;
        Update();
    }
    
    private void Discard()
    {
        m_Gender = null;
        m_Strength = null;
        m_Weakness = null;
        m_Profession = null;
        m_Skills = SkillInfo.GenerateSkillList();
        
        m_CurrentStage = 0;
        
        Update();
    }
    
    private void Update()
    {
        this.getChildren().clear();

        Label lTitle = new Label("New Character");
        lTitle.setFont(new Font("Arial", 64));
        lTitle.setTextFill(Color.WHITE);
        
        Button bExit = new Button("Exit");
        bExit.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bExit.setOnMouseEntered(e -> bExit.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bExit.setOnMouseExited(e -> bExit.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bExit.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_ParentPanel.ShowCharacters();
            }
        });   
        
        HBox hbTitle = new HBox(lTitle, bExit);
           
        HBox hbBody = new HBox(RenderSummaryPanel(), RenderSelectionPanel());
        
        this.getChildren().addAll(hbTitle, hbBody);
    }
    
    private VBox RenderSummaryPanel()
    {
        VBox vbSummary = new VBox();
        
        Button bDiscard = new Button("Discard Character");
        bDiscard.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bDiscard.setOnMouseEntered(e -> bDiscard.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bDiscard.setOnMouseExited(e -> bDiscard.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bDiscard.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                Discard();
            }
        });
        vbSummary.getChildren().add(bDiscard);
        
        Label lGender = new Label("Gender: " + (m_Gender != null ? m_Gender.toString() : ""));
        lGender.setTextFill(Color.WHITE);
        Label lStrength = new Label("Strength: " + (m_Strength != null ? m_Strength.toString() : ""));
        lStrength.setTextFill(Color.WHITE);
        Label lWeakness = new Label("Weakness: " + (m_Weakness != null ? m_Weakness.toString() : ""));
        lWeakness.setTextFill(Color.WHITE);
        Label lProfession = new Label("Profession: " + (m_Profession != null ? m_Profession.GetProfessionName() : ""));
        lProfession.setTextFill(Color.WHITE);
        Label lFear = new Label("Fear: " + (!m_Phobias.isEmpty() ? m_Phobias.elementAt(0).toString() : ""));
        lFear.setTextFill(Color.WHITE);
        vbSummary.getChildren().addAll(lGender, lStrength, lWeakness, lProfession, lFear);
        
        vbSummary.getChildren().add(RenderAllSkills());
        
        return vbSummary;
    }
    
    private VBox RenderAllSkills()
    {
        VBox vbAllSkills = new VBox();
        for (SkillInfo s : m_Skills)
        {
            vbAllSkills.getChildren().add(RenderSkill(s));
        }        
        return vbAllSkills;
    }
    
    private HBox RenderSkill(SkillInfo skill)
    {
        HBox hbSkillBox = new HBox();
        hbSkillBox.setStyle(EnumsAndConstants.cBoxStyle);
        
        Label lSkill = new Label(skill.GetName());
        lSkill.setTextFill(Color.WHITE);
        hbSkillBox.getChildren().add(lSkill);
        
        double radius = 8;
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
    
    private VBox RenderSelectionPanel()
    {
        VBox vb = new VBox();
        
        switch (m_CurrentStage)
        {
            case 0:
            {
                for (EnumsAndConstants.eGender g : EnumsAndConstants.eGender.values())
                    vb.getChildren().add(GenderBox(g));
                break;
            }
            
            case 1:
            {
                for (eSkillGroups sg : eSkillGroups.values())                    
                    vb.getChildren().add(StrengthBox(sg));
                break;
            }
            
            case 2:
            {
                for (eSkillGroups sg : eSkillGroups.values())   
                {
                    if (sg != m_Strength)
                        vb.getChildren().add(WeaknessBox(sg));
                }
                break;                    
            }
            
            case 3:
            {
                switch (m_Weakness)
                {
                    case INSIGHT:
                    {
                        for (ProfessionInfo p : ProfessionInfo.GetAllAvailableProfessions(EnumsAndConstants.eSkills.INVESTIGATION, EnumsAndConstants.eSkills.PERCEPTION, EnumsAndConstants.eSkills.TECHNOLOGY))
                        {
                            vb.getChildren().add(ProfessionBox(p));
                        }
                        break;
                    }
                    case KNOWLEDGE:
                    {
                        for (ProfessionInfo p : ProfessionInfo.GetAllAvailableProfessions(EnumsAndConstants.eSkills.MEDICINE, EnumsAndConstants.eSkills.SCIENCE, EnumsAndConstants.eSkills.MYSTICISM))
                        {
                            vb.getChildren().add(ProfessionBox(p));
                        }
                        break;
                    }
                    case PROWESS:
                    {
                        for (ProfessionInfo p : ProfessionInfo.GetAllAvailableProfessions(EnumsAndConstants.eSkills.STRENGTH, EnumsAndConstants.eSkills.STEALTH, EnumsAndConstants.eSkills.DEXTERITY))
                        {
                            vb.getChildren().add(ProfessionBox(p));
                        }
                        break;
                    }
                    case RESOLVE:
                    {
                        for (ProfessionInfo p : ProfessionInfo.GetAllAvailableProfessions(EnumsAndConstants.eSkills.WILL, EnumsAndConstants.eSkills.COMMAND, EnumsAndConstants.eSkills.PERSUASION))
                        {
                            vb.getChildren().add(ProfessionBox(p));
                        }
                        break;
                    }
                } 
                break;                    
            }
            
            case 8:
            {
                for (EnumsAndConstants.ePhobia p : EnumsAndConstants.ePhobia.values())   
                {
                    vb.getChildren().add(PhobiaBox(p));
                }
                break;                     
            }
            
            case 9:
            {
                vb.getChildren().add(NamePanel());
                break;                    
            }
            
            default:
            {
                for (SkillInfo s : m_Skills)
                {
                    if (s.GetTrainable() && s.GetExpertise() < 3)
                        vb.getChildren().add(SkillBox(s));
                }
                break;
            }
        }
        
        return vb;
    }
    
    private VBox NamePanel()
    {
        VBox vb = new VBox();
        
        String image_file = "CHARACTER_" + m_Gender.toString() + "_" + m_Profession.GetProfessionName().toUpperCase();
        image_file = image_file.replace(" ", "_");
        ImageView im = ImageProcessor.GetImage(image_file);
        im.setFitHeight(480);
        im.setFitWidth(360);
        System.out.println(image_file);
        
        Label lName = new Label(GetRandomName());
        lName.setFont(new Font("Arial", 64));
        
        Button bRename = new Button("Re-roll name");
        bRename.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {     
                lName.setText(GetRandomName());
            }
        });
        
        Button bAdd = new Button("Add Character");
        bAdd.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                CharacterData.AddNewCharacter(new CharacterData(lName.getText(), EnumsAndConstants.EnumToString(m_Gender) + " " + m_Profession.GetProfessionName(), m_Skills, m_Phobias));
                m_ParentPanel.ShowCharacters();
            }
        });
        
        vb.getChildren().addAll(im, lName, bRename, bAdd);
        
        return vb;
    }
    
    private String GetRandomName()
    {
        switch (m_Gender)
        {
            case MALE:
            {
                return m_Profession.GetProfessionTitle() + RandomNameGenerator.GetMaleName();
            }
            case FEMALE:
            {
                return m_Profession.GetProfessionTitle() + RandomNameGenerator.GetFemaleName();
            }
        }
        return "UNKNOWN GENDER";
    }
    
    private HBox GenderBox(EnumsAndConstants.eGender gender)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.WHITE);
        
        Label lTitle = new Label(EnumsAndConstants.EnumToString(gender));
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bSelect.setOnMouseEntered(e -> bSelect.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bSelect.setOnMouseExited(e -> bSelect.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_Gender = gender;
                m_CurrentStage = 1;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private HBox PhobiaBox(EnumsAndConstants.ePhobia phobia)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.WHITE);
        
        Label lTitle = new Label(EnumsAndConstants.EnumToString(phobia));
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setStyle(EnumsAndConstants.cButtonIdleStyle);
        bSelect.setOnMouseEntered(e -> bSelect.setStyle(EnumsAndConstants.cButtonHoveredStyle));
        bSelect.setOnMouseExited(e -> bSelect.setStyle(EnumsAndConstants.cButtonIdleStyle));
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_Phobias.add(phobia);
                m_CurrentStage = 9;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private HBox StrengthBox(eSkillGroups strength)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.BLUE);
        
        String str = strength.toString();
        Label lTitle = new Label(str.substring(0, 1).toUpperCase() + str.substring(1).toLowerCase());
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_Strength = strength;
                ApplyStrength();
                m_CurrentStage = 2;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private HBox WeaknessBox(eSkillGroups weakness)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.RED);
        
        String str = weakness.toString();
        Label lTitle = new Label(str.substring(0, 1).toUpperCase() + str.substring(1).toLowerCase());
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_Weakness = weakness;
                ApplyWeakness();
                m_CurrentStage = 3;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private HBox ProfessionBox(ProfessionInfo job)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.BLACK);
        
        Label lTitle = new Label(job.GetProfessionName());
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                m_Profession = job;
                for (SkillInfo s : m_Skills)
                {
                    if (m_Profession.GetSkills().contains(s.GetSkillType()))
                    {
                        s.IncrementExpertise();
                        s.IncrementExpertise();
                    }
                }
                m_CurrentStage = 4;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private HBox SkillBox(SkillInfo skill)
    {
        HBox hb = new HBox();
        
        Rectangle im = new Rectangle(64, 64);
        im.setFill(Color.YELLOW);
        
        Label lTitle = new Label(skill.GetName());
        lTitle.setTextFill(Color.WHITE);
        
        Button bSelect = new Button("Select");
        bSelect.setOnAction(new EventHandler<ActionEvent>() {
            @Override public void handle(ActionEvent e) 
            {
                skill.IncrementExpertise();
                m_CurrentStage++;
                Update();
            }
        });
        
        hb.getChildren().addAll(im, lTitle, bSelect);
        
        return hb;
    }
    
    private void ApplyStrength()
    {
        switch (m_Strength)
        {
            case INSIGHT:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.PERCEPTION) || (si.GetSkillType() == EnumsAndConstants.eSkills.TECHNOLOGY) || (si.GetSkillType() == EnumsAndConstants.eSkills.INVESTIGATION))
                        si.IncrementExpertise();
                }
                return;
            }
            
            case KNOWLEDGE:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.MYSTICISM) || (si.GetSkillType() == EnumsAndConstants.eSkills.SCIENCE) || (si.GetSkillType() == EnumsAndConstants.eSkills.MEDICINE))
                        si.IncrementExpertise();
                }
                return;
            }
            
            case PROWESS:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.STEALTH) || (si.GetSkillType() == EnumsAndConstants.eSkills.STRENGTH) || (si.GetSkillType() == EnumsAndConstants.eSkills.DEXTERITY))
                        si.IncrementExpertise();
                }
                return;
            }
            
            case RESOLVE:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.WILL) || (si.GetSkillType() == EnumsAndConstants.eSkills.COMMAND) || (si.GetSkillType() == EnumsAndConstants.eSkills.PERSUASION))
                        si.IncrementExpertise();
                }
                return;
            }
        }
    }
    
    private void ApplyWeakness()
    {
        switch (m_Weakness)
        {
            case INSIGHT:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.PERCEPTION) || (si.GetSkillType() == EnumsAndConstants.eSkills.TECHNOLOGY) || (si.GetSkillType() == EnumsAndConstants.eSkills.INVESTIGATION))
                        si.SetTrainable(false);
                }
                return;
            }
            
            case KNOWLEDGE:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.MYSTICISM) || (si.GetSkillType() == EnumsAndConstants.eSkills.SCIENCE) || (si.GetSkillType() == EnumsAndConstants.eSkills.MEDICINE))
                        si.SetTrainable(false);
                }
                return;
            }
            
            case PROWESS:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.STEALTH) || (si.GetSkillType() == EnumsAndConstants.eSkills.STRENGTH) || (si.GetSkillType() == EnumsAndConstants.eSkills.DEXTERITY))
                        si.SetTrainable(false);
                }
                return;
            }
            
            case RESOLVE:
            {
                for (SkillInfo si : m_Skills)
                {
                    if ((si.GetSkillType() == EnumsAndConstants.eSkills.WILL) || (si.GetSkillType() == EnumsAndConstants.eSkills.COMMAND) || (si.GetSkillType() == EnumsAndConstants.eSkills.PERSUASION))
                        si.SetTrainable(false);
                }
                return;
            }
        }
    }
}
