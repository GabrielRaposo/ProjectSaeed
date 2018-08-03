using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour {

    public TextMeshProUGUI 
        titleDisplay, 
        timeDisplay, 
        deathDisplay,
        healthDisplay,
        continueDisplay;

    [HideInInspector] public bool allowRestart;

    private void Start()
    {
        ToggleEnable(false);
    }

    private void ToggleEnable(bool value)
    {
        titleDisplay.enabled = value;
        timeDisplay.enabled = value;
        deathDisplay.enabled = value;
        healthDisplay.enabled = value;
        continueDisplay.enabled = value;
    }

    public void SetValues(string clearTime, int deathCount, int healthCount)
    {
        timeDisplay.text = "Clear time: " + clearTime;
        deathDisplay.text = "Deaths: " + deathCount;
        healthDisplay.text = "Remaining health: " + healthCount;

        StartCoroutine(DisplayAnimation());
    }

    public IEnumerator DisplayAnimation()
    {
        yield return new WaitForSeconds(.5f);
        yield return FadeInText(titleDisplay);

        StartCoroutine(FadeInText(timeDisplay));
        StartCoroutine(FadeInText(deathDisplay));
        StartCoroutine(FadeInText(healthDisplay));

        yield return new WaitForSeconds(2.5f);
        continueDisplay.enabled = true;
        allowRestart = true;
    }

    IEnumerator FadeInText(TextMeshProUGUI display)
    {
        Color color = display.color;
        color.a = 0;
        display.color = color;
        display.enabled = true;

        while(display.color.a < 1)
        {
            yield return new WaitForFixedUpdate();
            color.a += .01f;
            display.color = color;
        }
    }
}
