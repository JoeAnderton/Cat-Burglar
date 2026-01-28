using UnityEngine;

public class LookDown : MonoBehaviour
{
    public float lookDownDistance=2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()  //moves down the box collider which the camera follows
    {
        if (Input.GetKeyDown("e") && !FindFirstObjectByType<GameManager>().endGame)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - lookDownDistance, transform.localPosition.z);
        }
        if (Input.GetKeyUp("e") && !FindFirstObjectByType<GameManager>().endGame)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + lookDownDistance, transform.localPosition.z);
        }
    }
}
