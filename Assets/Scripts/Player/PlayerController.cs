using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    [Header("Movement Statistics")] // headers separate sections in the inspector
    public float acceleration = 1f;
    [Tooltip("Multiply velocity each frame to simulate friction (0..1). 1 = no friction")] // wow look a tooltip idk ill probably make this useful
    [Range(0f, 1f)]
    public float frictionCoefficient = 0.9f;

    [Header("Dash Statistics")]
    [Tooltip("Sudden dash forward. Larger numbers go farther.")]
    public float dashStrength = 10f;
    public float dashStamina = 10f;

    //[Header("Invincibility Frames")]
    //public float iframes = 0.5f;              // invincibility frames after taking damage
    //private float iframeStartTime;            // time when invincibility frames started

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem dashParticles; // dash particle effect
    private ParticleSystem dashParticlesInstance;          // dash particle instance

    [Header("Movement Variables")]
    private Vector2 moveInput;          // input vector
    private Vector2 inputAcceleration;  // acceleration vector
    private Vector2 inputVelocity;      // velocity vector
    private Vector2 additionalVelocity; // velocity vector added by other means (like dashing)

    [Header("Dash Variables")]
    private Cooldown dashCooldown = new(0.1f); // dash cooldown
    private bool dashRequested;         // whether dash was requested
    private bool isDashing;             // whether player is currently dashing
    private float dashRequestTime;      // time when dash was requested

    [Header("Afterimage Variables")]
    private float lastAfterimageTime;    // last time an afterimage was created
    [SerializeField] public float afterimageTime = 0.05f; // time between afterimages

    [Header("Player Statistics")]
    public PlayerStatistics playerStats;

    // --------------------------------------------------------------------------------- //
    // 

    void Start()
    {
        // grab rigidbody reference if not set
        rb = GetComponent<Rigidbody2D>();

        // initialize statistics
        playerStats = GetComponent<PlayerStatistics>();
    }

    // collision handler
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if anything needs to be handled on collision, do it here
    }

    // movement input handler
    public void OnMove(InputValue value)
    {
        // read input vector and scale it to the acceleration vector
        moveInput = value.Get<Vector2>();
    }

    // dash input handler
    public void OnDash(InputValue value)
    {
        // ummm idk apparently >0.5f for buttons detects presses so like yeah
        if (value.Get<float>() > 0.5f)
        {
            dashRequested = true;
            dashCooldown.Trigger();
        }
    }
    
    // movement handlers
    private void Dash()
    {
        // add a sudden burst of velocity in the direction of movement input
        Vector2 dashDirection = moveInput.normalized;
        additionalVelocity = dashDirection * dashStrength;

        // reduce stamina on dash
        playerStats.stamina.Consume(dashStamina);

        // reset dash request
        dashRequested = false;

        // play dash particles
        dashParticlesInstance = Instantiate(dashParticles, transform.position, Quaternion.identity);

        // temporarily mark player as dashing (for afterimage effect)
        isDashing = true;

        // create afterimage effect i hope
        PlayerAfterimagePool.Instance.GetFromPool();

    }

    private void MovePlayer()
    {
        // calculate player acceleration based on input
        inputAcceleration = moveInput * acceleration;

        // update player velocity based on acceleration
        inputVelocity += inputAcceleration * Time.deltaTime;

        // apply some friction to slow down over time
        inputVelocity *= Mathf.Pow(frictionCoefficient, Time.deltaTime);
        additionalVelocity *= Mathf.Pow(frictionCoefficient, Time.deltaTime);

        // add additional velocity (like from dashing) to player velocity
        rb.linearVelocity = inputVelocity + additionalVelocity;
    }

    // statistics handlers
    public bool TakeDamage(float damage)
    {
        if (playerStats.iframes.IsReady())
        {
            return false; // still in invincibility frames
        }

        playerStats.health.Consume(damage); // reduce health
        playerStats.iframes.Trigger(); // start invincibility frames

        return true; // damage taken successfully
    }

    // Update is called once per frame
    void Update()
    {
        // whenever dash is requested, perform dash
        if (dashRequested && dashCooldown.IsReady() && playerStats.stamina.Has(dashStamina) && moveInput != Vector2.zero)
        {
            Dash();
        }

        if (isDashing) {
            // dash afterimage
            if (Time.time > lastAfterimageTime + afterimageTime) {
                PlayerAfterimagePool.Instance.GetFromPool();
                lastAfterimageTime = Time.time;
            }

            // stop dashing effect when additional velocity is low enough
            if (additionalVelocity.magnitude < 1f)
            {
                isDashing = false;
            }
        }

        // move the player each frame
        MovePlayer();
    }
}