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
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameUI gameUIref;
    private AudioSource damageSFX;

    private void Start() {
        currentHealth = maxHealth;
        damagePanel.SetActive(false);
        damageSFX = this.GetComponent<AudioSource>();
    }

    public void ResetHealth(){
        currentHealth = maxHealth;
    }

    public void DealDamage(float amount){
        StartCoroutine(TriggerDamagePanel(amount));
    }

    private IEnumerator TriggerDamagePanel(float amount){
        damagePanel.SetActive(true);
        damageSFX.Play();
        currentHealth -= amount;
        Debug.Log("Player Health: " + currentHealth);
        yield return new WaitForSeconds(damagePanelUpTime);
        damagePanel.SetActive(false);
        if(currentHealth <= 0){
            loseUI.SetActive(true);
            gameUIref.EndGame();
        }
    }
}
