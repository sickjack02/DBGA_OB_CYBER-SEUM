using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// questo script gestisce la visualizzazione della vita dei nemici
public class FloatingHealhtBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject damageTxt;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offsetSlider;
    [SerializeField] private Vector3 offsetDamage;

    // Update is called once per frame

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;

        // setta dove voglio lo slider
        transform.position = target.position + offsetSlider;
    }

    // aggiorna l'healthbar 
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth/maxHealth;
    }

    // mostra il danno accanto al nemico
    public void ShowDamage(string text)
    {
        if (damageTxt)
        {
            GameObject prefab = Instantiate(damageTxt, transform.position + offsetDamage, Quaternion.identity);
            prefab.GetComponentInChildren<TextMeshProUGUI>().text = text;
            Destroy(prefab, 1f);
        }
    }
}
