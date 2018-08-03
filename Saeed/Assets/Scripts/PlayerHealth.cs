using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [Header("Values")]
    public int maxHealth;

    [Header("External Info")]
    public PlayerHealthDisplay UIDisplay;

    public int value { get; private set; }
    [HideInInspector] static public int deathCount;

    private void Start()
    {
        value = maxHealth;
        //UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (UIDisplay)
        {
            UIDisplay.ChangeFill((float) value / maxHealth);
        }
    } 

    public void SetDamage(int damage)
    {
        value -= damage;
        UpdateDisplay();
        if(value < 1)
        {
            GameManager.instance.ResetScene();
        }
    }
}
