using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerHealthUpdater : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damagePanelUpTime;
    [SerializeField] private GameObject damagePanel;
    private AudioSource damageSFX;

    private void Start() {
        currentHealth = maxHealth;
        damagePanel.SetActive(false);
        damageSFX = this.GetComponent<AudioSource>();
    }

    public void DealDamage(float amount){
        currentHealth -= amount;
        Debug.Log("Player Health: " + currentHealth);
        StartCoroutine(TriggerDamagePanel());
    }

    private IEnumerator TriggerDamagePanel(){
        damagePanel.SetActive(true);
        damageSFX.Play();
        yield return new WaitForSeconds(damagePanelUpTime);
        damagePanel.SetActive(false);
    }
}
