using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyController))]
public class EnemyAI : MonoBehaviour
{
    public bool debug = true;
    enum AIState
    {
        Idle, Following, AttackInitiated, Attacking
    }
    private EnemyController controller;
    private GameObject targetPlayer;
    private Vector3 vectorToPlayer;
    [Header("Search Parameters")]
    // Enemy starts to follow the player when within its searchRadius
    [SerializeField]
    private float searchRadius;
    // Enemy continues following until distance to player exceeds the searchRadius plus the addedFollowDistance
    [SerializeField]
    private float addedFollowDistance;
    [Header("Attack Parameters")]
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float attackCooldown;
    private AIState state = AIState.Idle;

    float inputHorizontal = 0f;
    float inputVertical = 0f;
    float inputBlock = 0f;
    bool inputLightHit;
    bool inputDeath;
    bool inputAttackR;
    bool inputAttackL;
    
    void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        controller = GetComponent<EnemyController>();
        if (debug)
        {
            Debug.Log(gameObject.name + ": is targeting " + targetPlayer.name);
        }
    }
    
    void Update()
    {
        if (targetPlayer == null)
        {
            inputHorizontal = 0f;
            inputVertical = 0f;
            targetPlayer = GameObject.FindGameObjectWithTag("Player");
            if (targetPlayer == null)
            {
                return;
            }
        }

        vectorToPlayer = targetPlayer.transform.position - transform.position;
        vectorToPlayer.y = 0;
        switch (state)
        {
            case AIState.Idle:
                if (vectorToPlayer.magnitude < searchRadius)
                {
                    state = AIState.Following;
                    if (debug)
                    {
                        Debug.Log(gameObject.name + ": Player found.");
                    }
                }
                break;
            case AIState.Following:
                if (vectorToPlayer.magnitude > searchRadius + addedFollowDistance)
                {
                    state = AIState.Idle;
                    inputHorizontal = 0f;
                    inputVertical = 0f;
                    if (debug)
                    {
                        Debug.Log(gameObject.name + ": Lost sight of player.");
                    }
                }
                else if (vectorToPlayer.magnitude < attackRange)
                {
                    state = AIState.AttackInitiated;
                    if (debug)
                    {
                        Debug.Log(gameObject.name + ": Attacking player.");
                    }
                }
                else
                {
                    // Follow player
                    inputHorizontal = vectorToPlayer.normalized.x;
                    inputVertical = vectorToPlayer.normalized.z;
                }
                break;
            case AIState.AttackInitiated:
                inputHorizontal = 0f;
                inputVertical = 0f;
                InvokeRepeating("AttackPlayer", attackDelay, attackCooldown);
                state = AIState.Attacking;
                break;
            case AIState.Attacking:
                if (vectorToPlayer.magnitude > attackRange)
                {
                    CancelInvoke("AttackPlayer");
                    state = AIState.Following;
                    if (debug)
                    {
                        Debug.Log(gameObject.name + ": Resumed chasing player.");
                    }
                }
                break;
            default:
                break;
        }

        // Controller input
        controller.inputHorizontal = inputHorizontal;
        controller.inputVertical = inputVertical;
        controller.inputAttackL = inputAttackL;
        controller.inputAttackR = inputAttackR;
        controller.inputBlock = inputBlock;
        controller.inputLightHit = inputLightHit;
        controller.inputDeath = inputDeath;
    }

    void AttackPlayer()
    {
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        if (Random.value < 0.5f)
        {
            inputAttackL = true;
        }
        else
        {
            inputAttackR = true;
        }
        Invoke("CancelAttack", 0.1f);
    }

    void CancelAttack()
    {
        inputAttackL = false;
        inputAttackR = false;
    }
}
