using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementStatusPanel : MonoBehaviour
{
    [SerializeField] private ElementHealthIndicator m_HealthIndicator;
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private Image m_StunnedImage;
    [SerializeField] private Image m_BleedingImage;
    [SerializeField] private GameObject m_WeaponDetailsCollection;
    [SerializeField] private GameObject m_WeaponDetailsPrefab;
    [SerializeField] private GameObject m_FireModeDetailsPrefab;

    public void UpdateDetails(UnitElement element, Unit target_unit, float cover_modifier)
    {
        foreach (Transform child in m_WeaponDetailsCollection.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        m_HealthIndicator.ShowHealth(element);
        if (element != null)
        {
            m_NameField.text = element.GetName();
            m_NameField.color = Color.black;
            m_StunnedImage.gameObject.SetActive(element.IsStunned());
            m_BleedingImage.gameObject.SetActive(element.IsBleeding());
        }
        else
        {
            m_NameField.text = "K.I.A.";
            m_NameField.color = Color.red;
        }

        foreach (WeaponScriptableObject weapon in element.GetAllWeapons())
        {
            ElementWeaponStatusPanel l_WeaponStatusPanel = Instantiate(m_WeaponDetailsPrefab, m_WeaponDetailsCollection.transform).GetComponent<ElementWeaponStatusPanel>();
            
            l_WeaponStatusPanel.UpdateDetails(weapon, element.IsAbleToAct());

            foreach (WeaponScriptableObject.WeaponFireMode fire_mode in weapon.all_fire_modes)
            {
                ElementFireModeStatusPanel l_ModeStatusPanel = Instantiate(m_FireModeDetailsPrefab, l_WeaponStatusPanel.GetCollectionGameObject().transform).GetComponent<ElementFireModeStatusPanel>();
                l_ModeStatusPanel.UpdateDetails(weapon, fire_mode, element, target_unit, cover_modifier, element.IsAbleToAct());

                Button l_Button = l_ModeStatusPanel.GetComponent<Button>();
                l_Button.onClick.AddListener(delegate
                {
                    element.SetWeaponAndFiringMode(weapon, fire_mode);
                    UpdateDetails(element, null, 0f);
                });
            }
        }
    }
}
