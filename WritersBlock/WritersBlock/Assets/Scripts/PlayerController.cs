using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	private bool isGrounded = false;
    private bool facingRight = true;
    private bool jump = false;
    private float moveForce = 100f;
    private float maxSpeed = 1.9f;
    private float jumpForce = 455f;
    private bool inventoryLeft = true;

    // Audio
    private AudioSource source;
    public AudioClip deathSound;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip backgroundSound;

    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;
    private Collider2D collider;

    public GameObject inventory;
    public LayerMask groundLayer;
    public LayerMask wordLayer;
    private float walkSoundStartTime = 0f;
    [SerializeField] private int page = 1; // 1 or 2 to indicate right or left page, used to check if word can be dragged

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 2.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        hit = Physics2D.Raycast(position, direction, distance, wordLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    // Use this for initialization
    void Awake()
    {
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        source.loop = true;
        source.clip = backgroundSound;
        source.Play(0);
    }

    // Update is called once per frame
    void Update()
	{
        float h = Input.GetAxis("Horizontal");
        anim.SetBool("walking", h != 0);

        if (transform.position.x > 0)
        {
            page = 2;
            if (inventoryLeft) {
                inventoryLeft = false;
                inventory.transform.SetPositionAndRotation(new Vector3(-inventory.transform.position.x, inventory.transform.position.y, transform.position.z), Quaternion.identity);
            }
        }
        if (transform.position.x <= 0)
        {
            page = 1;
            if (!inventoryLeft) {
                inventoryLeft = true;
                inventory.transform.SetPositionAndRotation(new Vector3(-inventory.transform.position.x, inventory.transform.position.y, transform.position.z), Quaternion.identity);
            }
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


        // anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed)
            rb2d.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

        if (IsGrounded() && rb2d.velocity.x != 0 && (Time.time - walkSoundStartTime) >= (walkSound.length / 3))
        {
            walkSoundStartTime = Time.time;
            source.PlayOneShot(walkSound, 0.5f);
        }

        if (jump)
        {
            source.PlayOneShot(jumpSound, 0.5f);
            // anim.SetTrigger("Jump");
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

    public void SetPage(int pg) {
        page = pg;
    }

    public int GetPage() {
        return page;
    }

}
