using UnityEngine;

[CreateAssetMenu(menuName = "Level/Connection")] //makes the code appear in the right-click menu
public class levelConnection : ScriptableObject //allows the code to be reused quickly
{
    [SerializeField]
    private levelConnection connection;
    public static levelConnection activeConnection { get; set; } //gets and sets it as the current levelConnection
}
