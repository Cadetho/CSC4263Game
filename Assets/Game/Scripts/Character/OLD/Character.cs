using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Character : MonoBehaviour
{
    [Header("Stats")]
    public int BaseHealth = 100;
    [Header("Movement")]
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField][Range(0.5f, 2f)] float m_AnimSpeedMultiplier = 1f;

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;


	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}


	public void Move(Vector3 move)
	{

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
		move = transform.InverseTransformDirection(move);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();

		// send input and other state parameters to the animator
		UpdateAnimator(move);
	}

	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);

		m_Animator.speed = move.magnitude > 0 ? m_AnimSpeedMultiplier : 1;
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}


    //public void OnAnimatorMove()
    //{
    //    // we implement this function to override the default root motion.
    //    // this allows us to modify the positional speed before it's applied.
    //    if (Time.deltaTime > 0)
    //    {
    //        Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

    //        // we preserve the existing y part of the current velocity.
    //        v.y = m_Rigidbody.velocity.y;
    //        m_Rigidbody.velocity = v;
    //    }
    //}
}