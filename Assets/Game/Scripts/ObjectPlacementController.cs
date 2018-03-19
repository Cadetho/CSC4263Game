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

    public GameObject currentPlaceableObject;//the instance of the object to be placed
    private boardTile currentPlaceableTile;
    public GameObject prefabWall;
    public GameObject prefabDoor;
    public GameManager gm;
    Vector2 boardLen;
    Vector2 center;

    private Quaternion qforward = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    private Quaternion qright = Quaternion.LookRotation(Vector3.right, Vector3.up);
    private Quaternion qback = Quaternion.LookRotation(Vector3.back, Vector3.up);
    private Quaternion qleft = Quaternion.LookRotation(Vector3.left, Vector3.up);
    /* 
     * This method will create the gameBoard plane as soon as the objectPlacementController is instantiated
     */
    float wallPad = 0.45f;
    public Vector3 mousePos;
    public Ray castPoint;
    public RaycastHit hit;
    GameObject gameBoard;
    bool boardReady = false;

    private void Start() {
        gameBoard = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Vector3 temp = new Vector3(1, 1, Camera.main.nearClipPlane);
        float distance = Camera.main.ViewportToWorldPoint(temp).y;
        int endX = (int)(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance)).x * 1.2);
        int startX = (int)(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x * 1.2);
        int startY = (int)(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).z * 1.2);
        int endY = (int)(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance)).z * 1.2);
        boardLen.x = endX - startX;
        boardLen.y = endX - startX;
        center = new Vector2(boardLen.x / 2, boardLen.y / 2);
        gameBoard.transform.localScale = new Vector3(boardLen.x, 1f, boardLen.y);
        gameBoard.GetComponent<MeshRenderer>().enabled = false;
        gm.placeDefaultTile();
    }


    private void Update () {

        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)//this if statement handles all of the object movement methods
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            MoveCurrentObject();
            RotateCurrentObject();
            PlaceIfClick();
        }
	}
    public GameObject createGridCard(boardTile newTile){
        //Vector2 gridPosition = new Vector2(center.x + x, center.y + y);
        GameObject newGridCard = new GameObject();
        newGridCard.transform.SetParent(gameBoard.transform);
        //newGridCard.transform.position = new Vector3(center.x + x, 1.4f, center.y + y);
        for(int i = 0; i < newTile.yLen; i++) {
            for(int j = 0; j < newTile.xLen; j++) {
                if (!newTile.squares[i, j].isEmpty()) {
                    GameObject newRoom = createGridRoom(newTile.squares[i, j],j,  -i, newGridCard);
                    //newRoom.transform.SetParent(newGridCard.transform);
                    newRoom.tag = "Room";
                }
            }
        }
        return newGridCard;
    }

    public void hideBoard() {
        gameBoard.SetActive(false);
        Camera.main.GetComponent<cameraController>().hideGrid();
    }
    public void showBoard() {
        gameBoard.SetActive(true);
    }
    public void setGridCardSpot(int x, int y, boardTile tile, GameObject card) {
        Vector2 gridPosition = new Vector2(center.x + x, center.y + y);
        card.transform.position = new Vector3(x, 0, y);
        //int childNum = 0;
        //for(int i = 0; i<tile.yLen;i++) {
        //    for (int j = 0; j < tile.xLen; j++) {
        //        if (!tile.squares[i, j].isEmpty()) {
        //            placeRoomToGrid(gridPosition.x + j, gridPosition.y - i, card.transform.GetChild(childNum));
        //            childNum++;
        //        }
        //    }
        //}
    }

    public void placeRoomToGrid(float x, float y, Transform roomTransform) {
        roomTransform.position = new Vector3(x, 0.1f, y);
    }
    public GameObject createGridRoom(boardTile.tileSquare room, float x, float y, GameObject parent) {
        GameObject roomObj = new GameObject();
        roomObj.transform.SetParent(parent.transform);
        roomObj.tag = "Room";
        //roomObj.transform.position = new Vector3(x, 1.4f, y);
        if (!room.isEmpty()) {
            GameObject top;
            GameObject right;
            GameObject bottom;
            GameObject left;

            if (room.getTop() != boardTile.Wall.noWall) {
                if(room.getTop() == boardTile.Wall.door) {
                    top = Instantiate(prefabDoor);
                } else {
                    top = Instantiate(prefabWall);
                }
                top.transform.SetParent(roomObj.transform);
                top.transform.position = new Vector3(x, 0, y + wallPad);
            }
            if (room.getRight() != boardTile.Wall.noWall) {
                if (room.getRight() == boardTile.Wall.door) {
                    right = Instantiate(prefabDoor);
                } else {
                    right = Instantiate(prefabWall);
                }
                right.transform.SetParent(roomObj.transform);
                right.transform.SetPositionAndRotation(new Vector3(x+wallPad, 0, y), qright);
            }
            if (room.getBottom() != boardTile.Wall.noWall) {
                if (room.getBottom() == boardTile.Wall.door) {
                    bottom = Instantiate(prefabDoor);
                } else {
                    bottom = Instantiate(prefabWall);
                }
                bottom.transform.SetParent(roomObj.transform);
                bottom.transform.position = new Vector3(x, 0, y - wallPad);
            }
            if (room.getLeft() != boardTile.Wall.noWall) {
                if (room.getLeft() == boardTile.Wall.door) {
                    left = Instantiate(prefabDoor);
                } else {
                    left = Instantiate(prefabWall);
                }
                left.transform.SetParent(roomObj.transform);
                left.transform.SetPositionAndRotation(new Vector3(x-wallPad, 0, y), qleft);
            }
          
        }
        return gameObject;
    }
    /* 
     * This method will place the placeable object in the scene if the player clicks the left mouse button
     */
    private void PlaceIfClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentPlaceableTile != null) {
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
                    int checkSpotX = (int)Mathf.Floor(hit.point.x+ 0.5f);
                    int checkSpotY = (int)Mathf.Floor(hit.point.z + 0.5f);
                    if (gm.checkBoardSpot(checkSpotX, checkSpotY, currentPlaceableTile)) {
                        setGridCardSpot(checkSpotX, checkSpotY, currentPlaceableTile, currentPlaceableObject);
                        //Destroy(currentPlaceableObject);
                        gm.placeCard(checkSpotX, checkSpotY, currentPlaceableTile);
                        currentPlaceableTile = null;
                        currentPlaceableObject = null;
                    }
                }
            }
            //ends control of the currentPlaceableObject, leaving it in its current location
            rotation = 0;
        }
    }   
    public void selectedCard(GameObject card, boardTile tile) {
        currentPlaceableObject = card;
        currentPlaceableTile = tile;
    }
    /* 
     * This method will allow the player to rotate the placeable object with the left and right arrows
     */
    private void RotateCurrentObject()
    {
        if (Input.GetKeyDown(rotateRight))
        {
            rotation += rotationConstant;
            currentPlaceableTile.rotateTile(true);
        }
        else if (Input.GetKeyDown(rotateLeft))
        {
            rotation -= rotationConstant;
            currentPlaceableTile.rotateTile(false);
        }

        currentPlaceableObject.transform.Rotate(Vector3.up, rotation);
    }

    /* 
     * This method will allow the player to move the placeable object with the mouse
     */
    Vector3 moveVector;
    private void MoveCurrentObject()
    {
        RaycastHit hit;
        if(Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
            moveVector = new Vector3(hit.point.x, 0.1f, hit.point.z);
            currentPlaceableObject.transform.position = moveVector;
        }

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitInfo;
        //if(Physics.Raycast(ray, out hitInfo))
        //{
        //    currentPlaceableObject.transform.position = new Vector3(Mathf.Round(hitInfo.point.x), Mathf.Round(hitInfo.point.y), Mathf.Round(hitInfo.point.z));//Mathf.Round is used to make objects snap to the grid
        //    currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        //}
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
