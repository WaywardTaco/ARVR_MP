using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireDamager : MonoBehaviour
{
    [SerializeField] private float fireDamage;
    [SerializeField] private float fireDamageInterval;
    private PlayerHealthUpdater playerHealthUpdater;
    private void Update() {
        Debug.Log("Update Function");
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<PlayerHealthUpdater>() != null){
            this.playerHealthUpdater = other.gameObject.GetComponent<PlayerHealthUpdater>();
            StartCoroutine(DealFireDamageAtInterval());
        }
    }

    private IEnumerator DealFireDamageAtInterval(){
        yield return new WaitForSeconds(fireDamageInterval);
        playerHealthUpdater.DealDamage(fireDamage);
        StartCoroutine(DealFireDamageAtInterval());
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<PlayerHealthUpdater>() != null){
            this.StopAllCoroutines();
        }
    }
}
