using UnityEngine;
using System.Collections;

public class EnemyStats : CharacterStats
{
    public override void Die()
    {
        // Tell GameManager
        Destroy(gameObject);
    }
}
