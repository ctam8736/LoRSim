using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayPrinter : GamePrinter
{
    public CardData cardData;
    public Sprite nullImage;
    public AttackToken playerOneAttackTokenHandler;
    public AttackToken playerTwoAttackTokenHandler;
    public GameObject displayCanvas;
    public GameObject manaContainer;

    ManaGemDisplay playerOneManaGemDisplay;
    ManaGemDisplay playerTwoManaGemDisplay;
    ManaGemDisplay playerOneSpellManaDisplay;
    ManaGemDisplay playerTwoSpellManaDisplay;
    TextMeshProUGUI playerOneNexusHealthText;
    TextMeshProUGUI playerTwoNexusHealthText;
    TextMeshProUGUI playerOneManaGemText;
    TextMeshProUGUI playerOneSpellManaText;
    TextMeshProUGUI playerTwoManaGemText;
    TextMeshProUGUI playerTwoSpellManaText;
    TextMeshProUGUI roundMessage;

    public int gamesPerFrame = 10;
    public string statsWriteFile = "Assets/stats.txt";
    public List<Card> stats = new List<Card>();

    //--- Play Timer Logic ---
    public float updateDelay = .5f;
    protected float timer = 0f;

    int currentRoundNumber = 0;

    void OnEnable()
    {
        lorGameManager.OnGameStateUpdated += UpdateText;
    }

    // Start is called before the first frame update
    void Start()
    {
        board = lorGameManager.board;
        game = lorGameManager.game;
        FindTextReferences();
        FindDisplayReferences();
        UpdateText();
    }
        
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateDelay)
        {
            UpdateText();
            timer = timer % updateDelay;
        }
    }

    public override void WriteEndGameOutput()
    {
        /**
        if (playedGamesInMatch != 0)
        {
            if (healthStats.Count < 100)
            {
                if (board.gameResult == 1)
                {
                    foreach (Card card in board.playerOneSide.hand.cards)
                    {
                        healthStats.Add(card);
                    }
                }
            }
            else
            {
                StreamWriter writer = new StreamWriter(healthWriteFile, true);
                for (int i = 0; i < healthStats.Count; i++)
                {
                    writer.Write("\n" + healthStats[i].name);
                }
                writer.Close();
                healthStats = new List<Card>();
            }
        }
        **/
    }

    protected void FindDisplayReferences()
    {
        playerOneNexusHealthText = GetTextComponent(displayCanvas, "P1 Nexus Health");
        playerTwoNexusHealthText = GetTextComponent(displayCanvas, "P2 Nexus Health");
        playerOneManaGemText = GetTextComponent(manaContainer, "P1 Mana Gems");
        playerOneSpellManaText = GetTextComponent(manaContainer, "P1 Spell Mana");
        playerTwoManaGemText = GetTextComponent(manaContainer, "P2 Mana Gems");
        playerTwoSpellManaText = GetTextComponent(manaContainer, "P2 Spell Mana");
        roundMessage = GetTextComponent(displayCanvas, "Round Message"); ;

        playerOneManaGemDisplay = GetManaGemComponent(manaContainer, "P1 Mana Gem Display");
        playerTwoManaGemDisplay = GetManaGemComponent(manaContainer, "P2 Mana Gem Display");
        playerOneSpellManaDisplay = GetManaGemComponent(manaContainer, "P1 Spell Mana Display");
        playerTwoSpellManaDisplay = GetManaGemComponent(manaContainer, "P2 Spell Mana Display");
    }

    ManaGemDisplay GetManaGemComponent(GameObject parent, string name)
    {
        return parent.transform.Find(name).gameObject.GetComponent<ManaGemDisplay>();
    }

    new void UpdateText()
    {
        board = lorGameManager.board;
        game = lorGameManager.game;

        base.UpdateText();
        if (!lorGameManager.showRounds) { return; }

        playerOneNexusHealthText.text = "" + board.playerOneSide.nexus.health;
        playerTwoNexusHealthText.text = "" + board.playerTwoSide.nexus.health;
        playerOneManaGemText.text = "" + board.playerOneSide.mana.manaGems;
        playerOneSpellManaText.text = "" + board.playerOneSide.mana.spellMana;
        playerTwoManaGemText.text = "" + board.playerTwoSide.mana.manaGems;
        playerTwoSpellManaText.text = "" + board.playerTwoSide.mana.spellMana;
        UpdateDisplayBoard();
    }

    //---Update Display Scene---

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
        Transform cardRegion = displayCanvas.transform.Find("Player One Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerOneSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardData.cardImages)
            {
                if (!cardData.imageDictionary.ContainsKey(card.name))
                {
                    Debug.Log("Could not find image for " + card.name);
                }
                if (sprite.name.Equals(cardData.imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            if (card is UnitCard)
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit((UnitCard)card);
            }
            else if (card is SpellCard)
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderSpell((SpellCard)card);
            }
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(null);
            cardIndex += 1;
        }
    }
    void UpdatePlayerTwoHand()
    {
        Transform cardRegion = displayCanvas.transform.Find("Player Two Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerTwoSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardData.cardImages)
            {
                if (!cardData.imageDictionary.ContainsKey(card.name))
                {
                    Debug.Log("Could not find image for " + card.name);
                }
                if (sprite.name.Equals(cardData.imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            if (card is UnitCard)
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit((UnitCard)card);
            }
            else if (card is SpellCard)
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderSpell((SpellCard)card);
            }
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(null);
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBench()
    {
        UpdateUnitCardDisplayArray(6, displayCanvas.transform.Find("Player One Bench"), board.playerOneSide.bench.units);
    }

    void UpdatePlayerTwoBench()
    {
        UpdateUnitCardDisplayArray(6, displayCanvas.transform.Find("Player Two Bench"), board.playerTwoSide.bench.units);
    }

    void UpdatePlayerOneBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = displayCanvas.transform.Find("Player One Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            UnitCard card;
            if (board.attackingPlayer == 1)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(card);
            cardIndex += 1;
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(null);
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = displayCanvas.transform.Find("Player Two Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            UnitCard card;
            if (board.attackingPlayer == 2)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(card);
            cardIndex += 1;
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(null);
            cardIndex += 1;
        }
    }

    void UpdateSpellStack()
    {
        int numSlots = 10;
        Transform cardRegion = displayCanvas.transform.Find("Spell Stack");
        int cardIndex = 1;

        foreach (Card card in board.spellStack.spells)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardData.cardImages)
            {
                if (sprite.name.Equals(cardData.imageDictionary[card.name]))
                {
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderSpell((SpellCard)card);
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderSpell(null);
            cardIndex += 1;
        }
    }

    void UpdateAttackTokens()
    {
        if (board.playerOneSide.hasAttackToken)
        {
            playerOneAttackTokenHandler.ShowAttackToken();
        }
        else
        {
            playerOneAttackTokenHandler.ShowNoToken();
        }

        if (board.playerTwoSide.hasAttackToken)
        {
            playerTwoAttackTokenHandler.ShowAttackToken();
        }
        else
        {
            playerTwoAttackTokenHandler.ShowNoToken();
        }
    }

    void UpdateUnitCardDisplayArray(int numSlots, Transform cardRegion, List<UnitCard> units)
    {

        int cardIndex = 1;
        foreach (UnitCard card in units)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(card);
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<CardRenderer>().RenderUnit(null);
            cardIndex += 1;
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