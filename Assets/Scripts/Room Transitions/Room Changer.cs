using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChanger : MonoBehaviour
{
    [SerializeField]
    private levelConnection connection;
    [SerializeField]
    public string targetScene; //destination scene
    [SerializeField]
    private Transform spawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (connection == levelConnection.activeConnection) //sees if the player is entering from this enterance
        {
            FindFirstObjectByType<Player>().transform.position = spawnPoint.position; //sets the player to the correct spawn point
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player != null)
        {
            levelConnection.activeConnection = connection; //makes the current connection script the active one
            SceneManager.LoadScene(targetScene); //loads the destination scene
        }
    }
}
