using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacementController : MonoBehaviour {

    [SerializeField]
    private GameObject placeableObjectPrefab;//the prefab which will be placed

    [SerializeField]
    private float rotationConstant = 90f;//degrees of rotation per arrow press

    [SerializeField]
    private float boardX = 1f;//width of the board

    [SerializeField]
    private float boardZ = 1f;//length of the board

    private float rotation = 0;
    private KeyCode placeableObjectHotkey = KeyCode.Space;//the key pressed to instantiate the object

    //hotkeys for rotating elements left or right (has been tested to work even if rotating a square has no visible effect)
    private KeyCode rotateRight = KeyCode.RightArrow;
    private KeyCode rotateLeft = KeyCode.LeftArrow;

    private GameObject currentPlaceableObject;//the instance of the object to be placed

    /* 
     * This method will create the gameBoard plane as soon as the objectPlacementController is instantiated
     */
    private void Start()
    {
        GameObject gameBoard = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gameBoard.transform.localScale = new Vector3(boardX, 1f, boardZ);
    }

    private void Update () {

        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)//this if statement handles all of the object movement methods
        {
            MoveCurrentObject();
            RotateCurrentObject();
            PlaceIfClick();
        }
	}

    /* 
     * This method will place the placeable object in the scene if the player clicks the left mouse button
     */
    private void PlaceIfClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentPlaceableObject = null;//ends control of the currentPlaceableObject, leaving it in its current location
            rotation = 0;
        }
    }   

    /* 
     * This method will allow the player to rotate the placeable object with the left and right arrows
     */
    private void RotateCurrentObject()
    {
        if (Input.GetKeyDown(rotateRight))
        {
            rotation += rotationConstant;
        }
        else if (Input.GetKeyDown(rotateLeft))
        {
            rotation -= rotationConstant;
        }

        currentPlaceableObject.transform.Rotate(Vector3.up, rotation);
    }

    /* 
     * This method will allow the player to move the placeable object with the mouse
     */
    private void MoveCurrentObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo))
        {
            currentPlaceableObject.transform.position = new Vector3(Mathf.Round(hitInfo.point.x), Mathf.Round(hitInfo.point.y), Mathf.Round(hitInfo.point.z));//Mathf.Round is used to make objects snap to the grid
            currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
    }

    /* 
     * This method will check to see if the player has pressed the hotkey to begin the placement process
     */
    private void HandleNewObjectHotkey()
    {
        if (Input.GetKeyDown(placeableObjectHotkey))
        {
            if(currentPlaceableObject == null)
            {
                currentPlaceableObject = Instantiate(placeableObjectPrefab);
            }
            else//hitting space again before placing the object will destroy it (mainly for testing purposes)
            {
                Destroy(currentPlaceableObject);
                rotation = 0;
            }
        }
    }
}
