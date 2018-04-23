using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {
    public float pacedTime = 0;
    private float paceToTime = 0;
    public float pauseTime = 1;
    public bool playerInBossRoom = false;
    public GameObject targetPlayer;
    private Vector3 vectorToPlayer;
    private BossState state = BossState.idle;
    public int attackRange;
    Animator bossAnimator;
    public float moveSpeed = 1f;
    public Vector3 pacingTo;
    private float paceDistance = 0;
    private float pausedTime = 0;
    float paceAngle;
    float percentBetweenWaypoints;
    public float easeAmount;
    private Vector3 fromPlace;


    enum BossState {
        pacing, idle, groundSlam, dying, punching, following
    }



    void Start () {
        bossAnimator = GetComponent<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () {
        vectorToPlayer = targetPlayer.transform.position - transform.position;
        vectorToPlayer.y = 0;

        switch (state) {
            case BossState.idle:
                if (playerInBossRoom) {
                    if(vectorToPlayer.magnitude > attackRange) {
                        switchToFollowing();
                        state = BossState.following;
                    } else {
                        state = chooseBossAttack();
                    }
                }
                if (pausedTime >= pauseTime) {
                    switchToPacing();
                } else {
                    pausedTime += Time.deltaTime;
                }
                break;
            case BossState.pacing:
                if (playerInBossRoom) {
                    if (vectorToPlayer.magnitude > attackRange) {
                        state = BossState.following;
                    }
                }
                if (transform.position.x <= pacingTo.x && transform.position.z <= pacingTo.z) {
                    transform.position = calculatePathing(Time.deltaTime);
                } else {
                    switchToIdle();
                }
                break;
            case BossState.following:

                break;
        }
	}
    float Ease(float x) {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
    public Vector3 calculatePathing(float deltaTime) {
        percentBetweenWaypoints += deltaTime * moveSpeed;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(fromPlace, pacingTo, easedPercentBetweenWaypoints);
        if (percentBetweenWaypoints >= 1) {
            switchToIdle();
        }

        return newPos - transform.position;

    }
    private BossState chooseBossAttack() {
        return BossState.idle;
    }

    private void switchToFollowing() {
        bossAnimator.SetInteger("AnimNum", 9);
    }

    private void switchToIdle() {
        bossAnimator.SetInteger("AnimNum", 1);
        pacedTime = 0;
        state = BossState.idle;
    }
    
    private void switchToPacing() {
        pacingTo = choosePointInBossRoom();
        state = BossState.pacing;
        pausedTime = 0;
        transform.LookAt(pacingTo);
        fromPlace = transform.position;
        paceDistance = Vector3.Distance(transform.position, pacingTo);
        paceToTime = paceDistance * 20;
        
        paceAngle = Vector3.Angle(transform.position, pacingTo);
        bossAnimator.SetInteger("AnimNum", 9);
    }

    public Vector3 choosePointInBossRoom() {
        float xLoc = Random.Range(-1f,1f);
        float yLoc = Random.Range(-1f,-3f);
        Debug.Log(" Moving to: " + xLoc + " " + yLoc);
        Vector3 returnVec = new Vector3(xLoc, 0, yLoc);
        return returnVec;
    }
}
