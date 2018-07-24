using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    const float Y_SIZE = 1f;

    public SpriteRenderer visualComponent;
    public ParticleSystem explosionParticles;
    public Collider2D explosionHitbox;

    bool bounceDisabled;
    float originalGravity;
    Vector2 originalScale;
    Coroutine trembleRoutine;

    Rigidbody2D _rigidbody;
    Collider2D _collider;
    Shader shaderBlink; 

	void Awake () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        originalScale = visualComponent.transform.localScale;
        shaderBlink = Shader.Find("GUI/Text Shader");
    }

    void setComponents(bool value)
    {
        visualComponent.enabled = value;
        _collider.enabled = value;
        if (value) {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        } else {
            _rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
	
    public void SetToLaunch()
    {
        setComponents(true);
        originalGravity = _rigidbody.gravityScale;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 0;
        StartCoroutine(WaitToLaunch());
    }

    IEnumerator WaitToLaunch()
    {
        yield return new WaitForSeconds(2);

        float 
            x = Random.Range(4f, 8f),
            y = Random.Range(0f, 3f);
        int signal = (Random.Range(1, 3) * 2) - 3;
        _rigidbody.gravityScale = originalGravity;
        _rigidbody.velocity = Vector2.right * x * signal + Vector2.up * y;
        StartCoroutine(ExplosionTimer());
    }

    IEnumerator ExplosionTimer()
    {
        Shader shaderOriginal = visualComponent.material.shader;
        float 
            waitingTimer = 1.2f,
            timerDecrease = .25f;

        while (waitingTimer > .1f)
        {
            yield return new WaitForSeconds(waitingTimer);
            waitingTimer -= timerDecrease;

            visualComponent.material.shader = shaderBlink;
            for (int i = 0; i < 2; i++) yield return new WaitForEndOfFrame();
            visualComponent.material.shader = shaderOriginal;
        }
        waitingTimer = .1f;

        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(waitingTimer);

            visualComponent.material.shader = shaderBlink;
            for (int j = 0; j < 2; j++) yield return new WaitForEndOfFrame();
            visualComponent.material.shader = shaderOriginal;
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
        setComponents(false);
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
