using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private int maxJumpCount = 3;
    private int currentJumpCount = 0;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float thrust = 16f;

    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashThrust = 10f;

    public bool isStomping = false;
    [SerializeField] private float stompForce = 15f;
    private bool isGrounded;

    public CameraShakeScript cameraShake;

    [SerializeField] private float maxHealth = 3;
    [SerializeField] private float currentHealth;

    [SerializeField] private float hitCooldown;
    [SerializeField] public bool isOnCooldown = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem stompParticles;
    [SerializeField] private ParticleSystem doorParticles;
    [SerializeField] private TrailRenderer stompTrail;
    [SerializeField] private AudioManagerScript _audio;
    [SerializeField] private GameManagerScript gameManager;

    [SerializeField] private Image[] images;

    private bool gameHasEnded = false;

    [System.Obsolete]

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
    }
    private void Update()
    { 
        horizontal = Input.GetAxisRaw("Horizontal"); //GetAxisRaw returns 1,0,-1

        if (Input.GetButtonDown("Jump") && !isDashing && currentJumpCount<maxJumpCount && !isStomping && !gameHasEnded)
        {
            _audio.jumpfx.Play();
            rb.velocity = new Vector2(rb.velocity.x,thrust);
            currentJumpCount++;
            isGrounded = false;
            ParticleSystem particlesInstance =  Instantiate(jumpParticles, transform.position, Quaternion.Euler(transform.rotation.x,transform.rotation.y,-155f));
            particlesInstance.Play();
            Destroy(particlesInstance.gameObject, particlesInstance.startLifetime);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y*0.5f); //This allows player to jump if button is held longer or shorter
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !gameHasEnded)
        {
            _audio.dashfx.Play();
            isStomping = false;
            StartCoroutine(Dash());
            ParticleSystem particlesInstance = Instantiate(dashParticles, transform.position, transform.rotation);
            particlesInstance.Play();
            Destroy(particlesInstance.gameObject,particlesInstance.startLifetime);
        }
        if (Input.GetMouseButtonDown(0) && !isStomping && !isGrounded && !gameHasEnded)
        {
            _audio.dashfx.Play();
            ParticleSystem particlesInstance = Instantiate(jumpParticles, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 25));
            particlesInstance.Play();
            Destroy(particlesInstance.gameObject, particlesInstance.startLifetime);
            isStomping = true;
            stompTrail.emitting = true;
            rb.velocity = new Vector2(0,-stompForce);
        }
        Flip();
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isStomping && !gameHasEnded)
        {
            rb.velocity = new Vector2(horizontal*speed,rb.velocity.y);
        }
    }

    [System.Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && collision.gameObject.transform.position.y<transform.position.y)
        {
            if (isStomping)
            {
                StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
                _audio.stompfx.Play();
                ParticleSystem particlesInstance = Instantiate(stompParticles, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 25));
                particlesInstance.Play();
                Destroy(particlesInstance.gameObject, particlesInstance.startLifetime);
                isStomping = false;
                stompTrail.emitting = false;
            }

            isGrounded = true;
            currentJumpCount = 0;
        }
        if (collision.collider.tag == "Spike" && isOnCooldown == false)
        {
            StartCoroutine(HitCooldownTimer());
            _audio.damagefx.Play();
            StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
            currentHealth --;
            images[(int)currentHealth].enabled = false;
            TerminationCheck();
        }
    }


    public void PlayerHit()
    {
        StartCoroutine(HitCooldownTimer());
        Debug.Log("Player script1" + isStomping);
        _audio.damagefx.Play();
        StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
        currentHealth--;
        images[(int)currentHealth].enabled = false;
        TerminationCheck();
    }

    private void TerminationCheck() {
        if (currentHealth <= 0)
        {
            gameHasEnded = true;
            gameManager.Lose();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Key")
        {
            _audio.dingfx.Play();
            gameManager.doorClose.SetActive(false);
            gameManager.doorOpen.SetActive(true);

            collision.gameObject.SetActive(false);
        }

        if (collision.tag=="DoorOpen")
        {
            gameHasEnded = true;
            doorParticles.Play();
            gameManager.Win();
            _audio.winfx.Play();
            rb.velocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f|| !isFacingRight && horizontal > 0f && !gameHasEnded)
        {
            isFacingRight = !isFacingRight; // Setting isFacingRight to its opposite boolean value or vice-versa
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; // This is flipping the player character
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale; //Storing original gravity scale
        rb.gravityScale = 0f; // Changing gravity scale to 0 so it dosen't move vertically while dashing

        rb.velocity = new Vector2(transform.localScale.x*dashThrust,0f);
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false; 
        dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private IEnumerator HitCooldownTimer() 
    {
        isOnCooldown = true;
        float elapsed = 0f;
        while (elapsed < hitCooldown)
        {
            elapsed += Time.deltaTime;

            yield return null;        //Waiting until next frame is played
        }
        isOnCooldown = false;
    }
}