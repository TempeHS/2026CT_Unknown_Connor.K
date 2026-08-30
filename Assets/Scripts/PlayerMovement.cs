using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 12f;
    private bool jumpQueued = false;
    private bool isFacingRight = true;
    private float airTime = 0.0f;
    private float jumpAnimTime = 0.0f;
    private float dashDir = 1;
    private Vector2 groundCheckSize = new Vector2(0.45f, 0.1f);
    private Vector2 queueCheckSize = new Vector2(0.45f, 2f); 
    public Vector2 respawnPos = new Vector2(0.0f, 0.0f);   
    public float dashCD = 0.0f;
    private float dashTime = 0.0f;
    public float playerMaxHealth = 8.0f;
    public float playerHealth = 8.0f;
    public int iFrames = 0;
    public float playerKBTime = 0.0f;
    public float playerAttackCD = 0.0f;
    private float playerAttackTime=0.0f;
    private float playerDeathAnimTime = 0.0f;
    public bool grounded = true;
    public float currentSpeed;
    public GameObject playerAttackBox;
    public GameObject playerDeathScreen;
    private bool hasStopped = false;
    
    public bool isDead = false;

    public static bool canInput = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform bufferCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;


    // Update is called once per frame
    private void Awake()
    {
        playerDeathScreen.SetActive(false);

        playerMaxHealth = 8.0f;
        playerHealth = 8.0f;
    }
    void Update()
    {
        if(canInput){
            if(hasStopped){
                hasStopped = false;
            }
        }
        playerDeathAnimTime -= Time.deltaTime;
        if(isDead && playerDeathAnimTime <0.0f){
            playerDeathScreen.SetActive(true);
            Time.timeScale=0;
        }
        if(isDead) return;
        if(!canInput){
            animator.SetTrigger("toIdle");
            if(!hasStopped){
                rb.linearVelocity = Vector2.zero;
                animator.SetTrigger("toIdle");
                hasStopped = true;
            }
            return;
        }
        
        currentSpeed = rb.linearVelocity.x;
        dashTime -= Time.deltaTime;
        dashCD -= Time.deltaTime;
        airTime += Time.deltaTime;
        jumpAnimTime -= Time.deltaTime;
        animator.SetFloat("jumpAnimTime", jumpAnimTime);
        playerKBTime -= Time.deltaTime;
        playerAttackCD -= Time.deltaTime;
        playerAttackTime -= Time.deltaTime;
        

        if (playerHealth > playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
        }
        if (iFrames > 0)
        {
            iFrames--;
        }
        if (iFrames < 0)
        {
            iFrames = 0;
        }
        if (playerKBTime <= 0)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpAnimTime = 0.2f;
                animator.SetTrigger("jumpStart");
            }
            else if (Input.GetButtonDown("Jump") && Buffer() && rb.linearVelocity.y < 0)
            {
                jumpQueued = true;
            }
            // if(Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            // {
            //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y *0.5f);
            // }

            if (IsGrounded())
            {
                animator.SetBool("isGrounded", true);
                animator.SetBool("isFalling", false);
                if (jumpQueued)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                    jumpQueued = false;
                    jumpAnimTime = 0.2f;
                    animator.SetTrigger("jumpStart");
                }
                grounded = true;
                airTime = 0.0f;

            }
            else
            {
                grounded = false;
                animator.SetBool("isGrounded", false);
            }

        }
        if (horizontal != 0 && currentSpeed > 0.1 || currentSpeed < -0.1)
        {
            if (dashTime < 0 && horizontal != 0)
            {
                dashDir = horizontal;
            }
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (airTime > 0.3f)
        {
            animator.SetBool("isFalling", true);
        }


        flip();

        if (playerKBTime <= 0 && playerAttackCD <= 0 )
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("attack");
                playerAttackTime=0.4f;

                playerAttackCD = 0.6f;
            }

        }
        if (playerAttackTime<0.2f && playerAttackTime >0.1f)
        {
            playerAttackBox.SetActive(true);
        }
        else
        {
            playerAttackBox.SetActive(false);
        }
        if(playerHealth<=0.0f && !isDead){
            animator.SetTrigger("death");
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            playerDeathAnimTime = 2.2f;
            playerHealth=-10000.0f;          
        }


    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0.0f, groundLayer);

    }
    private bool Buffer()
    {
        return Physics2D.OverlapBox(bufferCheck.position, queueCheckSize, 0.0f, groundLayer);
    }
    private void FixedUpdate()
    {
        if(!canInput) return;
        if(isDead) return;
        if (playerKBTime <= 0)
        {
            if (dashTime > 0)
            {
                rb.linearVelocity = new Vector2(dashDir * 30, 0.1f);
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
            }

        }
        if (Input.GetKey(KeyCode.LeftShift) && dashCD <= 0)
        {
            playerAttackCD = 0.25f;
            animator.SetTrigger("dash");
            dashTime = 0.25f;
            dashCD = 1.2f;
        }
    }
    private void flip()
    {
        if ((isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) && dashTime <= 0f && playerAttackCD<=0.0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.contacts[0].point;
        Vector2 pushDirection = ((Vector2)transform.position - contactPoint);
        pushDirection.x = GetDirection(contactPoint);
        HazardTagApplier enemy = collision.gameObject.GetComponent<HazardTagApplier>();
        if (enemy != null)
        {

            playerHealth -= enemy.damage;
            if (enemy.flashRed)
            {
                animator.SetTrigger("flashRed");
            }
            
            iFrames = 100;
            if (enemy.kbAmount > 0)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(pushDirection * enemy.kbAmount*1.5f, ForceMode2D.Impulse);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x , rb.linearVelocity.y+2);
            }
            if (enemy.willStun)
            {
                playerKBTime = 0.2f;
            }

        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 contactPoint = other.ClosestPoint(transform.position);
        Vector2 pushDirection = ((Vector2)transform.position - contactPoint);
        pushDirection.x = GetDirection(contactPoint);
        HazardTagApplier enemy = other.gameObject.GetComponent<HazardTagApplier>();
        if (enemy != null)
        {

            playerHealth -= enemy.damage;
            if (enemy.flashRed)
            {
                animator.SetTrigger("flashRed");
            }
            
            iFrames = 100;
            if (enemy.kbAmount > 0)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(pushDirection * enemy.kbAmount *1.5f, ForceMode2D.Impulse);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y+2);
            }
            if (enemy.willStun)
            {
                playerKBTime = 0.3f;
            }

        }
    }
    private int GetDirection(Vector2 collider)
    {
        if(transform.position.x > collider.x)
        {
            return 1;
        }
        if (transform.position.x < collider.x)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
    public void respawn()
    {
        playerHealth = playerMaxHealth;
        rb.transform.position = respawnPos;
        isDead = false;
        animator.SetTrigger("respawn");
        playerDeathScreen.SetActive(false);
        Time.timeScale=1;

    }


    public void Save(ref PlayerSaveData data)
    {
        data.position = transform.position;
    }
    public void load( PlayerSaveData data)
    {
        transform.position = data.position;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
}