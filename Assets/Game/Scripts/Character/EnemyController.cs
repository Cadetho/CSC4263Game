using UnityEngine;
using System.Collections;
using UnityEditor.Events;


//public enum Weapon
//{
//	UNARMED = 0,
//	RELAX = 8
//}

[SelectionBase]
[RequireComponent(typeof(CharacterStats))]
[DisallowMultipleComponent]
public class EnemyController : MonoBehaviour
{
    #region Fields

    // Components
    Rigidbody rb;
    protected Animator animator;
    public GameObject target;
    [HideInInspector]
    public Vector3 targetDashDirection;
    // Navmesh variables
    public bool useNavMesh = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private float navMeshSpeed;
    public Transform goal;

    // movement variables
    [Header("Movement")]
    public float walkSpeed = 1.35f;
    float moveSpeed;
    public float runSpeed = 6f;
    float rotationSpeed = 40f;
    Vector3 inputVec;
    Vector3 newVelocity;
    [HideInInspector]
    public bool canMove = true;

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

    [Header("Attack")]
    public GameObject rightFist;
    public GameObject leftFist;
    private MeleeWeapon rightPunch;
    private MeleeWeapon leftPunch;
    [Range(0f, 0.5f)]
    public float attackDuration = 0.4f;
    [Range(0f, 0.5f)]
    public float attackDelay = 0.1f;

    [HideInInspector]
    public CharacterStats stats;

    // input variables
    [HideInInspector]
    public float inputHorizontal = 0f;
    [HideInInspector]
    public float inputVertical = 0f;
    [HideInInspector]
    public float inputBlock = 0f;
    [HideInInspector]
    public bool inputLightHit;
    [HideInInspector]
    public bool inputDeath;
    [HideInInspector]
    public bool inputAttackR;
    [HideInInspector]
    public bool inputAttackL;

    #endregion

    #region Initialization


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
    }

    #endregion

    #region Updates


    void Update()
    {
        // make sure there is an animator on this character

        if (animator)
        {
            //if (canMove && !isDead && !useNavMesh)
            //{
            //    CameraRelativeMovement();
            //}

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

    //void CameraRelativeMovement()
    //{
    //    // converts control input vectors into camera facing vectors
    //    Transform cameraTransform = sceneCamera.transform;
    //    // Forward vector relative to the camera along the x-z plane   
    //    Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
    //    forward.y = 0;
    //    forward = forward.normalized;
    //    // Right vector relative to the camera always orthogonal to the forward vector
    //    Vector3 right = new Vector3(forward.z, 0, -forward.x);

    //    inputVec = inputHorizontal * right + inputVertical * forward;
    //}

    // rotate character towards direction moved

    void RotateTowardsMovementDir()
    {

        if (inputVec != Vector3.zero && !isStrafing)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
        }
    }

    float UpdateMovement()
    {

        //if (!useNavMesh)
        //{
        //    CameraRelativeMovement();
        //}
        inputVec = inputHorizontal * Vector3.right + inputVertical * Vector3.forward;
        Vector3 motion = inputVec.normalized;
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

    #region CombatMethods

    //0 = No side
    //1 = Left
    //2 = Right
    //3 = Dual

    public void Attack(int attackSide)
    {
        if (canAction)
        {
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
        }
    }

    public void GetHit()
    {
        // Maybe add a random chance of flinching
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

        while (isKnockback)
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