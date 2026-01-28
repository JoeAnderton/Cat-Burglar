using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    Rigidbody2D rigidBody;
    public GameObject policeDogSpeed;
    public GameObject GameManager;
    GameObject currentCatfood;
    Animator animator;
    public Animator otherAnimator;
    public AudioSource dogBark;
    public AudioSource catfoodCollect;
    public AudioSource alarm;
    public float speed = 5.0f;
    public float boxHeight = 1;
    public bool grounded = false;
    public bool inExitTunnel = false;
    public bool inEnterTunnel = false;
    public bool isCaught;
    public bool stickLanding = false;
    public bool hasGameManager = false;
    bool addedToCaughtTotal = false;
    bool inRoomWithBox = false;
    bool dogBarked;
    [SerializeField]
    bool boxed = false;
    [SerializeField]
    bool canBox = false;
    bool stayInBox = false;
    public string enterTunnelDirection;
    public string exitTunnelDirection;
    public float jumpSpeed = 0f;
    public float fallMulti = 2.5f;
    public float lowJumpMulti = 2f;
    [SerializeField]
    int upExitDirection = 1;
    public int enterTotal;
    public int resetCounter = 0;
    public int resetCounterMax = 60;
    public List<int> catfoodGot = new List<int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        otherAnimator = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(8,9,true); //so you can walk past dogs
        canBox = FindFirstObjectByType<GameManager>().hasBox;
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        enterTotal = GameObject.FindGameObjectsWithTag("enterTunnel").Length; //mainly used to make sure you're not in an enter tunnel, since they delete themselves
        if (rigidBody.linearVelocity.x * transform.localScale.x > 1.0f)
            transform.localScale = new Vector3(-transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
        if (rigidBody.linearVelocity.x * transform.localScale.x < -1.0f)
            transform.localScale = new Vector3(transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
        if (Input.GetButtonDown("Jump") && grounded && !isCaught && !boxed && !(inEnterTunnel&&enterTunnelDirection=="up") && !stickLanding && !FindFirstObjectByType<GameManager>().endGame) {
                rigidBody.linearVelocity = new Vector2(0, jumpSpeed);
        }
        float xSpeed = Mathf.Abs(rigidBody.linearVelocity.x);
        animator.SetFloat("xSpeed", xSpeed);
        float ySpeed = Mathf.Abs(rigidBody.linearVelocity.y);
        animator.SetFloat("yspeed", ySpeed);
        animator.SetBool("grounded", grounded);
        animator.SetBool("caught", isCaught);
        if (rigidBody.linearVelocityY > 1 || rigidBody.linearVelocityY < -1)
            grounded = false;
        if (otherAnimator.GetCurrentAnimatorStateInfo(0).IsName("Police Dog Arrest") && otherAnimator != null) //checks to see if the cat is being arrested
            arrested(true);
        if (Input.GetAxisRaw("Vertical") == -1f && !boxed && grounded && canBox) //puts on the box
        {
            boxed = true;
            FindFirstObjectByType<GameManager>().catInBox = true;
            animator.SetBool("boxed", boxed);
            speed = 2.5f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - boxHeight, transform.localPosition.z); //these are so the player doesn't glitch through the ground
        }
        if (Input.GetAxisRaw("Vertical") == 1f && boxed && !stayInBox) //takes off the box
        {
            boxed = false;
            FindFirstObjectByType<GameManager>().catInBox = false;
            animator.SetBool("boxed", boxed);
            speed = 5;
            transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y + boxHeight, transform.localPosition.z);
        }
    }
    void FixedUpdate()
    {
        if (isCaught)
        {
            if(grounded)
            resetCounter++;
            if (resetCounter > resetCounterMax)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); //restarts scene if caught
            }
        }
        float h = Input.GetAxisRaw("Horizontal"); 
            //these have a bunch of checks to see if the player is in control
            if (!isCaught && h == 1f && !(inEnterTunnel && (enterTunnelDirection == "up")) && !(inExitTunnel && (exitTunnelDirection == "up")) && !((inExitTunnel && exitTunnelDirection == "left") || (inEnterTunnel && enterTunnelDirection == "left" && enterTotal > 0)) || (inExitTunnel && exitTunnelDirection == "right") || (inEnterTunnel && enterTunnelDirection == "right" && enterTotal > 0))
            {
                rigidBody.linearVelocity = new Vector2(speed, rigidBody.linearVelocityY);
            }
            else if (!isCaught && h == -1f && !(inEnterTunnel && (enterTunnelDirection == "up")) && !(inExitTunnel && (exitTunnelDirection == "up")) || (inExitTunnel && exitTunnelDirection == "left") || (inEnterTunnel && enterTunnelDirection == "left"&&enterTotal>0))
            {
                rigidBody.linearVelocity = new Vector2(-speed, rigidBody.linearVelocityY);
            }
            else
            {
                //immediately stops player if no input
                rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocityY);
            }
        if (inExitTunnel && exitTunnelDirection == "up" || inEnterTunnel && enterTunnelDirection == "up" || stickLanding)
        {
            float move = -1*upExitDirection;
            if (inExitTunnel)
                move = rigidBody.linearVelocityX;
            if (inEnterTunnel || stickLanding)
                move = -3*upExitDirection;      //so the player automatically lands on the ground when going through up entrances
            if (stickLanding && grounded)
                stickLanding = false;
            if (!stickLanding && !grounded)
                rigidBody.linearVelocityY = 12;
            rigidBody.linearVelocity = new Vector2(move, rigidBody.linearVelocityY); 
        }
        if (rigidBody.linearVelocity.y < 0)
            rigidBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMulti - 1) * Time.deltaTime;
        else if (rigidBody.linearVelocity.y > 0 && ((!Input.GetButton("Jump") && !(inExitTunnel && exitTunnelDirection == "up")) || inEnterTunnel && enterTunnelDirection == "up" || stickLanding))
        {
            rigidBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMulti - 1) * Time.deltaTime; //falls faster if in air for longer
        }
        }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && (rigidBody.linearVelocityY<1||rigidBody.linearVelocityY>-1))
        {
            grounded = true; //checks if on the ground
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Tuna") //collecting catfood
        {
            if (!isCaught)
            {
                catfoodCollect.Play();
                FindFirstObjectByType<GameManager>().totalCatfoodCollected++;
                currentCatfood = coll.gameObject;
                int catfoodNumber = currentCatfood.GetComponent<Catfood>().catfoodNumber;
                FindFirstObjectByType<GameManager>().catfoodGot[currentCatfood.GetComponent<Catfood>().catfoodNumber] = true;
                catfoodGot.Add(catfoodNumber);
            }
        }
        if (coll.gameObject.tag == "dog" && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Cat Burglar Box Idle")&&grounded)) //seen by police dog
        {
            otherAnimator = coll.gameObject.GetComponentInParent<Animator>();
            policeDogSpeed = coll.gameObject;
            if (!policeDogSpeed.GetComponentInParent<PoliceDog>().checking || !policeDogSpeed.GetComponentInParent<PoliceDog>().sus) 
            {
                policeDogSpeed.GetComponentInParent<PoliceDog>().checking = true; //starts the countdown to be arrested
            }
            else
            {
                policeDogSpeed.GetComponentInParent<PoliceDog>().arresting = true; //actually arrested
                arrested(true);
            }
            policeDogSpeed = this.gameObject; //added this to stop null error appearing after stopping game. Has no impact on actual game.
        }
        if (coll.gameObject.tag == "Laser")
        {
            coll.gameObject.GetComponent<Laser>().triggered = true;
            arrested(false);
        }
        if (coll.gameObject.tag == "box")
        {
            FindFirstObjectByType<GameManager>().hasBox=true;
            canBox = true;
            inRoomWithBox = true;
        }
        if (coll.gameObject.tag == "inBox")
        {
            stayInBox = true; //stops the player leaving the box when the height is too small
            if (!boxed)
            {
                animator.SetBool("boxed", true);
                boxed = true;
                speed = 2.5f;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - boxHeight, transform.localPosition.z);
            }
        }
        if (coll.gameObject.tag == "exitTunnel" && enterTotal == 0 && !inEnterTunnel) {
            inExitTunnel = true;
            if (coll.gameObject.layer == 6)
                exitTunnelDirection = "left";
            if (coll.gameObject.layer == 7)
                exitTunnelDirection = "right";
            if (coll.gameObject.layer == 11)
                exitTunnelDirection = "up";
        }
        if (coll.gameObject.tag == "enterTunnel")
        {
            inEnterTunnel = true;
            inExitTunnel = false; //takes priority over exiting
            if (coll.gameObject.layer == 6)
                enterTunnelDirection = "left";
            if (coll.gameObject.layer == 7)
                enterTunnelDirection = "right";
            if (coll.gameObject.layer == 11)
                enterTunnelDirection = "up";
            if (coll.gameObject.layer == 12) 
                enterTunnelDirection = "down";
        }
        if (coll.gameObject.tag == "endGame") //ends the game
            FindFirstObjectByType<GameManager>().endGame = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {   //in case the player moves in the box whilst the dog is watching
        if (collision.gameObject.tag == "dog" && !isCaught && !otherAnimator.GetCurrentAnimatorStateInfo(0).IsName("Police Dog Check") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Cat Burglar Box Idle"))
        {
            otherAnimator = collision.gameObject.GetComponentInParent<Animator>();
            policeDogSpeed = collision.gameObject;
            policeDogSpeed.GetComponentInParent<PoliceDog>().checking = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enterTunnel")
        {
            inEnterTunnel = false;
            if (collision.gameObject.layer == 11)
            {
                stickLanding = true;
            }
        }
        if (collision.gameObject.tag == "exitTunnel")
            inExitTunnel = false;
        if (collision.gameObject.tag == "inBox")
            stayInBox = false;
        if (collision.gameObject.tag == "dog" && !isCaught)
        {
            otherAnimator = collision.gameObject.GetComponentInParent<Animator>();
            if (!otherAnimator.GetCurrentAnimatorStateInfo(0).IsName("Police Dog Sus"))
            {
                policeDogSpeed = collision.gameObject;
                policeDogSpeed.GetComponentInParent<PoliceDog>().sus = true;
            }
        }

    }
    private void arrested(bool dogOrAlarm) //true for dog bark sound effect
    {
        if (inRoomWithBox) //removes box power up if not escaped the room with it
        {
            canBox = false;
            FindFirstObjectByType<GameManager>().hasBox = false;
        }
        if (!dogBarked)
        {
            if (dogOrAlarm)
                dogBark.Play();
            else
                alarm.Play();
            dogBarked = true;
        }
        isCaught = true;
        if (!addedToCaughtTotal)
        {
            FindFirstObjectByType<GameManager>().timesArrested++;
            addedToCaughtTotal = true;
        }
        for (int i = 0; i < catfoodGot.Count; i++)
        {
            if (FindFirstObjectByType<GameManager>().catfoodGot[catfoodGot[i]])
                FindFirstObjectByType<GameManager>().refund++;
            FindFirstObjectByType<GameManager>().catfoodGot[catfoodGot[i]] = false; //returns all catfood to the room you were caught in
        }
    }
}
