/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package astonblades.GUI;

/**
 *
 * @author hp
 */
public class FortuneRollPanel extends DicePoolPanel
{
    public FortuneRollPanel()
    {
        AddDie();
    }

    @Override
    protected void ResolveResult() 
    {
        m_Result = m_IndividualDice.get(0).GetValue();
    }

    @Override
    public void RenderPanel() 
    {
        this.getChildren().clear();
        RenderDicePool(false);
    }

    @Override
    protected void RenderDicePool(boolean use_alternate_images) 
    {
        this.getChildren().add(RenderDie(m_IndividualDice.get(0), use_alternate_images));
    }    
}
