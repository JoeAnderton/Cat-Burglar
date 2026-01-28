using NUnit.Framework;
using UnityEngine;

public class Catfood : MonoBehaviour
{
    [SerializeField]
    GameObject GameManager;
    public int catfoodNumber;
    public bool catfoodCollect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update() //deletes itself upon collection
    {
        catfoodCollect = FindFirstObjectByType<GameManager>().catfoodGot[catfoodNumber];
        if (catfoodCollect)
        {
            Destroy(gameObject);
        }

    }
}
