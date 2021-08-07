using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DemoImageDisplay : MonoBehaviour
{
    public CardData cardData;
    public Sprite nullImage;
    public Image cardImage;
    protected Card currentCard = null;
    public static DemoImageDisplay _instance;

    private void Awake()
    {
        cardImage = gameObject.GetComponent<Image>();

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        currentCard = null;
        RenderUnit(null);
    }

    // Update is called once per frame
    public void RenderUnit(UnitCard card)
    {
        if (card == null)
        {
            currentCard = null;
            cardImage.sprite = nullImage;
            transform.Find("Power Background").gameObject.GetComponent<Image>().enabled = false;
            transform.Find("Health Background").gameObject.GetComponent<Image>().enabled = false;
            transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            cardImage.enabled = false;
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
        transform.Find("Power Background").gameObject.GetComponent<Image>().enabled = true;
        transform.Find("Health Background").gameObject.GetComponent<Image>().enabled = true;
        if (currentCard is UnitCard)
        {
            transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().text = ((UnitCard)currentCard).power.ToString();
            transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().text = ((UnitCard)currentCard).health.ToString();

            if (((UnitCard)currentCard).IsDamaged())
            {
                transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(137, 0, 14, 255);
            }
            else
            {
                transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
            }
        }
        cardImage.enabled = true;
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
        cardImage.enabled = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RenderUnit(null);
        }
    }
}
