using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementWeaponStatusPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private Image m_Image;

    [SerializeField] private GameObject m_FireModeDetailsCollection;
    public GameObject GetCollectionGameObject() { return m_FireModeDetailsCollection; }

    public void UpdateDetails(WeaponScriptableObject weapon, bool element_is_active)
    {
        m_NameField.text = weapon.weapon_name;
        m_NameField.color = (element_is_active ? Color.black : Color.grey);
        m_Image.sprite = weapon.sprite;
        m_Image.color = (element_is_active ? Color.black : Color.grey);
    }
}
