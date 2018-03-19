using UnityEngine;
using System.Collections;

public class PlayerStats : CharacterStats
{
    public override void Die()
    {
        // Tell GameManager
        Destroy(gameObject);
    }
}
