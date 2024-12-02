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
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<PlayerHealthUpdater>() != null){
            this.playerHealthUpdater = other.gameObject.GetComponent<PlayerHealthUpdater>();
            StartCoroutine(DealFireDamageAtInterval());
        }
    }

    private IEnumerator DealFireDamageAtInterval(){
        playerHealthUpdater.DealDamage(fireDamage);
        yield return new WaitForSeconds(fireDamageInterval);
        StartCoroutine(DealFireDamageAtInterval());
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<PlayerHealthUpdater>() != null){
            this.StopAllCoroutines();
        }
    }
}
