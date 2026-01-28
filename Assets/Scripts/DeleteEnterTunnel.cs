using UnityEngine;

public class DeleteEnterTunnel : MonoBehaviour
{
    public bool playerCheck=true;
    public int timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() //timer so that there is time for Unity to register the players whereabouts
    {
        if (!playerCheck && timer >= 5)
        {
            Destroy(gameObject);
        }
        else
            timer++;
    }
    private void OnTriggerStay2D(Collider2D collision) //checks to see if the player spawns inside of this enter tunnel to prevent it being deleted
    {
        if (collision.gameObject.tag == "Player")
            playerCheck = true;
        else
            playerCheck = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            playerCheck = false;
    }
}
