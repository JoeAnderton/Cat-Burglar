using UnityEngine;

public class Laser : MonoBehaviour
{
    Animator animator;
    public bool triggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() //turns the laser off if triggered
    {
        animator.SetBool("triggered", triggered);
    }
}
