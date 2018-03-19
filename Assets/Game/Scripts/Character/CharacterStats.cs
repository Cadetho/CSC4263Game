using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth;
    public int maxHealth;
    public int shield;
    public int attackPower;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= Mathf.Max(damage - shield, 0);
        Debug.Log(gameObject.name + " has taken " + damage + " damage.\n Health = is now " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log(gameObject.name + " has died.");
    }
}