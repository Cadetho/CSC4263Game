using UnityEngine;
using System.Collections;
using UnityEditor.Events;


//public enum Weapon
//{
//	UNARMED = 0,
//	RELAX = 8
//}

[SelectionBase]
[RequireComponent(typeof (CharacterStats))]
[DisallowMultipleComponent]
public class CharacterController : MonoBehaviour
{
	#region Fields

	// Components
	Rigidbody rb;
	protected Animator animator;
	public GameObject target;
	[HideInInspector]
	public Vector3 targetDashDirection;
	public Camera sceneCamera;
    // Navmesh variables
    [Space]
	public GameManager gm;
    public bool useNavMesh = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private float navMeshSpeed;
    public Transform goal;

	// movement variables
    [Header("Movement")]
	public float walkSpeed = .27f;
	float moveSpeed;
	public float runSpeed = 1.2f;
	float rotationSpeed = 40f;
	Vector3 inputVec;
	Vector3 newVelocity;
    [HideInInspector]
	public bool canMove = true;

    [Space]
    // rolling variables
    public float rollSpeed = 8;
	bool isRolling = false;
    bool canRoll = true;
	public float rollDuration;
    public float rollCooldown;

    //// Weapon and Shield
    //[HideInInspector]
    //public Weapon weapon;
    //int rightWeapon = 0;
    //int leftWeapon = 0;
    [HideInInspector]
    public bool isRelax = false;

    // isStrafing/action variables
    [HideInInspector]
	public bool canAction = true;
	[HideInInspector]
	public bool isStrafing = false;
	[HideInInspector]
	public bool isDead = false;
	public float knockbackMultiplier = 1f;
	bool isKnockback;

    [Header("Combat")]
    public GameObject rightFist;
    public GameObject leftFist;
    private MeleeWeapon rightPunch;
    private MeleeWeapon leftPunch;
    [Range(0f, 0.5f)]
    public float attackDuration=0.4f;
    [Range(0f, 0.5f)]
    public float attackDelay=0.1f;
    public float lockOnRadius = 1f;
    [Range(0f, 90f)]
    public float lockOnAngle = 45f;
    public LayerMask combatantMask;

    [HideInInspector]
    public CharacterStats stats;

    // input variables
    float inputHorizontal = 0f;
	float inputVertical = 0f;
	float inputDashVertical = 0f;
	float inputDashHorizontal = 0f;
	float inputBlock = 0f;
	bool inputLightHit;
	bool inputDeath;
	bool inputAttackR;
	bool inputAttackL;
	bool inputCastL;
	bool inputCastR;
	bool inputJump;

	#endregion

	#region Initialization and Inputs


	void Start()
	{
		// set the animator component
		animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
        leftPunch = leftFist.GetComponent<MeleeWeapon>();
        rightPunch = leftFist.GetComponent<MeleeWeapon>();
        stats = GetComponent<CharacterStats>();
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();
    }

    void Inputs()
    {
        inputDashHorizontal = Input.GetAxisRaw("DashHorizontal");
        inputDashVertical = Input.GetAxisRaw("DashVertical");
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        inputLightHit = Input.GetButtonDown("LightHit");
        inputDeath = Input.GetButtonDown("Death");
        inputAttackL = Input.GetButtonDown("AttackL");
        inputAttackR = Input.GetButtonDown("AttackR");
        inputCastL = Input.GetButtonDown("CastL");
        inputCastR = Input.GetButtonDown("CastR");
        inputBlock = Input.GetAxisRaw("TargetBlock");
        inputJump = Input.GetButtonDown("Jump");
    }

	#endregion

	#region Updates


