using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject wielder;
    private bool active = false;
    private int attackPower;

    public void Attack(int power, float duration, float delay=0f)
    {
        if (!active)
        {
            attackPower = power;
            StartCoroutine(PerformAttack(duration, delay));
        }

    }

    IEnumerator PerformAttack(float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        //Debug.Log("Attacked with " + gameObject.name);
        active = true;
        yield return new WaitForSeconds(duration);

        //Debug.Log("Finished attack");
        active = false;
        attackPower = 0;
    }

    void OnTriggerEnter(Collider col)
    {
        if (active)
        {
			if (wielder.tag == "Player" && col.tag == "Enemy") {
				//Debug.Log(col.name + " has been hit.");
				col.GetComponent<CharacterStats> ().TakeDamage (attackPower);
				col.GetComponent<EnemyController> ().GetHit (); // Maybe make a Controller interface or super class
			} else if (wielder.tag == "Player" && col.tag == "Boss") {
				col.GetComponent<CharacterStats> ().TakeDamage (attackPower);
				col.GetComponent<BossController> ().getHit ();
			}
            else if (wielder.tag == "Enemy" && col.tag == "Player")
            {
                //Debug.Log(col.name + " has been hit.");
                col.GetComponent<CharacterStats>().TakeDamage(attackPower);
            }

        }
    }
}
