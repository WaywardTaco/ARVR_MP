using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    public bool isEntered = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealthUpdater>() != null)
        {
            GameUI gameUI = GameObject.Find("Canvas").GetComponent<GameUI>();
            if(gameUI != null){
                if(gameUI.isRunning){
                    isEntered = true;
                }
            }
        }
        else
        {
            isEntered = false;
        }
    }

}
