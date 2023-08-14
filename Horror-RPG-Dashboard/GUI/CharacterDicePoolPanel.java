/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.CharacterData;
import astonblades.Application.SixSidedDie;
import javafx.scene.layout.TilePane;

/**
 *
 * @author hp
 */
public abstract class CharacterDicePoolPanel extends DicePoolPanel
{
    protected CharacterData m_Character;
    
    protected void RenderDicePool(boolean use_alternate_images)
    {
        TilePane tpAllDice = new TilePane();
        tpAllDice.setPrefRows(2);
        tpAllDice.setPrefColumns(2);
        
        for (SixSidedDie d : m_IndividualDice)
        {
            tpAllDice.getChildren().add(RenderDie(d, use_alternate_images));
        }
        
        this.getChildren().add(tpAllDice);
    }
}
