using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthDisplay : MonoBehaviour {

    public Image heart;
    public Image delayedHeart;

    RectTransform _rect;

	void Start ()
    {
        _rect = GetComponent<RectTransform>();
        InstantRefill();
    }
    
    public void InstantRefill()
    {
        heart.fillAmount = 1;
        delayedHeart.fillAmount = 1;
    }

    public void ChangeFill(float amount)
    {
        StopAllCoroutines();
        delayedHeart.fillAmount = heart.fillAmount;

        StartCoroutine(HorizontalTremble());
        StartCoroutine(DelayedDecrease(amount));
    }

    IEnumerator HorizontalTremble()
    {
        _rect = GetComponent<RectTransform>();
        float
            force = 10f,
            originalX = _rect.anchoredPosition.x;

        _rect.anchoredPosition += Vector2.left * force / 2;
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            _rect.anchoredPosition += Vector2.right * force * (i % 2 == 0 ? 1 : -1);
            force -= .05f;
        }
        _rect.anchoredPosition = new Vector2(originalX, _rect.anchoredPosition.y);
    }

    IEnumerator DelayedDecrease(float amount)
    {
        while (heart.fillAmount > amount)
        {
            yield return new WaitForFixedUpdate();
            heart.fillAmount -= .1f;
        }
        heart.fillAmount = amount;

        yield return new WaitForSeconds(.5f);
        while(delayedHeart.fillAmount > amount)
        {
            yield return new WaitForFixedUpdate();
            delayedHeart.fillAmount -= .01f;
        }
        delayedHeart.fillAmount = amount;
    }
}
