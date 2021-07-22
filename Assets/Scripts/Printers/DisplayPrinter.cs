using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayPrinter : Printer
{
    public List<Sprite> cardImages;
    public Dictionary<string, string> imageDictionary = new Dictionary<string, string>();
    public GameObject boardDisplay;
    public Sprite nullImage;
    public AttackToken attackTokenHandler;
    public ManaGemDisplay playerOneManaGemDisplay;
    public ManaGemDisplay playerTwoManaGemDisplay;
    public ManaGemDisplay playerOneSpellManaDisplay;
    public ManaGemDisplay playerTwoSpellManaDisplay;

    public TextMeshProUGUI playerOneNexusHealthText;
    public TextMeshProUGUI playerTwoNexusHealthText;
    public TextMeshProUGUI playerOneManaGemText;
    public TextMeshProUGUI playerOneSpellManaText;
    public TextMeshProUGUI playerTwoManaGemText;
    public TextMeshProUGUI playerTwoSpellManaText;

    public TextMeshProUGUI roundMessage;

    int currentRoundNumber = 0;

    public void FillImageDictionary()
    {
        imageDictionary.Add("Vanguard Defender", "01DE020");
        imageDictionary.Add("Silverwing Diver", "01DE030");
        imageDictionary.Add("Cithria of Cloudfield", "01DE039");
        imageDictionary.Add("Plucky Poro", "01DE049");
        imageDictionary.Add("Mystic Shot", "01PZ052");
        imageDictionary.Add("Amateur Aeronaut", "01PZ009");
        imageDictionary.Add("Vanguard Lookout", "01DE046");
        imageDictionary.Add("Daring Poro", "01PZ020");
        imageDictionary.Add("Academy Prodigy", "01PZ018");
        imageDictionary.Add("Golden Crushbot", "01PZ059");
        imageDictionary.Add("Radiant Strike", "01DE018");
        //imageDictionary.Add("Chain Vest", "01DE013");
        imageDictionary.Add("Sumpworks Map", "01PZ026");
        imageDictionary.Add("Succession", "01DE047");
        imageDictionary.Add("Dauntless Vanguard", "01DE016");
        imageDictionary.Add("Unlicensed Innovation", "01PZ014");
        imageDictionary.Add("Illegal Contraption", "01PZ014T1");
    }

    // Start is called before the first frame update
    void Start()
    {
        FindTextReferences();

        InitializeDictionary();
        FillImageDictionary();

        playerADeck = LoadDeckFromJson("Assets/Decks/cithria.json");
        playerBDeck = LoadDeckFromJson("Assets/Decks/cithria.json");

        ResetGame(true);
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (showRounds)
        {
            timer += Time.deltaTime;
            if (timer > turnDelay)
            {
                timer = timer % turnDelay;
                HandleGameEnd();
                PlayGameTurn();
                UpdateText();
                //PlotData();
            }
        }
        else
        {
            while (!HandleGameEnd())
            {
                PlayGameTurn();
            }
            UpdateText();
            //PlotData();
        }
    }

    //Creates a data point reflecting results.
    void PlotData()
    {
        float score = (results[1] * 100f + results[0] * 50f) / playedGamesInMatch;
        if (!float.IsNaN(score))
        {
            plotter.createDataPoint((playedGamesInMatch * (1000f / numberOfGamesInMatch)) - 500, score, finishedMatches * 10);
        }
    }

    new void UpdateText()
    {
        base.UpdateText();
        playerOneNexusHealthText.text = "" + board.playerOneSide.nexus.health;
        playerTwoNexusHealthText.text = "" + board.playerTwoSide.nexus.health;
        playerOneManaGemText.text = "" + board.playerOneSide.mana.manaGems;
        playerOneSpellManaText.text = "" + board.playerOneSide.mana.spellMana;
        playerTwoManaGemText.text = "" + board.playerTwoSide.mana.manaGems;
        playerTwoSpellManaText.text = "" + board.playerTwoSide.mana.spellMana;
        UpdateDisplayBoard();
    }

    void UpdateDisplayBoard()
    {
        UpdatePlayerOneHand();
        UpdatePlayerOneBench();
        UpdatePlayerOneBattlefield();
        UpdatePlayerTwoHand();
        UpdatePlayerTwoBench();
        UpdatePlayerTwoBattlefield();
        UpdateSpellStack();
        UpdateAttackTokens();
        UpdateMana();
        UpdateRoundMessage();
    }

    void UpdatePlayerOneHand()
    {
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerOneSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBench()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Bench");
        int cardIndex = 1;
        foreach (Card card in board.playerOneSide.bench.units)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            Card card;
            if (board.attackingPlayer == 1)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            Sprite cardSprite = null;
            if (card != null)
            {
                foreach (Sprite sprite in cardImages)
                {
                    if (sprite.name.Equals(imageDictionary[card.name]))
                    {
                        cardSprite = sprite;
                    }
                }
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            }
            else
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            }
            cardIndex += 1;
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoHand()
    {
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerTwoSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoBench()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Bench");
        int cardIndex = 1;
        foreach (Card card in board.playerTwoSide.bench.units)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            Card card;
            if (board.attackingPlayer == 2)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            Sprite cardSprite = null;

            if (card != null)
            {
                foreach (Sprite sprite in cardImages)
                {
                    if (sprite.name.Equals(imageDictionary[card.name]))
                    {
                        cardSprite = sprite;
                    }
                }
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            }
            else
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            }

            cardIndex += 1;
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdateSpellStack()
    {
        int numSlots = 10;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Spell Stack");
        int cardIndex = 1;

        foreach (Card card in board.spellStack.spells)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdateAttackTokens()
    {
        if (board.playerOneSide.hasAttackToken)
        {
            attackTokenHandler.ShowAttackToken(1);
        }
        else if (board.playerTwoSide.hasAttackToken)
        {
            attackTokenHandler.ShowAttackToken(2);
        }
        else
        {
            attackTokenHandler.ShowNoToken(1);
        }
    }

    void UpdateMana()
    {
        playerOneManaGemDisplay.SetManaGemSprite(board.playerOneSide.mana.manaGems);
        playerTwoManaGemDisplay.SetManaGemSprite(board.playerTwoSide.mana.manaGems);
        playerOneSpellManaDisplay.SetSpellManaSprite(board.playerOneSide.mana.spellMana);
        playerTwoSpellManaDisplay.SetSpellManaSprite(board.playerTwoSide.mana.spellMana);
    }

    void UpdateRoundMessage()
    {
        if (board.roundNumber != currentRoundNumber)
        {
            currentRoundNumber = board.roundNumber;
            roundMessage.text = "Round " + currentRoundNumber;
        }
        else
        {
            roundMessage.text = "";
        }
    }
}