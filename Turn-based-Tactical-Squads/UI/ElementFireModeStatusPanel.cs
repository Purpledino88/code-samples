using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementFireModeStatusPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private TMPro.TextMeshProUGUI m_ShotsField;
    [SerializeField] private TMPro.TextMeshProUGUI m_HitChanceField;
    [SerializeField] private Image m_DamageImage;
    [SerializeField] private Sprite m_BulletSprite;
    [SerializeField] private Sprite m_BlastSprite;

    public void UpdateDetails(WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode fire_mode, UnitElement element, Unit target_unit, float cover_modifier, bool element_is_active)
    {
        m_NameField.text = fire_mode.mode_name;
        m_NameField.color = (element_is_active ? Color.black : Color.grey);

        m_DamageImage.color = (element_is_active ? Color.white : Color.grey);
        if (fire_mode.blast_chance_to_hit > 0f)
            m_DamageImage.sprite = m_BlastSprite;
        else
            m_DamageImage.sprite = m_BulletSprite;

        m_ShotsField.color = (element_is_active ? Color.red : Color.grey);
        int l_NumberOfShots = fire_mode.shots_per_action;
        if (l_NumberOfShots > 1)
        {
            m_ShotsField.gameObject.SetActive(true);
            m_ShotsField.text = "x" + l_NumberOfShots;
        }
        else
            m_ShotsField.gameObject.SetActive(false);

        Image l_ButtonImage = GetComponent<Image>();
        if (element.IsAbleToAct())
        {
            if (element.GetSelectedFireMode() == fire_mode)
                l_ButtonImage.color = new Color(l_ButtonImage.color.r, l_ButtonImage.color.g, l_ButtonImage.color.b, 1f);
            else
                l_ButtonImage.color = new Color(l_ButtonImage.color.r, l_ButtonImage.color.g, l_ButtonImage.color.b, 0f);

            if (target_unit != null)
            {
                m_HitChanceField.gameObject.SetActive(true);
                int l_RecoilApproximation = Mathf.RoundToInt(0.5f * fire_mode.recoil * (fire_mode.shots_per_action - 1));
                int l_HitChanceWithRecoil = Mathf.Clamp(element.CalculateHitChance(target_unit, weapon, fire_mode, cover_modifier) - l_RecoilApproximation, 0, 100);
                m_HitChanceField.text = (fire_mode.recoil > 0f ? "~" : "") + l_HitChanceWithRecoil + "%";
            }
            else
            {
                m_HitChanceField.gameObject.SetActive(false);
            }
        }
        else
        {
            l_ButtonImage.color = new Color(l_ButtonImage.color.r, l_ButtonImage.color.g, l_ButtonImage.color.b, 0f);
            m_HitChanceField.gameObject.SetActive(false);
        }
    }
}