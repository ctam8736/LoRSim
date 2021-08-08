using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CardData : MonoBehaviour
{
    public List<Sprite> cardImages;
    public static Dictionary<string, Card> cardDictionary;
    public Dictionary<string, string> imageDictionary = new Dictionary<string, string>();

    public void Awake()
    {
        ImportSpritesFromFolder("CardImages/Set 1/Demacia");
        ImportSpritesFromFolder("CardImages/Set 1/PnZ");
        FillImageDictionary();
        FillCardDictionary();
    }

    protected void ImportSpritesFromFolder(string path)
    {
        Object[] importedObjects = Resources.LoadAll(path, typeof(Sprite));
        foreach (Object thing in importedObjects)
        {
            cardImages.Add((Sprite)thing);
        }
    }

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
        imageDictionary.Add("Sumpworks Map", "01PZ026");
        imageDictionary.Add("Succession", "01DE047");
        imageDictionary.Add("Dauntless Vanguard", "01DE016");
        imageDictionary.Add("Unlicensed Innovation", "01PZ014");
        imageDictionary.Add("Illegal Contraption", "01PZ014T1");
        imageDictionary.Add("Laurent Bladekeeper", "01DE003");
        imageDictionary.Add("Vanguard Sergeant", "01DE006");
        imageDictionary.Add("For Demacia!", "01DE035");
        imageDictionary.Add("Vanguard Bannerman", "01DE001");
        imageDictionary.Add("Stand Alone", "01DE017");
    }

    public static void FillCardDictionary()
    {
        cardDictionary = new Dictionary<string, Card>(){
            /**
            {"Cithria of Cloudfield", new UnitCard("Cithria of Cloudfield", 1, 2, 2)},
            {"Bloodthirsty Marauder", new UnitCard("Bloodthirsty Marauder", 1, 3, 1)},
            {"Legion Rearguard", new UnitCard("Legion Rearguard", 1, 3, 2, new List<Keyword> { Keyword.CantBlock })},
            {"Prowling Cutthroat", new UnitCard("Prowling Cutthroat", 1, 1, 1, new List<Keyword> { Keyword.Elusive, Keyword.Fearsome })},
            {"Precious Pet", new UnitCard("Precious Pet", 1, 2, 1, new List<Keyword> { Keyword.Fearsome })},
            {"Sinister Poro", new UnitCard("Sinister Poro", 1, 1, 1, new List<Keyword> { Keyword.Fearsome })},
            {"Daring Poro", new UnitCard("Daring Poro", 1, 1, 1, new List<Keyword> { Keyword.Elusive })},
            {"Nimble Poro", new UnitCard("Nimble Poro", 1, 1, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Plucky Poro", new UnitCard("Plucky Poro", 1, 1, 1, new List<Keyword> { Keyword.Tough })},
            {"Vanguard Lookout", new UnitCard("Vanguard Lookout", 2, 1, 4)},
            {"Startled Stomper", new UnitCard("Startled Stomper", 2, 2, 3, new List<Keyword> { Keyword.Overwhelm })},
            {"Vanguard Defender", new UnitCard("Vanguard Defender", 2, 2, 2, new List<Keyword> { Keyword.Tough })},
            {"Ruthless Raider", new UnitCard("Ruthless Raider", 2, 3, 1, new List<Keyword> { Keyword.Overwhelm, Keyword.Tough })},
            {"Academy Prodigy", new UnitCard("Academy Prodigy", 2, 3, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Arachnid Horror", new UnitCard("Arachnid Horror", 2, 3, 2, new List<Keyword> { Keyword.Fearsome })},
            {"Loyal Badgerbear", new UnitCard("Loyal Badgerbear", 3, 3, 4)},
            {"Golden Crushbot", new UnitCard("Golden Crushbot", 3, 2, 5)},
            {"Amateur Aeronaut", new UnitCard("Amateur Aeronaut", 3, 2, 3, new List<Keyword> { Keyword.Elusive })},
            {"Iron Ballista", new UnitCard("Iron Ballista", 3, 4, 3, new List<Keyword> { Keyword.Overwhelm })},
            {"Reckless Trifarian", new UnitCard("Reckless Trifarian", 3, 5, 4, new List<Keyword> { Keyword.CantBlock })},
            {"Silverwing Diver", new UnitCard("Silverwing Diver", 4, 2, 3, new List<Keyword> { Keyword.Elusive, Keyword.Tough })},
            {"Bull Elnuk", new UnitCard("Bull Elnuk", 4, 4, 5)},
            {"Trifarian Shieldbreaker", new UnitCard("Trifarian Shieldbreaker", 5, 6, 5, new List<Keyword> { Keyword.Fearsome })},
            {"Alpha Wildclaw", new UnitCard("Alpha Wildclaw", 6, 7, 6, new List<Keyword> { Keyword.Overwhelm })},
            {"The Empyrean", new UnitCard("The Empyrean", 7, 6, 5, new List<Keyword> { Keyword.Elusive })},

            {"Health Potion", new SpellCard("Health Potion", 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnitOrNexus})},
            {"Radiant Strike", new SpellCard("Radiant Strike", 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Chain Vest", new SpellCard("Chain Vest", 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Sumpworks Map", new SpellCard("Sumpworks Map", 2, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Mystic Shot", new SpellCard("Mystic Shot", 2, SpellType.Fast, new List<TargetType>{TargetType.Anything})},
            //{"Avalanche", new SpellCard("Avalanche", 4, SpellType.Slow, null)},
            {"Decimate", new SpellCard("Decimate", 5, SpellType.Slow, new List<TargetType>{TargetType.EnemyNexus})},
            {"Succession", new SpellCard("Succession", 3, SpellType.Slow, null)},
            {"Unlicensed Innovation", new SpellCard("Unlicensed Innovation", 6, SpellType.Slow, null)}
            **/

            {"Cithria of Cloudfield", new UnitCard("Cithria of Cloudfield", Region.Demacia, 1, 2, 2)},
            {"Plucky Poro", new UnitCard("Plucky Poro", Region.Demacia, 1, 1, 1, new List<Keyword> { Keyword.Tough })},
            {"Vanguard Lookout", new UnitCard("Vanguard Lookout", Region.Demacia, 2, 1, 4)},
            {"Vanguard Defender", new UnitCard("Vanguard Defender", Region.Demacia, 2, 2, 2, new List<Keyword> { Keyword.Tough })},
            {"Silverwing Diver", new UnitCard("Silverwing Diver", Region.Demacia, 4, 2, 3, new List<Keyword> { Keyword.Elusive, Keyword.Tough })},

            {"Daring Poro", new UnitCard("Daring Poro", Region.PnZ, 1, 1, 1, new List<Keyword> { Keyword.Elusive })},
            {"Academy Prodigy", new UnitCard("Academy Prodigy", Region.PnZ, 2, 3, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Golden Crushbot", new UnitCard("Golden Crushbot", Region.PnZ, 3, 2, 5)},
            {"Amateur Aeronaut", new UnitCard("Amateur Aeronaut", Region.PnZ, 3, 2, 3, new List<Keyword> { Keyword.Elusive })},

            {"Radiant Strike", new SpellCard("Radiant Strike", Region.Demacia, 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Sumpworks Map", new SpellCard("Sumpworks Map", Region.PnZ, 2, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Mystic Shot", new SpellCard("Mystic Shot", Region.PnZ, 2, SpellType.Fast, new List<TargetType>{TargetType.Anything})},
            {"Succession", new SpellCard("Succession", Region.Demacia, 3, SpellType.Slow, null)},
            {"Unlicensed Innovation", new SpellCard("Unlicensed Innovation", Region.PnZ, 6, SpellType.Slow, null)},

            {"Laurent Bladekeeper", new UnitCard("Laurent Bladekeeper", Region.Demacia, 4, 3, 3, onPlay: new SpellCard("Laurent Bladekeeper Play", Region.Null, 0, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit}))},
            {"Vanguard Sergeant", new UnitCard("Vanguard Sergeant", Region.Demacia, 3, 3, 3, onSummon: new SpellCard("Vanguard Sergeant Summon", Region.Null, 0, SpellType.Burst, null))},
            {"For Demacia!", new SpellCard("For Demacia!", Region.Demacia, 6, SpellType.Slow, null)},
            {"Stand Alone", new SpellCard("Stand Alone", Region.Demacia, 4, SpellType.Burst, null)},
            {"Vanguard Cavalry", new UnitCard("Vanguard Cavalry", Region.Demacia, 5, 5, 5, new List<Keyword> { Keyword.Tough })},

            {"Vanguard Bannerman", new UnitCard("Vanguard Bannerman", Region.Demacia, 4, 3, 3, onSummon: new SpellCard("Vanguard Bannerman Summon", Region.Null, 0, SpellType.Burst, null))},
            //{"Back to Back", new SpellCard("Back to Back", Region.Demacia, 6, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit, TargetType.AlliedUnit})},
            //{"Laurent Protege", new UnitCard("Laurent Protege", Region.Demacia, 3, 2, 4, new List<Keyword> { Keyword.Challenger })},
        };
    }

    public static Card ConvertToCard(string cardName)
    {
        if (cardDictionary == null) { FillCardDictionary(); }

        if (cardDictionary.ContainsKey(cardName))
        {
            Card newCard = cardDictionary[cardName];
            if (newCard is UnitCard)
            {
                return UnitCard.CopyCard((UnitCard)newCard);
            }
            if (newCard is SpellCard)
            {
                return SpellCard.CopyCard((SpellCard)newCard);
            }
        }

        return null;
    }

    //Creates a deck from a json file given path.
    public static Deck LoadDeckFromJson(string filePath)
    {
        if (cardDictionary == null) { FillCardDictionary(); }

        string deckOneJson;
        using (StreamReader reader = new StreamReader(filePath))
        {
            deckOneJson = reader.ReadToEnd();
        }
        DeckBuilder deck = JsonUtility.FromJson<DeckBuilder>(deckOneJson);
        return deck.ToDeck(CardData.cardDictionary);
    }

    public class DeckBuilder
    {
        public string name;
        public string[] cards;

        public DeckBuilder()
        {
        }
        public Deck ToDeck(Dictionary<string, Card> cardDictionary)
        {
            List<Card> newCards = new List<Card>();
            foreach (string cardName in cards)
            {
                newCards.Add(ConvertToCard(cardName));
            }
            Deck deck = new Deck(name, newCards);
            return deck;
        }
    }
}