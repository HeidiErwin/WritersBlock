using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    private bool facingRight = true;
    private bool jump = false;
    private float moveForce = 365f;
    private float maxSpeed = 1.5f;
    private float jumpForce = 365f;
    private bool inventoryLeft = true;

    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;
    private Collider2D collider;

    public GameObject inventory;
    public LayerMask groundLayer;

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 0 && inventoryLeft)
        {
            inventoryLeft = false;
            inventory.transform.SetPositionAndRotation(new Vector3(-inventory.transform.position.x, inventory.transform.position.y, transform.position.z), Quaternion.identity);
        }
        if (transform.position.x <= 0 && !inventoryLeft)
        {
            inventoryLeft = true;
            inventory.transform.SetPositionAndRotation(new Vector3(-inventory.transform.position.x,inventory.transform.position.y,transform.position.z), Quaternion.identity);
        }
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Launcher");
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();
        if (!IsGrounded())
        {
            h = 0f;
        }

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed)
            rb2d.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

        if (jump)
        {

            anim.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
