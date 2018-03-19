using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHand : MonoBehaviour{
    private readonly int handSize = 7;
    List<boardTile> tiles = new List<boardTile>();
    private int handCount = 0;
    public Transform handPanel;
    public GameObject cardPrefab;
    private float cardNextOffset = 0.04f;
    private float cardOffsetAmount = 0.03f;
    private float cardWidth = 0.11f;

    private cardController currentClickedCard;
    public ObjectPlacementController gridController;
    GameObject selectedCard;

    public bool addCard(boardTile newtile) {
        bool added = false;
        if(tiles.Count < handSize + 1) {
            tiles.Add(newtile);
            added = true;
            showNewCard(newtile);
        }

        return added;
    }

   public void replaceCard(boardTile newTile, GameObject oldCard) {
        currentClickedCard = null;
        GameObject newTileButton = Instantiate(cardPrefab);
        RectTransform newRect = newTileButton.GetComponent<RectTransform>();
        cardController cardC = newTileButton.GetComponent<cardController>();
        cardC.setTile(newTile);
        cardC.setParent(this);
        newRect.SetParent(handPanel);
        newRect.anchorMin = oldCard.GetComponent<RectTransform>().anchorMin;
        newRect.anchorMax = oldCard.GetComponent<RectTransform>().anchorMax;
        newRect.offsetMin = Vector2.zero;
        newRect.offsetMax = Vector2.zero;
        Destroy(oldCard);
    }
    private void showNewCard(boardTile newTile) {
        GameObject newTileButton = Instantiate(cardPrefab);
        RectTransform newRect = newTileButton.GetComponent<RectTransform>();
        cardController cardC = newTileButton.GetComponent<cardController>();
        cardC.setTile(newTile);
        cardC.setParent(this);

        newRect.SetParent(handPanel);
        newRect.anchorMin = new Vector2(cardNextOffset, 0);
        newRect.anchorMax = new Vector2(cardNextOffset + cardWidth, 1);
        newRect.offsetMin = Vector2.zero;
        newRect.offsetMax = Vector2.zero;
        cardNextOffset += cardWidth + cardOffsetAmount;
    }

    public void cardClicked(cardController card, GameObject cardObj) {
        if(currentClickedCard != null && gridController.currentPlaceableObject != null) {
            currentClickedCard.unselect();
            Destroy(selectedCard);
        }
        //Cursor.visible = false;
        currentClickedCard = card;
        currentClickedCard.select();
        selectedCard = gridController.createGridCard(card.tile);
        gridController.selectedCard(selectedCard, card.tile, cardObj);
       // GameManager.selectedCard(currentClickedCard);
    }
}
