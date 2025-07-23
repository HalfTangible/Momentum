using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //Attach this and a 2d rigidbody to an object, then set gravity to 0

    public float movSpeed;
    //float speedX, speedY;
    Rigidbody2D rb;
    Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        //speedY = Input.GetAxisRaw("Vertical") * movSpeed;
        movement.x = Input.GetAxisRaw("Horizontal") * movSpeed;
        movement.y = Input.GetAxisRaw("Vertical") * movSpeed;
    }

    void FixedUpdate() //Update can vary with the framerate, so FixedUpdate works better for physics. (consistently called 50x a second)
    {
        //rb.velocity = new Vector2(speedX, speedY);
        rb.MovePosition(rb.position + movement * movSpeed * Time.fixedDeltaTime);
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
