using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System.Collections;

public class GameUI : MonoBehaviour
{

    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private float placeHoverDist;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject moveAwayUI;
    [SerializeField] private GameObject cantPlaceUI;
    [SerializeField] private float cantPlaceUIFlashTime;
    public TextMeshProUGUI stopwatchText; 
    public GameObject StartPlaceButton;
    private Button toggleButton;

    public GameObject objectivePrefab, Player;
    public float PlayerStartDist;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool isPressed = false;

    private GameObject objectiveTarget;
    private bool isPlacingGoal = false;
    private bool isGoalOnFloor = false;
    private bool isGameOver = false;
    
    private Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    
    private Vector3 camCenterWorldPositionOnPlane;


    void Start()
    {

        stopwatchText.text = FormatTime(elapsedTime);
        toggleButton = StartPlaceButton.GetComponent<Button>();
        toggleButton.onClick.AddListener(ButtonCallback);

        RestartGame();
    }

    public void RestartGame(){
        if(objectiveTarget != null) Destroy(objectiveTarget);

        winUI.SetActive(false);
        loseUI.SetActive(false);
        moveAwayUI.SetActive(false);
        this.cantPlaceUI.SetActive(false);

        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        objectivePrefab.SetActive(false);

        elapsedTime = 0f;
        stopwatchText.text = FormatTime(elapsedTime);

        isPlacingGoal = false;
        isGoalOnFloor = false;
        isRunning = false;
        isGameOver = false;
    }

    public void EndGame(){
        isRunning = false;
        isGameOver = true;
        if(objectiveTarget != null) Destroy(objectiveTarget);
        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Replay?";
        StartPlaceButton.SetActive(true);
    }

    void Update()
    {

        if (isRunning)
        {
            if(objectiveTarget != null){
                if(objectiveTarget.GetComponent<Objective>().isEntered){
                    winUI.SetActive(true);
                    EndGame();
                }
            }

            elapsedTime += Time.deltaTime;
            stopwatchText.text = FormatTime(elapsedTime);
        }
    }

    private IEnumerator UpdateGoalPlacePosition(){
        if(!isPlacingGoal) yield break;
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        List<ARRaycastHit> hits = new();
        raycastManager.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
        if(hits.Count > 0){
            this.camCenterWorldPositionOnPlane = hits[0].pose.position;
            objectiveTarget.transform.position = this.camCenterWorldPositionOnPlane;
            isGoalOnFloor = true;
        } else {
            Vector3 hoverPosition = Player.transform.forward * placeHoverDist;
            hoverPosition += Player.transform.position;
            objectiveTarget.transform.position = hoverPosition;
            isGoalOnFloor = false;
            Debug.Log("Player: " + hoverPosition);
        }

        Debug.Log(hits.Count);
        yield return new WaitForFixedUpdate();
        StartCoroutine(UpdateGoalPlacePosition());
    }

    private IEnumerator CheckGoalDistance(){
        if(objectiveTarget == null) yield break;
        if(Vector3.Distance(objectiveTarget.transform.position, Player.transform.position) >= PlayerStartDist){
            isRunning = true;

            moveAwayUI.SetActive(false);
            yield break;
        }
        moveAwayUI.SetActive(true);
        yield return new WaitForEndOfFrame();
        StartCoroutine(CheckGoalDistance());
    }

    void ButtonCallback()
    {
        if(isGameOver){
            RestartGame();
            return;
        }

        if(!isPlacingGoal){
            objectiveTarget = GameObject.Instantiate(objectivePrefab);
            objectiveTarget.SetActive(true);
            isPlacingGoal = true;
            toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Place";
            this.StartCoroutine(UpdateGoalPlacePosition());
            return;
        }

        if(isGoalOnFloor){
            isPlacingGoal = false;
            StartPlaceButton.SetActive(false);

            StartCoroutine(CheckGoalDistance());
        } else {
            StartCoroutine(BlinkCantPlace());
        }
    }

    private IEnumerator BlinkCantPlace(){
        this.cantPlaceUI.SetActive(true);
        yield return new WaitForSeconds(cantPlaceUIFlashTime);
        this.cantPlaceUI.SetActive(false);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    public bool IsRunning()
    {
        return isRunning;
    }

}
