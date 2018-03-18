using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardController : MonoBehaviour {
    public boardTile tile;
    playerHand parent;
    Material shader;
    bool selected;
    float emission;
    Color baseColor = Color.blue;
    Color finalColor;
    RectTransform roomPanel;
    GameObject[,] rooms;
    public GameObject cardDoorPrefab;
    public GameObject cardWallPrefab;
    public GameManager gm;
    Vector2 topWallMax = new Vector2(1, 1);
    Vector2 topWallMin = new Vector2(0, 0.8f);

    Vector2 rightWallMax = new Vector2(1, 1);
    Vector2 rightWallMin = new Vector2(0.8f, 0);

    Vector2 bottomWallMax = new Vector2(1, 0.2f);
    Vector2 bottomWallMin = Vector2.zero;

    Vector2 leftWallMax = new Vector2(0.2f, 1);
    Vector2 leftWallMin = Vector2.zero;



    public void setParent(playerHand _parent) {
        parent = _parent;
    }
    public void setTile(boardTile newTile) {
        tile = newTile;
        roomPanel = this.gameObject.transform.Find("RoomPanel").GetComponent<RectTransform>();
        roomPanel.anchorMax = new Vector2(0.9f, .9f);
        roomPanel.anchorMin = new Vector2(0.1f, 0.4f);
        float xLen = tile.xLen;
        float yLen = tile.yLen;
        rooms = new GameObject[(int)yLen, (int)xLen];
        float scaling = Mathf.Max(xLen, yLen);
        for(int i = 0; i < yLen; i++) {
            for(int j = 0; j < xLen; j++) {
                if (!tile.squares[i, j].isEmpty()) {
                    displayRoomOnCard(tile.squares[i, j], new Vector2((float)((j+1)/xLen), (float)(1 -i/yLen)), new Vector2((float)(j /xLen), (float)(1 -(i+1)/yLen)));
                }
            }
        }
    }
    void displayRoomOnCard(boardTile.tileSquare room, Vector2 anchorMax, Vector2 anchorMin) {
        GameObject newRoom = new GameObject();
        RectTransform newRect = newRoom.AddComponent<RectTransform>();
        newRect.SetParent(roomPanel);
        newRect.anchorMax = anchorMax;
        newRect.anchorMin = anchorMin;
        newRect.offsetMax = Vector2.zero;
        newRect.offsetMin = Vector2.zero;

        GameObject top;
        GameObject right;
        GameObject bottom;
        GameObject left;
        if (room.getTop() == boardTile.Wall.door) {
            top = Instantiate(cardDoorPrefab);
        } else if (room.getTop() == boardTile.Wall.fullWall) {
            top = Instantiate(cardWallPrefab);
        } else {
            top = null;
        }
        if (room.getRight() == boardTile.Wall.door) {
            right = Instantiate(cardDoorPrefab);
        } else if (room.getRight() == boardTile.Wall.fullWall) {
            right = Instantiate(cardWallPrefab);
        } else {
            right = null;
        }
        if (room.getBottom() == boardTile.Wall.door) {
            bottom = Instantiate(cardDoorPrefab);
        } else if (room.getBottom() == boardTile.Wall.fullWall) {
            bottom = Instantiate(cardWallPrefab);
        } else {
            bottom = null;
        }
        if (room.getLeft() == boardTile.Wall.door) {
            left = Instantiate(cardDoorPrefab);
        } else if(room.getLeft() == boardTile.Wall.fullWall){
            left = Instantiate(cardWallPrefab);
        } else {
            left = null;
        }

        if (!(top == null)) {
            top.transform.SetParent(newRoom.transform);
            RectTransform topRect = top.GetComponent<RectTransform>();
            topRect.anchorMax = topWallMax;
            topRect.anchorMin = topWallMin;
            topRect.offsetMax = Vector2.zero;
            topRect.offsetMin = Vector2.zero;
        }
       if(!(right == null)) {
            right.transform.SetParent(newRoom.transform);
            RectTransform rightRect = right.GetComponent<RectTransform>();
            if (room.getRight() == boardTile.Wall.door) {
                swapAnchors(rightRect.GetChild(0).GetComponent<RectTransform>());
                swapAnchors(rightRect.GetChild(1).GetComponent<RectTransform>());
            }
            rightRect.anchorMax = rightWallMax;
            rightRect.anchorMin = rightWallMin;
            rightRect.offsetMax = Vector2.zero;
            rightRect.offsetMin = Vector2.zero;
        }
      if(!(bottom == null)) {
            bottom.transform.SetParent(newRoom.transform);
            RectTransform bottomRect = bottom.GetComponent<RectTransform>();
            bottomRect.anchorMax = bottomWallMax;
            bottomRect.anchorMin = bottomWallMin;
            bottomRect.offsetMax = Vector2.zero;
            bottomRect.offsetMin = Vector2.zero;
        }
      
      if(!(left == null)) {
            left.transform.SetParent(newRoom.transform);
            RectTransform leftRect = left.GetComponent<RectTransform>();
            if (room.getLeft() == boardTile.Wall.door) {
                swapAnchors(leftRect.GetChild(0).GetComponent<RectTransform>());
                swapAnchors(leftRect.GetChild(1).GetComponent<RectTransform>());
            }
            leftRect.anchorMax = leftWallMax;
            leftRect.anchorMin = leftWallMin;
            leftRect.offsetMax = Vector2.zero;
            leftRect.offsetMin = Vector2.zero;
        }  
    }
    void Start() {
        Image renderer = GetComponent<Image>();
        shader = renderer.material;
        selected = false;

    }
    void swapAnchors(RectTransform toSwap) {
        toSwap.anchorMax = new Vector2(toSwap.anchorMax.y, toSwap.anchorMax.x);
        toSwap.anchorMin = new Vector2(toSwap.anchorMin.y, toSwap.anchorMin.x);
    }
    void Update() {
        if (selected) {
            emission = Mathf.PingPong(Time.time, 1.0f);
            finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
            shader.SetColor("_EmissionColor", finalColor);
        }
    }
    public void onClick() {
        parent.cardClicked(this);
    }
    public void unselect() {
        shader.DisableKeyword("_Emission");
        selected = false;
    }
    public void select() {
        //gm.selectCard(this);
        shader.EnableKeyword("_Emission");
        selected = true;
    }
}
