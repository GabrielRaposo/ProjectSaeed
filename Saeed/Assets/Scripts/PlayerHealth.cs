using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour {
    [Header("Values")]
    public int maxHealth;

    [Header("External Info")]
    public TextMeshProUGUI UIDisplay;

    public int value { get; private set; }

    private void Start()
    {
        value = maxHealth;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (UIDisplay)
            UIDisplay.text = "Health: " + value.ToString("0");
    }

    public void TakeDamage()
    {
        value--;
        UpdateScore();
        if(value < 1)
        {
            GameManager.instance.ResetScene();
        }
    }
}
