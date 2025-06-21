using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourScript : MonoBehaviour
{
    [SerializeField] private float enemySpeed = 5f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particles;
    private bool isAlive = true;

    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private AudioManagerScript audio;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private CameraShakeScript cameraShake;


    private void Start()
    {
        audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManagerScript>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShakeScript>();
    }
    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }
        rb.velocity = new Vector2(transform.localScale.x * enemySpeed, rb.velocity.y);

        Collider2D contactCollider = Physics2D.OverlapPoint(new Vector2(groundCheck.position.x, groundCheck.position.y));
        if (contactCollider == null)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided(enemyScript)");
        if (collision.collider.tag == "Player" && !playerScript.isStomping && playerScript.isOnCooldown == false)
        {
            playerScript.PlayerHit();
        }
        if (collision.collider.tag == "Player" && playerScript.isStomping)
        {
            playerScript.isStomping = false;

            audio.stompfx.Play();
            ParticleSystem particlesObject = Instantiate(particles, transform.position, transform.rotation);
            StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
            playerScript.isStomping = false;
            Destroy(boxCollider);
            Destroy(spriteRenderer);
            Destroy(particlesObject.gameObject,particles.startLifetime);
            Destroy(gameObject,0.15f);
        }
    }
}
