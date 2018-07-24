using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour {

    [Header("Values")]
    public int[] stageValues;
    public Gradient gradient;

    [Header("Visual Info")]
    public RectTransform healthBar;
    public Image fillBar;
    public SpriteRenderer[] _renderers;

    public int value { get; private set; }
    int currentMaxValue;
    int stageIndex;
    bool invincible;
    Shader originalShader;
    Shader shaderGUItext;

    void Awake () {
        originalShader = _renderers[0].material.shader;
        shaderGUItext = Shader.Find("GUI/Text Shader");
	}

    public void SetStageIndex(int index)
    {
        stageIndex = index;
        SetMaxValue(stageValues[stageIndex]);
    }
	
    void SetMaxValue(int maxValue)
    {
        currentMaxValue = maxValue;
        value = currentMaxValue;
        UpdateBar();
    }

    private void UpdateBar()
    {
        float percent = (float)value / currentMaxValue;
        fillBar.fillAmount = percent;
        fillBar.color = gradient.Evaluate(percent);
    }

    public void SetDamage(int damage)
    {
        StartCoroutine(WhiteFlash());
        
        if (invincible) return;
        value -= damage;
        UpdateBar();
        if (value < 1)
        {
            if(stageIndex < stageValues.Length - 1)
            {
                stageIndex++;
            }
            StartCoroutine(Refill());
        }
    }

    IEnumerator WhiteFlash()
    {
        foreach (SpriteRenderer r in _renderers) r.material.shader = shaderGUItext;
        for (int i = 0; i < 3; i++) yield return new WaitForEndOfFrame();
        foreach (SpriteRenderer r in _renderers) r.material.shader = originalShader;
    }

    IEnumerator Refill() {
        invincible = true;
        yield return ShakeBar();
        yield return new WaitForSeconds(0.5f);

        float
            fillAmount = 0,
            increaseValue = .05f;

        fillBar.fillAmount = fillAmount;
        while(fillAmount < 1)
        {
            yield return new WaitForEndOfFrame();
            fillAmount += increaseValue;
            fillBar.fillAmount = fillAmount;
            fillBar.color = gradient.Evaluate(fillAmount);
        }
        fillBar.fillAmount = 1;

        SetMaxValue(stageValues[stageIndex]);
        GetComponent<BossMovement>().EndStage();
        invincible = false;
    }

    IEnumerator ShakeBar()
    {
        bool toggle = false;
        float 
            shakeForce = 4f,
            delta = .2f,
            originalX = healthBar.anchoredPosition.x;

        while (shakeForce > 0)
        {
            for(int i = 0; i < 3; i++) yield return new WaitForEndOfFrame();
            healthBar.anchoredPosition += shakeForce * ((toggle = !toggle) ? Vector2.left : Vector2.right);
            shakeForce -= delta;
        }
        healthBar.anchoredPosition = new Vector2(originalX, healthBar.anchoredPosition.y);
    }
}
