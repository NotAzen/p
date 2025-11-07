using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("References")]
    public Rigidbody2D rb;

    [Header("Movement Statistics")] // headers separate sections in the inspector
    public float acceleration = 1f;
    [Tooltip("Multiply velocity each frame to simulate friction (0..1). 1 = no friction")] // wow look a tooltip idk ill probably make this useful
    [Range(0f, 1f)]
    public float frictionCoefficient = 0.9f;

    [Header("Dash Statistics")]
    [Tooltip("Sudden dash forward. Larger numbers go farther.")]
    public float dashStrength = 10f;
    public float dashStamina = 10f;

    [Header("Player Statistics")]
    public float maxHealth = 100f;
    private float currentHealth;              // current health
    public float healthRegenRate = 10f;       // health regenerated per second
    public float healthRegenCooldown = 2f;    // time after taking damage before health starts regenerating
    private float startHealthRegenTime;       // time when health regen starts
    public float maxStamina = 30f;
    private float currentStamina;             // current stamina
    public float staminaRegenRate = 10f;      // stamina regenerated per second
    public float staminaRegenCooldown = 1f;   // time after dashing before stamina starts regenerating
    private float startStaminaRegenTime;      // time when stamina regen starts

    [Header("Invincibility Frames")]
    public float iframes = 0.5f;              // invincibility frames after taking damage
    private float iframeStartTime;            // time when invincibility frames statr

    [Header("Other Objects")]
    [SerializeField] StatisticPercentage StatisticPercentage;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem dashParticles; // dash particle effect
    private ParticleSystem dashParticlesInstance;          // dash particle instance

    [Header("Movement Variables")]
    private Vector2 moveInput;          // input vector
    private Vector2 inputAcceleration;  // acceleration vector
    private Vector2 inputVelocity;      // velocity vector
    private Vector2 additionalVelocity; // velocity vector added by other means (like dashing)

    [Header("Dash Variables")]
    private bool dashRequested;         // whether dash was requested
    private bool isDashing;             // whether player is currently dashing
    private float dashRequestTime;      // time when dash was requested

    [Header("Afterimage Variables")]
    private float lastAfterimageTime;    // last time an afterimage was created
    [SerializeField] public float afterimageTime = 0.05f; // time between afterimages

    // --------------------------------------------------------------------------------- //
    void Start()
    {
        // initialize player stats
        currentHealth = maxHealth;
        currentStamina = maxStamina;
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
        ConsumeStamina(dashStamina);

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
        if (Time.time < iframeStartTime + iframes)
        {
            return false; // still in invincibility frames
        }

        currentHealth -= damage; // reduce health
        startHealthRegenTime = Time.time + healthRegenCooldown; // set time to start regenerating health
        iframeStartTime = Time.time; // start invincibility frames

        return true; // damage taken successfully
    }

    private void ConsumeStamina(float usedStamina)
    {
        currentStamina -= usedStamina; // reduce stamina
        startStaminaRegenTime = Time.time + staminaRegenCooldown; // set time to start regenerating stamina
    }

    private void RegenerateStamina()
    {
        // if enough time has passed since last dash, regenerate stamina
        if (Time.time >= startStaminaRegenTime)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina); // clamp to max stamina
        }
    }
    private void RegenerateHealth()
    {
        // if enough time has passed since last dash, regenerate stamina
        if (Time.time >= startHealthRegenTime)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth); // clamp to max stamina
        }
    }

    // Update is called once per frame
    void Update()
    {
        // whenever dash is requested, perform dash
        if (dashRequested && dashRequestTime > Time.time && currentStamina >= dashStamina && moveInput != Vector2.zero) { Dash(); }
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

        // regenerate health and stamina each frame
        RegenerateHealth();
        RegenerateStamina();

        // move the player each frame
        MovePlayer();

        // communicate with other systems (like UI) here if needed
        StatisticPercentage.healthHandler.UpdateDisplay(currentHealth, maxHealth);
        StatisticPercentage.staminaHandler.UpdateDisplay(currentStamina, maxStamina);
    }
}