using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceProperty: MonoBehaviour {
    [Header("Values")]
    public float squishSpeed = 0.01f;
    public float quantTrembles;
    public float trembleForce;
    public float trembleDecrease;

    [Header("External Values")]
    public BossHealth bossHealth;

    SpriteRenderer _renderer;
    Collider2D _collider;

    Coroutine trembleRoutine;
    Vector2 originalScale;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }

    public void SmallTremble()
    {
        if (trembleRoutine != null) StopCoroutine(trembleRoutine);
        trembleRoutine = StartCoroutine(TrembleAnimation(0.5f));
    }

    public void BigTremble(int damage)
    {
        if (trembleRoutine != null) StopCoroutine(trembleRoutine);
        trembleRoutine = StartCoroutine(TrembleAnimation(1.0f));
        if (bossHealth) bossHealth.SetDamage(damage);
    }

    IEnumerator TrembleAnimation(float manualRatio)
    {
        transform.localScale = originalScale 
            + (Vector2.down  * trembleForce * 2) 
            + (Vector2.right * trembleForce * 2);

        float tForce = trembleForce * manualRatio;

        for (int i = 0; i < quantTrembles; i++)
        {
            yield return HorizontalSquish(tForce);

            transform.localScale = originalScale;
            tForce -= trembleDecrease;
        }
        _collider.enabled = true;
        yield return new WaitForEndOfFrame();
    }

    IEnumerator HorizontalSquish (float force)
    {
        bool press = true;
        while (true)
        {
            if (press) {
                transform.localScale += Vector3.right * squishSpeed;
                transform.localScale += Vector3.down  * squishSpeed;
                if (transform.localScale.x > originalScale.x + force)
                {
                    press = false;
                }
            } else {
                transform.localScale += Vector3.left * squishSpeed;
                transform.localScale += Vector3.up   * squishSpeed;
                if (transform.localScale.x < originalScale.x)
                {
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
