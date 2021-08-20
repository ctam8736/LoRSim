using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardRenderer : MonoBehaviour, IPointerClickHandler
{
    public CardData cardData;
    public Sprite nullImage;
    protected Image cardImage;
    protected Card currentCard = null;

    private void Awake()
    {
        cardImage = gameObject.GetComponent<Image>();
        RenderUnit(null);
    }

    // Update is called once per frame
    public void RenderUnit(UnitCard card)
    {
        if (card == null)
        {
            currentCard = null;
            cardImage.sprite = nullImage;
            return;
        }

        currentCard = UnitCard.CopyCard(card);
        Sprite cardSprite = null;
        foreach (Sprite sprite in cardData.cardImages)
        {
            if (sprite.name.Equals(cardData.imageDictionary[currentCard.name]))
            {
                cardSprite = sprite;
            }
        }

        cardImage.sprite = cardSprite;
    }

    public void RenderSpell(SpellCard card)
    {
        if (card == null)
        {
            currentCard = null;
            cardImage.sprite = nullImage;
            return;
        }

        currentCard = SpellCard.CopyCard(card);
        Sprite cardSprite = null;
        foreach (Sprite sprite in cardData.cardImages)
        {
            if (sprite.name.Equals(cardData.imageDictionary[currentCard.name]))
            {
                cardSprite = sprite;
            }
        }

        cardImage.sprite = cardSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !DemoImageDisplay._instance.cardImage.enabled)
        {
            if (currentCard is UnitCard)
            {
                DemoImageDisplay._instance.RenderUnit((UnitCard)currentCard);
            }
            else if (currentCard is SpellCard)
            {
                DemoImageDisplay._instance.RenderSpell((SpellCard)currentCard);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse is over " + gameObject.name);
    }
}
