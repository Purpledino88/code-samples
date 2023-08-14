/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

import astonblades.Application.ActionAttempt;
import astonblades.Application.SixSidedDie;
import java.util.Vector;
import javafx.scene.layout.VBox;
import javafx.scene.image.ImageView;

/**
 *
 * @author hp
 */
public abstract class DicePoolPanel extends VBox
{
    protected Vector<SixSidedDie> m_IndividualDice = new Vector<>();
    protected int m_Result = 0;
    protected boolean m_Rolled = false;

    public void RollDice()
    {
        for (SixSidedDie d : m_IndividualDice)
            d.RollDie();
        
        m_Rolled = true;
        ResolveResult();
    }
    
    public void ResetPanel()
    {
        m_IndividualDice.clear();
        m_Result = 0;
        m_Rolled = false;
    }
    
    protected void AddDie()
    {
        m_IndividualDice.add(new SixSidedDie());
    }
    
    public int GetResult() { return m_Result; }
    
    protected abstract void ResolveResult();
    
    public abstract void RenderPanel();
    
    protected abstract void RenderDicePool(boolean use_alternate_images);
    
    protected ImageView RenderDie(SixSidedDie die, boolean use_alternate_images)
    {
        return ImageProcessor.GetImage(ActionAttempt.GetActionType().toString() + "_DICE_" + die.GetValue() + (use_alternate_images ? "_ALT" : ""));
    }
}
