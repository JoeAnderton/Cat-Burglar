using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public TextMeshProUGUI results;
    public TextMeshProUGUI catfoodCount;
    public TextMeshProUGUI timesCaught;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI rankScore;
    public TextMeshProUGUI restartMessage;
    public Transform messageUI;
    public Transform endGameUI;
    public Transform startGameUI;
    [SerializeField]
    int totalCatfood = 2;
    public bool hasBox = false;
    public bool catInBox = false;
    public bool shownBoxMessage = false;
    public bool endGame = false;
    bool canRestart = false;
    public int refund = 0;
    public float totalCatfoodCollected = 0;
    public float timesArrested = 0;
    public List<bool> catfoodGot = new List<bool>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static bool alreadyManaged = false;

    void Awake() //deletes duplicate game managers if and when they are made
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
        // Check if an instance already exists
        if (!alreadyManaged)
        {
            alreadyManaged = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        FindFirstObjectByType<Player>().hasGameManager=true;
        DontDestroyOnLoad(this); //so it doesn't reset on scene changes
        for (int i = 0; i < totalCatfood; i++) //creates a list of which catfood haven't been taken
        {
            catfoodGot.Add(false);
        }
        StartCoroutine(StartMessage(4)); //since it's only added in scene 1, it can give the tutorial message automatically
    }

    // Update is called once per frame
    void Update()
    {
        FindFirstObjectByType<Player>().hasGameManager = true;
        if (hasBox&&!shownBoxMessage) //using box tutoiral
        {
            StartCoroutine(DoMessage(4));
        }
        if (!hasBox)
        {
            shownBoxMessage = false;
            messageUI.gameObject.SetActive(false);
        }
        string uiString = totalCatfoodCollected + "";
        uiText.text = uiString;
        if (endGame) //shows the results
        {
            StartCoroutine(EndMessage(2));
        }
        if (canRestart && Input.GetKeyDown("q")) //restarts the game
        {
            SceneManager.LoadScene("Room 1");
            alreadyManaged = false;
            Destroy(gameObject);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //subtracts any catfood obtained in a room where you're caught
    {
        totalCatfoodCollected = totalCatfoodCollected - refund;
        refund = 0;
    }
    IEnumerator DoMessage(float seconds) //the box tutorial
    {
        messageUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        messageUI.gameObject.SetActive(false);
        shownBoxMessage = true;
    }
    IEnumerator StartMessage(float seconds) //the start tutorial
    {
        startGameUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        startGameUI.gameObject.SetActive(false);
    }
    IEnumerator EndMessage(float seconds) //the results screen
    {
        string endGameText = "Catfood collected: " + totalCatfoodCollected;
        catfoodCount.text = endGameText;
        endGameText = "Times caught: " + timesArrested;
        timesCaught.text = endGameText;
        rankScore.text = ScoreCalculator();
        endGameUI.gameObject.SetActive(true);
        results.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        catfoodCount.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        timesCaught.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        rank.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        rankScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        restartMessage.gameObject.SetActive(true);
        canRestart = true;
    }

    string ScoreCalculator() //attempts to create an appropriate score
    {
        if (totalCatfoodCollected == 10 && timesArrested == 0)
            return "S";
        else if (timesArrested > 0)
        {
            if (totalCatfoodCollected / timesArrested >= 3f / 4f && totalCatfoodCollected > 8)
                return "A";
            else if (totalCatfoodCollected / timesArrested >= 2f/ 3f && totalCatfoodCollected > 6)
                return "B";
            else if (totalCatfoodCollected / timesArrested >= 1f / 2f && totalCatfoodCollected > 4)
                return "C";
            else if (totalCatfoodCollected / timesArrested >= 1f / 3f && totalCatfoodCollected > 0)
                return "D";
            else if (timesArrested > 0)
                return "F";
        }
        else if (timesArrested == 0)
        {
            if (totalCatfoodCollected == 0 && timesArrested == 0)
                return "P";
            else if (totalCatfoodCollected >= 8 && timesArrested == 0)
                return "A";
            else if (totalCatfoodCollected >= 6 && timesArrested == 0)
                return "B";
            else if (totalCatfoodCollected >= 4 && timesArrested == 0)
                return "C";
            else if (totalCatfoodCollected > 0 && timesArrested == 0)
                return "D";
        }
        
            return "F";
    }

}
