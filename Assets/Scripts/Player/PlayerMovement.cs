using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    public Rigidbody2D rb;

    [Header("Movement Statistics")] // headers separate sections in the inspector
    public float acceleration = 1f;

    [Tooltip("Multiply velocity each frame to simulate friction (0..1). 1 = no friction")] // wow look a tooltip idk ill probably make this useful
    [Range(0f, 1f)]
    public float frictionCoefficient = 0.9f;

    [Tooltip("Sudden dash forward. Larger numbers go farther.")]
    public float dashStrength = 10f;
    public float dashStamina = 10f;
    public float staminaRegenCooldown = 1f;  // time after dashing before stamina starts regenerating

    [Header("Player Statistics")]
    public float maxHealth = 100f;
    public float maxStamina = 30f;
    public float staminaRegenRate = 10f;      // stamina regenerated per second

    [Header("Other Objects")]
    [SerializeField] StatisticPercentage StatisticPercentage;

    [Header("uhmm pretty please dont mess with this")]
    private float currentHealth;        // current health
    private float currentStamina;       // current stamina

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    [SerializeField] private ParticleSystem dashParticles; // dash particle effect
    private ParticleSystem dashParticlesInstance;          // dash particle instance
    private ParticleSystem.EmissionModule dashParticlesEmission; // dash particle emission module

    private Vector2 inputAcceleration;  // acceleration vector
    private Vector2 inputVelocity;      // velocity vector
    private Vector2 additionalVelocity; // velocity vector added by other means (like dashing)

    private Vector2 moveInput;          // input vector
    private bool dashRequested;         // whether dash was requested
    private float dashRequestTime;      // time when dash was requested
    private bool isDashing;             // whether player is currently dashing

    private float startRegenTime;       // time when stamina regen starts

    // --------------------------------------------------------------------------------- //
    void Start()
    {
        // initialize player stats
        currentHealth = maxHealth;
        currentStamina = maxStamina;
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
            dashRequestTime = Time.time + 0.1f;
        }
    }
    
    // movement handlers
    private void Dash()
    {
        // add a sudden burst of velocity in the direction of movement input
        Vector2 dashDirection = moveInput.normalized;
        additionalVelocity = dashDirection * dashStrength;

        // reduce stamina on dash
        currentStamina -= dashStamina;

        // set time to start regenerating stamina
        startRegenTime = Time.time + staminaRegenCooldown;

        // reset dash request
        dashRequested = false;

        // play dash particles
        dashParticlesInstance = Instantiate(dashParticles, transform.position, Quaternion.identity);
        Destroy(dashParticlesInstance.gameObject, dashParticlesInstance.main.duration);

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
    private void RegenerateStamina()
    {
        // if enough time has passed since last dash, regenerate stamina
        if (Time.time >= startRegenTime)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina); // clamp to max stamina
        }
    }

    // Update is called once per frame
    void Update()
    {
        // whenever dash is requested, perform dash
        if (dashRequested && dashRequestTime > Time.time && currentStamina >= dashStamina && moveInput != Vector2.zero) { Dash(); }
        if (isDashing) {
            // dash afterimage
            PlayerAfterimagePool.Instance.GetFromPool();

            // stop dashing effect when additional velocity is low enough
            if (additionalVelocity.magnitude < 1f)
            {
                isDashing = false;
            }
        }

        // regenerate stamina each frame
        RegenerateStamina();

        // move the player each frame
        MovePlayer();

        // communicate with other systems (like UI) here if needed
        StatisticPercentage.healthHandler.UpdateDisplay(currentHealth, maxHealth);
        StatisticPercentage.staminaHandler.UpdateDisplay(currentStamina, maxStamina);
    }
}