	void Update()
	{
		// make sure there is an animator on this character
		if (transform.position.z < -1 && transform.position.z > -3 && transform.position.x > -1 && transform.position.x < 1) {
			gm.inBossRoom ();
		}
		if (animator)
		{
			Inputs();

			if (canMove && !isDead && !useNavMesh)
			{
				CameraRelativeMovement();
			} 
			Rolling();

			if (inputLightHit && canAction)
			{
				GetHit();
			}

			if (inputDeath && canAction)
			{

				if (!isDead)
				{
					StartCoroutine(_Death());
				}

				else
				{
					StartCoroutine(_Revive());
				}
			}

			if (inputAttackL && canAction)
			{
				Attack(1);
			}

			if (inputAttackR && canAction)
			{
				Attack(2);
			}

			//if (inputCastL && canAction && !isStrafing)
			//{
			//	AttackKick(1);
			//}

			//if (inputCastR && canAction && !isStrafing)
			//{
			//	AttackKick(2);
			//}

			//// if strafing
			//if ((Input.GetKey(KeyCode.LeftShift) || inputBlock > 0.1f) && canAction)
			//{  
			//	isStrafing = true;
			//	animator.SetBool("Strafing", true);

			//	if (inputCastL && canAction)
			//	{
			//		CastAttack(1);
			//	}

			//	if (inputCastR && canAction)
			//	{
			//		CastAttack(2);
			//	}
			//}

			//else
			//{
			//	isStrafing = false;
			//	animator.SetBool("Strafing", false);
			//}
            // Navmesh
            if (Input.GetMouseButtonDown(0))
            {
                if (useNavMesh)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        agent.destination = hit.point;
                    }
                }
            }
        }
		else
		{
			Debug.Log("ERROR: There is no animator for character.");
		}

        if (useNavMesh)
        {
            agent.enabled = true;
            navMeshSpeed = agent.velocity.magnitude;
        }
        else
        {
            if (agent != null)
            {
                agent.enabled = false;
            }
        }

        #region Time Manipulation
        // Slow time
        //if (Input.GetKeyDown(KeyCode.T))
        //{

        //	if (Time.timeScale != 1)
        //	{
        //		Time.timeScale = 1;
        //	}

        //	else
        //	{
        //		Time.timeScale = 0.15f;
        //	}
        //}

        //// Pause
        //if (Input.GetKeyDown(KeyCode.P))
        //{

        //	if (Time.timeScale != 1)
        //	{
        //		Time.timeScale = 1;
        //	}

        //	else
        //	{
        //		Time.timeScale = 0f;
        //	}
        //}
        #endregion
    }

    void FixedUpdate()
    {
        if (canMove && !isDead)
        {
            moveSpeed = UpdateMovement();
        }
    }

    // get velocity of rigid body and pass the value to the animator to control the animations
    void LateUpdate()
    {
        if (!useNavMesh)
        {
            // Get local velocity of character
            float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
			float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
			// Update animator with movement values
			animator.SetFloat("Velocity X", velocityXel / runSpeed);
			animator.SetFloat("Velocity Z", velocityZel / runSpeed);
			// if character is alive and can move, set our animator

			if (!isDead && canMove)
			{

				if (moveSpeed > 0)
				{
					animator.SetBool("Moving", true);
				}

				else
				{
					animator.SetBool("Moving", false);
				}
            }
        }

		else
		{
			animator.SetFloat("Velocity X", agent.velocity.sqrMagnitude);
			animator.SetFloat("Velocity Z", agent.velocity.sqrMagnitude);

			if (navMeshSpeed > 0)
			{
				animator.SetBool("Moving", true);
			}

			else
			{
				animator.SetBool("Moving", false);
			}
		}
	}

	#endregion

	#region UpdateMovement

	void CameraRelativeMovement()
    {
		// converts control input vectors into camera facing vectors
		Transform cameraTransform = sceneCamera.transform;
		// Forward vector relative to the camera along the x-z plane   
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		// Right vector relative to the camera always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		if (!isRolling)
		{
			targetDashDirection = inputDashHorizontal * right + inputDashVertical * -forward;
		}
		inputVec = inputHorizontal * right + inputVertical * forward;
	}

	// rotate character towards direction moved

	void RotateTowardsMovementDir()
	{

		if (inputVec != Vector3.zero && !isStrafing && !isRolling)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
		}
	}

	float UpdateMovement()
    {

        if (!useNavMesh)
        {
            CameraRelativeMovement();
        }
        Vector3 motion = inputVec;
		// reduce input for diagonal movement
		if (motion.magnitude > 1)
		{
			motion.Normalize();
		}
		if (canMove)
		{
			// set speed by walking / running
			if (isStrafing)
			{
				newVelocity = motion * walkSpeed;
			}

			else
			{
				newVelocity = motion * runSpeed;
			}
			// if rolling use rolling speed and direction
			if (isRolling)
			{
				// force the dash movement to 1
				targetDashDirection.Normalize();
				newVelocity = rollSpeed * targetDashDirection;
			}
		}

		if (!isStrafing || !canMove)
		{
			RotateTowardsMovementDir();
		}

		if (isStrafing && !isRelax)
		{
			// make character point at target
			Quaternion targetRotation;
			Vector3 targetPos = target.transform.position;
			targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
		}
		newVelocity.y = rb.velocity.y;
		rb.velocity = newVelocity;
		// return a movement value for the animator
		return inputVec.magnitude;
	}

    #endregion

    #region MiscMethods

    //0 = No side
    //1 = Left
    //2 = Right
    //3 = Dual

    public void Attack(int attackSide)
    {
        if (canAction)
        {
            // Lock on to nearby enemy
            // We can move this to a separate method
            Collider[] combatants = Physics.OverlapSphere(transform.position, lockOnRadius, combatantMask);
            if (combatants.Length > 0)
            {
                foreach (Collider combatant in combatants)
                {
                    Vector3 playerToCombatant = combatant.transform.position - transform.position;
                    playerToCombatant.y = 0f;
                    Quaternion rotationToCombatant = Quaternion.LookRotation(playerToCombatant);
                    if (Quaternion.Angle(transform.rotation, rotationToCombatant) < lockOnAngle)
                    {
                        transform.rotation = rotationToCombatant;
                        Debug.Log(gameObject.name + ": Locked onto " + combatant.name);
                        break;
                    }
                }
            }

            if (attackSide == 1)
            {
                // Left Punch
                animator.SetTrigger("Attack" + (3).ToString() + "Trigger");
                leftPunch.Attack(stats.attackPower, attackDuration, attackDelay);
            }
            else
            {
                // Right Punch
                animator.SetTrigger("Attack" + (6).ToString() + "Trigger");
                leftPunch.Attack(stats.attackPower, attackDuration, attackDelay);
            }
            

            StartCoroutine(_LockMovementAndAttack(0, .6f));

            //if (weapon == Weapon.UNARMED)
            //{
            //int maxAttacks = 3;
            //int attackNumber = 0;

            //if (attackSide == 1 || attackSide == 3)
            //{
            //    attackNumber = Random.Range(3, maxAttacks); // Left Hook
            //}
            //else if (attackSide == 2)
            //{
            //    attackNumber = Random.Range(6, maxAttacks + 3); // Right Hook
            //}
            //if (attackSide != 3)
            //{
            //    animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");

            //    StartCoroutine(_LockMovementAndAttack(0, .6f));
            //}
            //else
            //{
            //    // 2 Hand Attack
            //    animator.SetTrigger("AttackDual" + (attackNumber).ToString() + "Trigger");
            //    StartCoroutine(_LockMovementAndAttack(0, .75f));
            //}
            //}
            //else
            //{
            //    //  Armed Attack
            //    animator.SetTrigger("Attack" + (6).ToString() + "Trigger");
            //    StartCoroutine(_LockMovementAndAttack(0, .85f));
            //}
        }
    }


    //public void AttackKick(int kickSide)
    //{
    //    if (kickSide == 1)
    //    {
    //        animator.SetTrigger("AttackKick1Trigger"); // Left Foot
    //    }
    //    else
    //    {
    //        animator.SetTrigger("AttackKick2Trigger"); // Right Foot
    //    }
    //    StartCoroutine(_LockMovementAndAttack(0, .8f));
    //}

    //0 = No side
    //1 = Left
    //2 = Right
    //3 = Dual

    //public void CastAttack(int attackSide)
    //{

    //    //if (weapon == Weapon.UNARMED)
    //    //{
    //    int maxAttacks = 3;

    //    if (attackSide == 1)
    //    {
    //        int attackNumber = Random.Range(0, maxAttacks);

    //        animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
    //        StartCoroutine(_LockMovementAndAttack(0, .8f));
    //    }

    //    if (attackSide == 2)
    //    {
    //        int attackNumber = Random.Range(3, maxAttacks + 3);

    //        animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
    //        StartCoroutine(_LockMovementAndAttack(0, .8f));
    //    }

    //    if (attackSide == 3)
    //    {
    //        int attackNumber = Random.Range(0, maxAttacks);

    //        animator.SetTrigger("CastDualAttack" + (attackNumber + 1).ToString() + "Trigger");
    //        StartCoroutine(_LockMovementAndAttack(0, 1f));
    //    }
    //    //} 
    //}


    public void GetHit()
	{
		int hits = 5;
		int hitNumber = Random.Range(0, hits);
		animator.SetTrigger("GetHit" + (hitNumber + 1).ToString() + "Trigger");
		StartCoroutine(_LockMovementAndAttack(.1f, .4f));
		//apply directional knockback force

		if (hitNumber <= 1)
		{
			StartCoroutine(_Knockback(-transform.forward, 8, 4));
		}

		else if (hitNumber == 2)
		{
			StartCoroutine(_Knockback(transform.forward, 8, 4));
		}

		else if (hitNumber == 3)
		{
			StartCoroutine(_Knockback(transform.right, 8, 4));
		}

		else if (hitNumber == 4)
		{
			StartCoroutine(_Knockback(-transform.right, 8, 4));
		}
	}


	IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount)
	{
		isKnockback = true;
		StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
		yield return new WaitForSeconds(.1f);
		isKnockback = false;
	}


	IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount)
	{

		while(isKnockback)
		{
			rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
			yield return null;
		}
	}


	public IEnumerator _Death()
	{
		animator.SetTrigger("Death1Trigger");
		StartCoroutine(_LockMovementAndAttack(.1f, 1.5f));
		isDead = true;
		animator.SetBool("Moving", false);
		inputVec = new Vector3(0, 0, 0);
		yield return null;
	}


	public IEnumerator _Revive()
	{
		animator.SetTrigger("Revive1Trigger");
		isDead = false;
		yield return null;
	}

	//Animation Events

	void Hit()
	{

	}


	void FootL()
	{

	}


	void FootR()
	{

	}


	void Jump()
	{

	}


	void Land()
	{

	}

	#endregion

	#region Rolling


	void Rolling()
	{

		if (!isRolling && canRoll)
		{

			if (Input.GetAxis("DashVertical") > .5 || Input.GetAxis("DashVertical") < -.5 || Input.GetAxis("DashHorizontal") > .5 || Input.GetAxis("DashHorizontal") < -.5)
			{
				StartCoroutine(_DirectionalRoll(Input.GetAxis("DashVertical"), Input.GetAxis("DashHorizontal")));
			}
		}
	}


	public IEnumerator _DirectionalRoll(float x, float v)
	{
		// check which way the dash is pressed relative to the character facing
		float angle = Vector3.Angle(targetDashDirection, -transform.forward);
		float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(targetDashDirection, transform.forward)));
		// angle in [-179,180]
		float signed_angle = angle * sign;
		// angle in 0-360
		float angle360 = (signed_angle + 180) % 360;
		// determine the animation to play based on the angle

		if (angle360 > 315 || angle360 < 45)
		{
			StartCoroutine(_Roll(1));
		}

		if (angle360 > 45 && angle360 < 135)
		{
			StartCoroutine(_Roll(2));
		}

		if (angle360 > 135 && angle360 < 225)
		{
			StartCoroutine(_Roll(3));
		}

		if (angle360 > 225 && angle360 < 315)
		{
			StartCoroutine(_Roll(4));
		}
		yield return null;
	}


	public IEnumerator _Roll(int rollNumber)
	{

		if (rollNumber == 1)
		{
			animator.SetTrigger("RollForwardTrigger");
		}

		if (rollNumber == 2)
		{
			animator.SetTrigger("RollRightTrigger");
		}

		if (rollNumber == 3)
		{
			animator.SetTrigger("RollBackwardTrigger");
		}

		if (rollNumber == 4)
		{
			animator.SetTrigger("RollLeftTrigger");
		}
		isRolling = true;
        canRoll = false;
        yield return new WaitForSeconds(rollDuration);
		isRolling = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

	#endregion

	#region _Coroutines

	// method to keep character from moving while attacking, etc

	public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
	{
		yield return new WaitForSeconds(delayTime);

		canAction = false;
		canMove = false;
		animator.SetBool("Moving", false);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		inputVec = new Vector3(0, 0, 0);
		animator.applyRootMotion = true;

		yield return new WaitForSeconds(lockTime);

		canAction = true;
		canMove = true;
		animator.applyRootMotion = false;
	}

	#endregion
}