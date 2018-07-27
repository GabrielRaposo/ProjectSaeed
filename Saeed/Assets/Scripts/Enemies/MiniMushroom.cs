using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMushroom : MonoBehaviour {

    public float timeToRelease;

    Collider2D _collider;
    Rigidbody2D _rigidbody2D;
    Animator _animator;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(transform.position.y < -10)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _animator.SetTrigger("Toggle");
            Spawner.instance.Return(gameObject);
        }
    }

    public void PrepareToFall(Vector2 aimedPosition, float descendDelay)
    {
        transform.position = aimedPosition + Vector2.up * 1.5f;
        transform.rotation = Quaternion.Euler(Vector3.forward * 180);
        _collider.enabled = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(DescendToPosition(aimedPosition, descendDelay));
    }

    IEnumerator DescendToPosition(Vector2 position, float descendDelay)
    {
        yield return new WaitForSeconds(descendDelay);

        while (transform.position.y > position.y)
        {
            yield return new WaitForEndOfFrame();
            transform.position += Vector3.down * .02f;
        }

        transform.position = position;
        _animator.SetTrigger("Toggle");

        yield return new WaitForSeconds(timeToRelease);
        Release();
    }

    void Release()
    {
        transform.position += Vector3.down * .2f;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _collider.enabled = true;
    }
}
