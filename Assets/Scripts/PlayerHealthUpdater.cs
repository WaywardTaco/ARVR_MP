using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerHealthUpdater : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void DealDamage(float amount){
        currentHealth -= amount;
        Debug.Log("Player Health: " + currentHealth);
    }
}
