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
    public List<Sprite> keywordImages;

    GameObject keywordDisplay;

    private void Awake()
    {
        keywordDisplay = transform.Find("Keywords").gameObject;

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
        if (!FindAndSetCardImage(card)) return;

        transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;

        transform.Find("Power Background").gameObject.GetComponent<Image>().enabled = true;
        transform.Find("Health Background").gameObject.GetComponent<Image>().enabled = true;
        transform.Find("Cost Background").gameObject.GetComponent<Image>().enabled = true;
        keywordDisplay.transform.Find("Keywords Background").gameObject.GetComponent<Image>().enabled = true;
        transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().text = currentCard.cost.ToString();

        for (int i = 1; i < 8; i++)
        {
            keywordDisplay.transform.Find("Keyword " + i).gameObject.GetComponent<Image>().enabled = true;
        }

        if (currentCard is UnitCard)
        {
            UnitCard currentUnitCard = (UnitCard)currentCard;
            transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().text = currentUnitCard.power.ToString();
            transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().text = currentUnitCard.health.ToString();

            for (int i = 0; i < currentUnitCard.keywords.Count; i++)
            {
                string keywordString = currentUnitCard.keywords[i].ToString();
                foreach (Sprite keywordSprite in keywordImages)
                {
                    if (keywordSprite.name == keywordString)
                    {
                        keywordDisplay.transform.Find("Keyword " + (i + 1)).gameObject.GetComponent<Image>().sprite = keywordSprite;
                    }
                }
            }

            for (int i = currentUnitCard.keywords.Count; i < 7; i++)
            {
                keywordDisplay.transform.Find("Keyword " + (i + 1)).gameObject.GetComponent<Image>().sprite = nullImage;
            }

            //change number color
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
        if (!FindAndSetCardImage(card)) return;

        transform.Find("Cost Background").gameObject.GetComponent<Image>().enabled = true;
        transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().text = currentCard.cost.ToString();
    }

    private bool FindAndSetCardImage(Card card)
    {
        if (card == null)
        {

            currentCard = null;
            cardImage.sprite = nullImage;
            transform.Find("Power Background").gameObject.GetComponent<Image>().enabled = false;
            transform.Find("Health Background").gameObject.GetComponent<Image>().enabled = false;
            transform.Find("Cost Background").gameObject.GetComponent<Image>().enabled = false;
            keywordDisplay.transform.Find("Keywords Background").gameObject.GetComponent<Image>().enabled = false;
            transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;

            transform.Find("Unit Power Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("Unit Health Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            for (int i = 1; i < 8; i++)
            {
                keywordDisplay.transform.Find("Keyword " + i).gameObject.GetComponent<Image>().sprite = nullImage;
                keywordDisplay.transform.Find("Keyword " + i).gameObject.GetComponent<Image>().enabled = false;
            }
            cardImage.enabled = false;
            return false;
        }

        transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;

        if (card is UnitCard)
        {
            currentCard = UnitCard.CopyCard((UnitCard)card);
        }
        else if (card is SpellCard)
        {
            currentCard = SpellCard.CopyCard((SpellCard)card);
        }

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

        return true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RenderUnit(null);
        }
    }
}
