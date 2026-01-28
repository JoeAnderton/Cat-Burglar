using UnityEngine;

public class PoliceDog : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField]
    public float speed = 2.5f;
    float heldSpeed = 2.5f;
    bool grounded = false;
    bool turn = false;
    public bool checking = false;
    public bool sus = false;
    public bool arresting = false;
    int animationCounter= 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("arrested", arresting);
        if (rb.linearVelocity.x * transform.localScale.x > 1.0f)
            transform.localScale = new Vector3(-transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
        if (rb.linearVelocity.x * transform.localScale.x < -1.0f)
            transform.localScale = new Vector3(transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
        if (checking)
            animator.SetBool("checking",checking);
        if (sus) //does a temp check if you manage to escape. re-enetering during this is an instant capture
        {
            animationCounter++;
            animator.SetBool("sus", sus);
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Police Dog Sus") && animationCounter > 1)
            {
                sus = false;
                animator.SetBool("sus", sus);
                checking = false;
                animator.SetBool("checking",checking);
                animationCounter = 0;
            }
        }
    }
    void FixedUpdate()
    {
        if (grounded && !checking && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Police Dog Sus")&&!arresting) //walk cycle
        {
            speed = heldSpeed;
            rb.linearVelocity = new Vector2(speed, 0);
        }
        else
            rb.linearVelocity = new Vector2(0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            grounded = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "turnPoint"&&!turn) //turns the dog around
        {
            speed=-speed;
            heldSpeed = -heldSpeed;
            turn=true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "turnPoint")
            turn = false;
    }
}
