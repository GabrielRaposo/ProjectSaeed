using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    const float MIN_X_SPEED = .25f, Y_SIZE = 1f;

    public enum State { Stand, Airborne, Stunned }

    [Header("Debug")]
    public State state;
    public Color[] stateColors;

    [Header("Movement Values")]
    public float speedIncreaseRatio;
    public float speedDecreaseRatio;
    public float maxSpeed;
    public float jumpForce;
    public float jumpMaxHold;

    [Header("External Info")]
    public LayerMask groundLayer;

    [Header("Component Reference")]
    public AudioManager audioManager;
    public GameObject directionArrow;
    public ParticleSystem bouncePS;

    bool
        onGround,
        onWall,
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

        _renderer.color = stateColors[0];
    }
	
    void InitStrings()
    {
        horizontalAxis = "Horizontal";
        verticalAxis = "Vertical";
        jumpButton = "Jump";
    }

	void Update () {
        if (Time.timeScale < 1) return;
        switch (state)
        {
            case State.Stand:
                CheckGround();
                HorizontalMovement();
                JumpMovement();
                break;

            case State.Airborne:
                CheckGround();
                HorizontalMovement();
                if (onWall) GetWallJump();
                break;

            case State.Stunned:
                break;
        }
	}

    void CheckGround()
    {
        onGround = Physics2D.OverlapCircle(transform.position + (Vector3.down * .5f), .2f, groundLayer);

        switch (state)
        {
            case State.Stand:
                if(!onGround)
                {
                    state = State.Airborne;
                }
                break;

            case State.Airborne:
                if (onGround && scoreSystem)
                {
                    scoreSystem.ResetValue();
                    audioManager.Play("Land");
                    state = State.Stand;
                }
                break;
        }
    }

    void HorizontalMovement()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        Vector2 _velocity = _rigidbody.velocity;

        if(Mathf.Abs(horizontalInput) > 0)
        {

            _velocity += Vector2.right * speedIncreaseRatio * horizontalInput * (onGround ? 1f : .5f);
            if (horizontalInput > 0 && _velocity.x > maxSpeed)
                _velocity.x = maxSpeed;
            if (horizontalInput < 0 && _velocity.x < -maxSpeed)
                _velocity.x = -maxSpeed;

            if(onGround || !onWall)
                SetLookingRight( horizontalInput > 0 ? true : false );
        }
        else
        { 
            if (_velocity.x > 0) {
                _velocity += Vector2.left  * speedDecreaseRatio * (onGround ? 1f : .3f);
            } else
            if (_velocity.x < 0) {
                _velocity += Vector2.right * speedDecreaseRatio * (onGround ? 1f : .3f);
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
            audioManager.Play("Jump");
            Jump(Vector2.up);
            if (jumpRoutine != null) StopCoroutine(jumpRoutine);
            jumpRoutine = StartCoroutine(JumpHoldProperty());
        }
    }

    void Jump(Vector2 direction)
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(direction * jumpForce);
    }

    IEnumerator JumpHoldProperty() {
        for (int i = 0; i < jumpMaxHold && Input.GetButton(jumpButton); i++)
        {
            _renderer.color = stateColors[3];
            yield return new WaitForEndOfFrame();
            Jump(Vector2.up);
        }
        _renderer.color = stateColors[0];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Bouncy")
        {
            if (state == State.Stunned || bounceDisabled) return;

            BounceProperty bounceProperty = collision.gameObject.GetComponent<BounceProperty>();
            if (bounceProperty)
            {
                bool isAbove = (transform.position.y - Y_SIZE / 2 > collision.transform.position.y) ? true : false;
                if (isAbove) {
                    BounceAway(collision.transform.position);
                    bouncePS.Play();
                    bounceProperty.BigTremble();
                    if (scoreSystem) scoreSystem.AddValue();
                } else {
                    BounceSideways(collision.transform.position);
                    bounceProperty.SmallTremble();
                }

                audioManager.Play("Bounce");
                StartCoroutine(bounceDisableFrames());
            }
        } else 
        if(collision.tag == "Hitbox")
        {
            GetDamaged();
        }
    }

    void BounceAway(Vector2 bouncer)
    {
        int xMax = 3;
        int maxAngle = 30;
        float distance = transform.position.x - bouncer.x;
        float angle = (distance / xMax) * maxAngle;
        Vector2 direction = Vector2FromAngle(90 - angle);
        direction.y = 1;
        Jump(direction);

        //Jump(Vector2.up);
        if (jumpRoutine != null) StopCoroutine(jumpRoutine);
        jumpRoutine = StartCoroutine(JumpHoldProperty());
    }

    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    void BounceSideways(Vector3 collisionPosition) {
        Vector2 _force;

        if (transform.position.x < collisionPosition.x) 
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Bomb bomb = collision.gameObject.GetComponent<Bomb>();
            if (bomb) bomb.Explode();
            GetDamaged();
        } else
        if (collision.transform.tag == "Wall")
        {
            _renderer.color = stateColors[2];
            onWall = true;
            if (!onGround) {
                SetLookingRight((collision.contacts[0].point.x > transform.position.x) ? false : true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.tag == "Wall")
        {
            _renderer.color = stateColors[2];
            if (!onGround) {
                SetLookingRight((collision.contacts[0].point.x > transform.position.x) ? false : true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Wall")
        {
            _renderer.color = stateColors[0];
            onWall = false;
        }
    }

    private void GetWallJump()
    {
        if (!onGround && Input.GetButtonDown(jumpButton))
        {
            Vector2 sideDirection = lookingRight ? Vector2.right : Vector2.left;
            Jump((Vector2.up + sideDirection) * 1.2f);
            audioManager.Play("Jump");
        }
    }

    void GetDamaged()
    {
        if (invincible) return;
        _health.SetDamage(1);
        audioManager.Play("Hurt");
        DisableMovement();
    }

    void DisableMovement()
    {
        StopAllCoroutines();
        bounceDisabled = false;
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
        state = State.Stunned;
        _renderer.color = stateColors[1];
        yield return new WaitForSeconds(.5f);
        _renderer.color = stateColors[0];
        state = State.Stand;
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
