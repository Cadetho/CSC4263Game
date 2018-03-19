using System;
using UnityEngine;

[RequireComponent(typeof (Character))]
[DisallowMultipleComponent]
public class UserControl : MonoBehaviour
{
    private Character player;
    private Vector3 forwardDirection;
    private Vector3 rightDirection;
    private Vector3 moveDirection;
    
    private void Start()
    {
        if (Camera.main != null)
        {
            forwardDirection = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            rightDirection = Camera.main.transform.right;
        }
        else
        {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            forwardDirection = Vector3.forward;
            rightDirection = Vector3.right;
        }
        
        player = GetComponent<Character>();
    }
    
    private void FixedUpdate()
    {
        // read inputs
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        moveDirection = v*forwardDirection + h*rightDirection;
		// walk speed multiplier
	    //if (Input.GetButton("Walk")) moveDirection *= 0.5f;

        // pass all parameters to the character control script
        player.Move(moveDirection);
    }
}