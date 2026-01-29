using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //Attach this and a 2d rigidbody to an object, then set gravity to 0

    [SerializeField] private float moveSpeed = 5f;
    //float speedX, speedY;
    Rigidbody2D rb;
    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Critical for smooth movement with Cinemachine
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Optional but helps
    }

    // Update is called once per frame
    void Update()
    {
        // Read input once per frame (raw = -1/0/1)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize so diagonal isn't faster
        if (movement.sqrMagnitude > 1f)
            movement.Normalize();
    }

    void FixedUpdate()
    {
        // Apply movement using velocity (smoothest for Cinemachine)
        rb.velocity = movement * moveSpeed;

        // Alternative (if you prefer position-based):
        // rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Store enemy reference or ID if needed (optional for now)
            // For example: GameManager.Instance.SetCurrentEnemy(other.gameObject);
            UnityEngine.Debug.Log($"Hitting {other.gameObject.name}");
            Destroy(other.gameObject);
            UnityEngine.Debug.Log($"Destroyed {other.gameObject.name}");
            SceneManager.LoadScene("BattleScene");
        }
    }
}
