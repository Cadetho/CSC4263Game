using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {
    public float paceTime;
    public float pauseTime;
    public bool playerInBossRoom = false;
    public GameObject targetPlayer;
    private Vector3 vectorToPlayer;
    private BossState state = BossState.idle;
    public int attackRange;
    Animator bossAnimator;
    public float moveSpeed;
    public Vector3 pacingTo;
    private float pausedTime = 0;

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
                if (transform.position != pacingTo) {
                    transform.position = Vector3.MoveTowards(transform.position, pacingTo, moveSpeed * Time.deltaTime);
                } else {
                    switchToIdle();
                }
                break;
            case BossState.following:

                break;
        }
	}

    private BossState chooseBossAttack() {
        return BossState.idle;
    }

    private void switchToFollowing() {
        bossAnimator.SetFloat("AnimNum", 9);
    }

    private void switchToIdle() {
        bossAnimator.SetFloat("AnimNum", 1);
    }
    
    private void switchToPacing() {
        pacingTo = choosePointInBossRoom();
        state = BossState.pacing;
        pausedTime = 0;
        bossAnimator.SetFloat("AnimNum", 9);
    }

    public Vector3 choosePointInBossRoom() {
        return Vector3.zero;
    }
}
