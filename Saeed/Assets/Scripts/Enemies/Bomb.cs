using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    const float Y_SIZE = 1f;

    public SpriteRenderer visualComponent;
    public ParticleSystem spawnEffect;
    public ParticleSystem explosionParticles;
    public Collider2D explosionHitbox;

    bool bounceDisabled;
    Vector2 originalScale;
    Coroutine trembleRoutine;

    Rigidbody2D _rigidbody;
    Collider2D _collider;
    Shader shaderSprite, shaderBlink; 

	void Awake () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        originalScale = visualComponent.transform.localScale;

        shaderSprite = Shader.Find("Sprites/Default");
        shaderBlink = Shader.Find("GUI/Text Shader");
    }

    void SetComponents(bool value)
    {
        visualComponent.enabled = value;
        _collider.enabled = value;
        if (value) {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        } else {
            _rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
	
    public void Spawn()
    {
        SetComponents(false);
        StartCoroutine(SpawnAnimation());
    }

    IEnumerator SpawnAnimation()
    {
        originalScale = visualComponent.transform.localScale;
        visualComponent.transform.localScale = Vector2.zero;

        visualComponent.enabled = true;
        spawnEffect.Play();
        while(visualComponent.transform.localScale.x < originalScale.x)
        {
            yield return new WaitForEndOfFrame();
            visualComponent.transform.localScale += Vector3.one * .005f;
        }
        spawnEffect.Stop();

        StartCoroutine(WaitToLaunch());
    }


    IEnumerator WaitToLaunch()
    {
        yield return new WaitForSeconds(.5f);
        SetComponents(true);

        float 
            x = Random.Range(5f, 8f),
            y = Random.Range(4f, 6f);
        int signal = (Random.Range(1, 3) * 2) - 3;
        _rigidbody.velocity = Vector2.right * x * signal + Vector2.up * y;
        StartCoroutine(ExplosionTimer());
    }

    IEnumerator ExplosionTimer()
    {
        float 
            waitingTimer = 1.5f,
            timerDecrease = .3f;

        while (waitingTimer > .1f)
        {
            yield return new WaitForSeconds(waitingTimer);
            waitingTimer -= timerDecrease;

            visualComponent.material.shader = shaderBlink;
            for (int i = 0; i < 2; i++) yield return new WaitForEndOfFrame();
            visualComponent.material.shader = shaderSprite;
        }
        waitingTimer = .1f;

        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(waitingTimer);

            visualComponent.material.shader = shaderBlink;
            for (int j = 0; j < 2; j++) yield return new WaitForEndOfFrame();
            visualComponent.material.shader = shaderSprite;
        }

        Explode();
    }

    public void Explode()
    {
        StopAllCoroutines();
        visualComponent.transform.localScale = originalScale;
        explosionParticles.Play();
        explosionHitbox.enabled = true;
        StartCoroutine(waitToReturn());
    }

    IEnumerator waitToReturn()
    {
        SetComponents(false);
        yield return new WaitForSeconds(.3f);
        explosionHitbox.enabled = false;
        transform.parent.GetComponent<Spawner>().Return(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (trembleRoutine != null) StopCoroutine(trembleRoutine);
        trembleRoutine = StartCoroutine(Tremble());
        //_rigidbody.AddForce(Vector2.up * 200);
    }

    IEnumerator Tremble()
    {
        float
            squishForce = .03f,
            delta = .007f,
            squishSpeed = .02f;

        visualComponent.transform.localScale = originalScale;
        while (squishForce > 0)
        {
            int state = 0;
            while (state < 3)
            {
                yield return new WaitForEndOfFrame();
                switch (state)
                {
                    case 0:
                        visualComponent.transform.localScale += Vector3.right * squishSpeed;
                        visualComponent.transform.localScale += Vector3.down * squishSpeed;
                        if (visualComponent.transform.localScale.x > originalScale.x + squishForce) state++;
                    break;

                    case 1:
                        visualComponent.transform.localScale += Vector3.left * squishSpeed;
                        visualComponent.transform.localScale += Vector3.up * squishSpeed;
                        if (visualComponent.transform.localScale.x < originalScale.x - squishForce) state++;
                    break;

                    case 2:
                    default:
                        visualComponent.transform.localScale += Vector3.left * squishSpeed;
                        visualComponent.transform.localScale += Vector3.up * squishSpeed;
                        if (visualComponent.transform.localScale.x < originalScale.x) state++;
                    break;
                }
            }

            squishForce -= delta;
        }
        visualComponent.transform.localScale = originalScale;
    }
}
