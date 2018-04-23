using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {
    private float pauseTime = 3;
    public bool playerInBossRoom = false;
    public GameObject targetPlayer;
    private Vector3 vectorToPlayer;
    private BossState state = BossState.idle;
    public int attackRange;
    Animator bossAnimator;
	
    private float moveSpeed = 0.1f;
	
    public Vector3 pacingTo;
   
    private float pausedTime = 0;

	public float groundSlamCooldown = 10;
	public float groundSlamCooldownTime = 0;
	public float groundSlamPrepTime = 1;
	private float groundSlamPrepTimeElapsed = 0;
	
    private Vector3 fromPlace;
	private float distanceTime;
	private float startPaceTime = 0;
	private float relativeTime = 0;
	private float distanceBetweenPoints = 0;
	private bool groundSlamFade = false;
	public GameObject groundSlamPrefab;
	private float groundSlamAlphaTime;
	private GameObject groundSlam;
	private float lastColorChangedTime;
	private Color fullColor = new Color(1f,0f,0f,0.53f);
	private Color endColor = new Color (1f, 0f, 0f, 0.07f);
	private Material groundSlamMat;
	private float ratio;
	private Color startColor;
	private int groundSlamTime = 10;
	private float groundSlamTimeElapsed = 0;
	private bool groundPulsing;
	private bool takenDamageCurrentPulse;
	private float pulseStartTime = 0.37f;
	private float puseBetweenTime = 0.1f;
	private float timeToNextPulse = 0;
	private bool inGroundSlam = false;
	private float punchHitCooldown = 1f;
	private float punchHitElapsed = 0;
	private float bossHitTime = 1.1f;
	private float bossHitTimeElapsed = 0;
	private BossState interruptedState;
	private int interruptedAnim;


    enum BossState {
        pacing, idle, groundSlam, dying, punching, following, getHit
    }



	void Start () {
		startColor = fullColor;
        bossAnimator = GetComponent<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () {
        vectorToPlayer = targetPlayer.transform.position - transform.position;
        vectorToPlayer.y = 0;
		groundSlamCooldownTime += Time.deltaTime;
		switch (state) {
		case BossState.idle:
			if (playerInBossRoom) {
				if (vectorToPlayer.magnitude > attackRange) {
					switchToFollowing ();
					state = BossState.following;
				} else {
					state = chooseBossAttack ();
				}
			}
			if (pausedTime >= pauseTime) {
				switchToPacing ();
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
				transform.position = Vector3.MoveTowards (transform.position, pacingTo, 0.002f);
			} else {
				switchToIdle ();
			}
			break;
		case BossState.following:
			if (vectorToPlayer.magnitude > attackRange) {
				transform.position = Vector3.MoveTowards (transform.position, targetPlayer.transform.position, 0.002f);
				transform.LookAt (targetPlayer.transform.position);
			} else {
				switchToAttacking ();
			}
			break;
		case BossState.groundSlam:
			if (groundSlam == null) {
				createGroundSlamArea ();
			}
			if (bossAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Attack02")) {

				groundSlamPrepTimeElapsed += Time.deltaTime;
				float groundSlamAlpha;

				Color groundSlamAlphaColor;

				ratio = (float)((Time.time - lastColorChangedTime) / .25);
				ratio = Mathf.Clamp01 (ratio);
				groundSlamMat.color = Color.Lerp (startColor, endColor, ratio);
				if (ratio == 1f) {
					lastColorChangedTime = Time.time;
					var temp = startColor;
					startColor = endColor;
					endColor = temp;
				}
				if (groundSlamPrepTimeElapsed >= groundSlamPrepTime) {
					bossAnimator.SetInteger ("AnimNum", 8);
					groundSlamMat.color = fullColor; 
					inGroundSlam = true;
				}
			}
			if (inGroundSlam) {
//				if (timeToNextPulse == 0) {
//					timeToNextPulse = Time.time + pulseStartTime;
//				} 
				float groundSlamAlpha;
				Color groundSlamAlphaColor;
				ratio = (float)((Time.time - lastColorChangedTime) / .25);
				ratio = Mathf.Clamp01 (ratio);
				groundSlamMat.color = Color.Lerp (startColor, endColor, ratio);
				if (ratio == 1f) {
					lastColorChangedTime = Time.time;
					var temp = startColor;
					startColor = endColor;
					endColor = temp;
				}

				if (vectorToPlayer.magnitude <= 0.5 && (timeToNextPulse >= Time.time)){
					targetPlayer.GetComponent<CharacterController> ().stats.TakeDamage (2);
				}
				if (Time.time >= timeToNextPulse) {
					Debug.Log ("pulse");
					timeToNextPulse = timeToNextPulse + puseBetweenTime;
				}
				groundSlamTimeElapsed += Time.deltaTime;
				Debug.Log (groundSlamTime);
				if (groundSlamTimeElapsed >= groundSlamTime) {
					destroyGroundSlamArea ();
				}
			}
			break;
		case BossState.punching:
			if (vectorToPlayer.magnitude >= 0.6) {
				switchToFollowing ();
			}

			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit)) {
				if (hit.transform.name == "Player" && (punchHitElapsed >= punchHitCooldown)){
					punchHitElapsed = 0;
					targetPlayer.GetComponent<CharacterController> ().stats.TakeDamage (5);
				}
			}
			punchHitElapsed += Time.deltaTime;
			break;

		case BossState.getHit:
			bossHitTimeElapsed += Time.deltaTime;
			if (bossHitTimeElapsed > bossHitTime) {
				resumeSuspendedState ();
			}
			break;
		}
	}
	public void playerInRoom(){
		playerInBossRoom = true;
	}
    private BossState chooseBossAttack() {
        return BossState.idle;
    }

    private void switchToFollowing() {
        bossAnimator.SetInteger("AnimNum", 9);
    }

    private void switchToIdle() {
        bossAnimator.SetInteger("AnimNum", 1);
        state = BossState.idle;
    }
    
    private void switchToPacing() {
		pausedTime = 0;
		startPaceTime = Time.time;
        pacingTo = choosePointInBossRoom();
		distanceBetweenPoints = Vector3.Distance(transform.position, pacingTo);
        state = BossState.pacing;
        transform.LookAt(pacingTo);
        fromPlace = transform.position;
        bossAnimator.SetInteger("AnimNum", 9);
    }
	public void switchToAttacking(){

		if (groundSlamCooldownTime >= groundSlamCooldown) {
			state = BossState.groundSlam;
			groundSlamTimeElapsed = 0;
			lastColorChangedTime = Time.time;
			bossAnimator.SetInteger ("AnimNum", 12);
		} else {
			bossAnimator.SetInteger ("AnimNum", 13);
			state = BossState.punching;
		}
	}

	public void createGroundSlamArea(){
		groundSlam = Instantiate(groundSlamPrefab, transform);
		groundSlamMat = groundSlam.GetComponent<SpriteRenderer> ().material;

	}
	public void destroyGroundSlamArea(){
		groundSlamTimeElapsed = 0;
		groundSlamCooldownTime = 0;
		state = BossState.following;
		inGroundSlam = false;
		Destroy (groundSlam);
	}
    public Vector3 choosePointInBossRoom() {
        float xLoc = Random.Range(-0.8f,0.8f);
        float yLoc = Random.Range(-1.2f,-2.8f);
        Debug.Log(" Moving to: " + xLoc + " " + yLoc);
        Vector3 returnVec = new Vector3(xLoc, 0, yLoc);
        return returnVec;
    }
	public void getHit(){
		interruptedAnim = bossAnimator.GetInteger ("AnumNum");
		interruptedState = state;
		state = BossState.getHit;
		bossAnimator.SetInteger ("AnimNum", 14);
	}
	public void resumeSuspendedState(){
		bossAnimator.SetInteger ("AnimNum", interruptedAnim);
		state = interruptedState;
	}
}
