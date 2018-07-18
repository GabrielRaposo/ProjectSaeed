using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    const float MIN_X_SPEED = .25f, Y_SIZE = 1f;

    [Header("Movement Values")]
    public float speedIncreaseRatio;
    public float speedDecreaseRatio;
    public float maxSpeed;
    public float jumpForce;
    public float jumpMaxHold;

    [Header("External Info")]
    public LayerMask groundLayer;
    public string BouncyTag;
    public string EnemyTag;

    [Header("Component Reference")]
    public GameObject directionArrow;
    public ParticleSystem bouncePS;

    bool
        onGround,
        stunned,
        invincible,
        lookingRight,
        bounceDisabled;

    string 
        horizontalAxis, 
        verticalAxis, 
        jumpButton;

    Rigidbody2D _rigidbody;
    SpriteRenderer _renderer;
    PlayerHealth _health;
    ScoreSystem scoreSystem;
    Coroutine jumpRoutine;

	void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _health = GetComponent<PlayerHealth>();
        scoreSystem = ScoreSystem.instance;

        InitStrings();

        SetLookingRight(true);
    }
	
    void InitStrings()
    {
        horizontalAxis = "Horizontal";
        verticalAxis = "Vertical";
        jumpButton = "Jump";
    }

	void Update () {
        if (stunned) return;

        CheckGround();
        HorizontalMovement();
        if (onGround)
        {
            JumpMovement();
            if (scoreSystem) scoreSystem.ResetValue();
        }
	}

    void CheckGround()
    {
        onGround = Physics2D.OverlapCircle(transform.position + (Vector3.down * .5f), .2f, groundLayer);
    }

    void HorizontalMovement()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        Vector2 _velocity = _rigidbody.velocity;

        if(Mathf.Abs(horizontalInput) > 0)
        {
            SetLookingRight( horizontalInput > 0 ? true : false );

            _velocity += Vector2.right * speedIncreaseRatio * horizontalInput;
            if (horizontalInput > 0 && _velocity.x > maxSpeed)
                _velocity.x = maxSpeed;
            if (horizontalInput < 0 && _velocity.x < -maxSpeed)
                _velocity.x = -maxSpeed;
        }
        else
        { 
            if (_velocity.x > 0) {
                _velocity += Vector2.left  * (onGround ? speedDecreaseRatio : (speedDecreaseRatio / 2));
            } else
            if (_velocity.x < 0) {
                _velocity += Vector2.right * (onGround ? speedDecreaseRatio : (speedDecreaseRatio / 2));
            }

            if (Mathf.Abs(_velocity.x) < MIN_X_SPEED) _velocity.x = 0;
        }

        _rigidbody.velocity = _velocity;
    }

    void SetLookingRight(bool value)
    {
        if (value)
            directionArrow.transform.rotation = Quaternion.Euler(Vector3.forward * 270);
        else
            directionArrow.transform.rotation = Quaternion.Euler(Vector3.forward * 90);

        lookingRight = value;
    }

    void JumpMovement()
    {
        if (Input.GetButtonDown(jumpButton))
        {
            Jump();
            StartCoroutine(JumpHoldProperty());
        }
    }

    void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(Vector2.up * jumpForce);
    }

    IEnumerator JumpHoldProperty() {
        for (int i = 0; i < jumpMaxHold && Input.GetButton(jumpButton); i++)
        {
            yield return new WaitForEndOfFrame();
            Jump();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == BouncyTag)
        {
            if (stunned || bounceDisabled) return;

            BounceProperty bounceProperty = collision.gameObject.GetComponent<BounceProperty>();
            if (bounceProperty)
            {
                bool isAbove = (transform.position.y - Y_SIZE / 2 > collision.transform.position.y) ? true : false;
                if (isAbove) {
                    BounceUp();
                    bouncePS.Play();
                    bounceProperty.BigTremble();
                    if (scoreSystem) scoreSystem.AddValue();
                } else {
                    BounceSideways(collision.transform.position);
                    bounceProperty.SmallTremble();
                }
                StartCoroutine(bounceDisableFrames());
            }
        } else 
        if (collision.tag == EnemyTag)
        {
            if (invincible) return;
            _health.TakeDamage();
            DisableMovement();
        }
    }

    void BounceUp()
    {
        Jump();
        StartCoroutine(JumpHoldProperty());
    }

    void BounceSideways(Vector3 collisionPosition) {
        Vector2 _force;

        if(transform.position.x < collisionPosition.x) 
            _force = Vector2.left;
        else 
            _force = Vector2.right;

        _rigidbody.velocity = _force * 15;
    }

    IEnumerator bounceDisableFrames()
    {
        bounceDisabled = true;
        for (int i = 10; i > 0; i--) yield return new WaitForEndOfFrame();
        bounceDisabled = false;
    }

    void DisableMovement()
    {
        StopAllCoroutines();
        Knockback();
        StartCoroutine(blinkLoop());
    }

    void Knockback()
    {
        Vector2 pushDirection = Vector2.up * 8 + (lookingRight ? Vector2.left : Vector2.right) * 8;
        _rigidbody.velocity = pushDirection;
        StartCoroutine(stun());
    }

    IEnumerator stun()
    {
        stunned = true;
        yield return new WaitForSeconds(1);
        stunned = false;
    }

    IEnumerator blinkLoop()
    {
        int 
            numOfBlinks = 10,
            hideFrames = 3,
            showFrames = 8;

        invincible = true;

        for(int i = 0;  i < numOfBlinks; i++)
        {
            _renderer.enabled = false;
            for (int j = 0; j < hideFrames; j++)
                yield return null;

            _renderer.enabled = true;
            for (int j = 0; j < showFrames; j++)
                yield return null;
        }

        invincible = false;
    }
}
