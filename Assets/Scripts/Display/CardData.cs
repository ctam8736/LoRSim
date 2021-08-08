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

    /// <summary>
    /// Import card images from rResources folder to internal list.
    /// </summary>
    protected void ImportSpritesFromFolder(string path)
    {
        Object[] importedObjects = Resources.LoadAll(path, typeof(Sprite));
        foreach (Object thing in importedObjects)
        {
            cardImages.Add((Sprite)thing);
        }
    }

    /// <summary>
    /// Fill dictionary with card image IDs.
    /// </summary>
    public void FillImageDictionary()
    {
        imageDictionary.Add("Vanguard Bannerman", "01DE001");
        //imageDictionary.Add("Tianna Crownguard", "01DE002");
        imageDictionary.Add("Laurent Bladekeeper", "01DE003");
        imageDictionary.Add("Vanguard Sergeant", "01DE006");
        imageDictionary.Add("Dauntless Vanguard", "01DE016");
        imageDictionary.Add("Stand Alone", "01DE017");
        imageDictionary.Add("Radiant Strike", "01DE018");
        imageDictionary.Add("Vanguard Defender", "01DE020");
        imageDictionary.Add("Single Combat", "01DE026");
        imageDictionary.Add("Silverwing Diver", "01DE030");
        imageDictionary.Add("For Demacia!", "01DE035");
        imageDictionary.Add("Cithria of Cloudfield", "01DE039");
        imageDictionary.Add("Vanguard Lookout", "01DE046");
        imageDictionary.Add("Succession", "01DE047");
        imageDictionary.Add("Plucky Poro", "01DE049");

        imageDictionary.Add("Amateur Aeronaut", "01PZ009");
        imageDictionary.Add("Unlicensed Innovation", "01PZ014");
        imageDictionary.Add("Illegal Contraption", "01PZ014T1");
        imageDictionary.Add("Academy Prodigy", "01PZ018");
        imageDictionary.Add("Daring Poro", "01PZ020");
        imageDictionary.Add("Sumpworks Map", "01PZ026");
        imageDictionary.Add("Mystic Shot", "01PZ052");
        imageDictionary.Add("Golden Crushbot", "01PZ059");
    }

    /// <summary>
    /// Fill dictionary with card information.
    /// </summary>
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

            // Set 1 Demacia

            {"Vanguard Bannerman", new UnitCard("Vanguard Bannerman", Region.Demacia, 4, 3, 3, onSummon: new SpellCard("Vanguard Bannerman Summon", Region.Null, 0, SpellType.Burst, null))},
            //{"Tianna Crownguard", new UnitCard("Tianna Crownguard", Region.Demacia, 8, 8, 8, new List<Keyword> { Keyword.Tough }, onSummon: new SpellCard("Tianna Crownguard Summon", Region.Null, 0, SpellType.Burst, null))},
            {"Laurent Bladekeeper", new UnitCard("Laurent Bladekeeper", Region.Demacia, 4, 3, 3, onPlay: new SpellCard("Laurent Bladekeeper Play", Region.Null, 0, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit}))},
            //{"Silverwing Vanguard", new UnitCard("Silverwing Vanguard", Region.Demacia, 4, 2, 1, new List<Keyword> { Keyword.Challnger }, onSummon: ?????)},
            {"Vanguard Sergeant", new UnitCard("Vanguard Sergeant", Region.Demacia, 3, 3, 3, onSummon: new SpellCard("Vanguard Sergeant Summon", Region.Null, 0, SpellType.Burst, null))},
            //{"Judgement", new SpellCard("Judgement", Region.Demacia, 8, SpellType.Fast, new List<TargetType>{TargetType.AlliedUnit})},
            //{"Brightsteel Protector", new UnitCard("Brightsteel Protector", Region.Demacia, 2, 3, 2, onPlay: new SpellCard("Brightsteel Protector Play", Region.Null, 0, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit}))},
            //Swiftwing Lancer
            //{"Laurent Protege", new UnitCard("Laurent Protege", Region.Demacia, 3, 2, 4, new List<Keyword> { Keyword.Challenger })},
            //garen
            //garen 2
            //{"Garen's Judgement", new SpellCard("Garen's Judgement", Region.Demacia, 8, SpellType.Fast, new List<TargetType>{TargetType.AlliedUnit})},
            {"Chain Vest", new SpellCard("Chain Vest", Region.Demacia, 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            //Reinforcements
            //Radiant Guardian
            {"Stand Alone", new SpellCard("Stand Alone", Region.Demacia, 4, SpellType.Burst, null)},
            {"Radiant Strike", new SpellCard("Radiant Strike", Region.Demacia, 1, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            //Mobilize
            {"Vanguard Defender", new UnitCard("Vanguard Defender", Region.Demacia, 2, 2, 2, new List<Keyword> { Keyword.Tough })},
            //{"Relentless Pursuit", new SpellCard("Relentless Pursuit", Region.Demacia, 3, SpellType.Slow, null)},
            //Lucian
            //lucian 2
            //{"Lucian's Relentless Pursuit", new SpellCard("Lucian's Relentless Pursuit", Region.Demacia, 3, SpellType.Slow, null)},
            //Mageseeker Investigator
            //Mageseeker Conservator
            //Detain
            {"Single Combat", new SpellCard("Single Combat", Region.Demacia, 2, SpellType.Fast, new List<TargetType>{TargetType.AlliedUnit, TargetType.EnemyUnit})},
            //En Garde
            {"Vanguard Cavalry", new UnitCard("Vanguard Cavalry", Region.Demacia, 5, 5, 5, new List<Keyword> { Keyword.Tough })},
            //Fleetfeather Tracker
            {"Silverwing Diver", new UnitCard("Silverwing Diver", Region.Demacia, 4, 2, 3, new List<Keyword> { Keyword.Elusive, Keyword.Tough })},
            //Dawnspeakers
            //{"Prismatic Barrier", new SpellCard("Prismatic Barrier", Region.Demacia, 3, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            //Rememberance
            //{"Battlesmith", new UnitCard("Battlesmith", Region.Demacia, 2, 2, 2, ?????)},
            {"For Demacia!", new SpellCard("For Demacia!", Region.Demacia, 6, SpellType.Slow, null)},
            //Vanguard Squire
            //Riposte
            //Senna, Sentinel of Light
            {"Cithria of Cloudfield", new UnitCard("Cithria of Cloudfield", Region.Demacia, 1, 2, 2)},
            //Mageseeker Persuader
            //{"Back to Back", new SpellCard("Back to Back", Region.Demacia, 6, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit, TargetType.AlliedUnit})},
            //{"Lux", new Champion("Lux", Region.Demacia, 6, 4, 5, new List<Keyword> { Keyword.Barrier})},
            //{"Lux", new Champion("Lux", Region.Demacia, 6, 5, 6, new List<Keyword> { Keyword.Barrier})},
            //lux prismatic barrier
            //war chefs
            //{"Redoubled Valor", new SpellCard("Redoubled Valor", Region.Demacia, 6, SpellType.Slow, new List<TargetType>{TargetType.AlliedUnit})},
            //{"Fiora", new Champion("Fiora", Region.Demacia, 3, 3, 2, new List<Keyword> { Keyword.Challenger})},
            //fiora 2
            //fiora riposte
            {"Vanguard Lookout", new UnitCard("Vanguard Lookout", Region.Demacia, 2, 1, 4)},
            {"Succession", new SpellCard("Succession", Region.Demacia, 3, SpellType.Slow, null)},
            //{"Mageseeker Inciter", new UnitCard("Mageseeker Inciter", Region.Demacia, 4, 4, 4, trigger: new SpellCard("Mageseeker Inciter Trigger", Region.Null, 0, SpellType.Burst, null))},
            {"Plucky Poro", new UnitCard("Plucky Poro", Region.Demacia, 1, 1, 1, new List<Keyword> { Keyword.Tough })},
            //{"Purify", new SpellCard("Radiant Strike", Region.Demacia, 2, SpellType.Burst, new List<TargetType>{TargetType.AnyUnit})},
            //{"Cithria the Bold", new UnitCard("Cithria the Bold", Region.Demacia, 6, 6, 6, onAttack: new SpellCard("Cithria the Bold Attack", Region.Null, 0, SpellType.Burst, null))},
            //{"Brightsteel Formation", new UnitCard("Brightsteel Formation", Region.Demacia, 9, 9, 9, onSummon: new SpellCard("Brightsteel Formation Summon/Attack", Region.Null, 0, SpellType.Burst, null), onAttack: new SpellCard("Brightsteel Formation Summon/Attack", Region.Null, 0, SpellType.Burst, null))},
            //{"Laurent Chevalier", new UnitCard("Laurent Chevalier", Region.Demacia, 4, 3, 2, onStrike: new SpellCard("Laurent Chevalier Strike", Region.Null, 0, SpellType.Burst, null))},
            //{"Vanguard Firstblade", new UnitCard("Vanguard Firstblade", Region.Demacia, 4, 2, 2, onAttack: new SpellCard("Vanguard Firstblade Attack", Region.Null, 0, SpellType.Burst, null))},
            //{"Laurent Duelist", new UnitCard("Laurent Duelist", Region.Demacia, 3, 4, 2, onSummon: new SpellCard("Laurent Duelist Summon", Region.Null, 0, SpellType.Burst, null))},
            //{"Vanguard Redeemer", new UnitCard("Vanguard Redeemer", Region.Demacia, 3, 3, 3, onSummon: new SpellCard("Vanguard Redeemer Summon", Region.Null, 0, SpellType.Burst, null))},

            {"Daring Poro", new UnitCard("Daring Poro", Region.PnZ, 1, 1, 1, new List<Keyword> { Keyword.Elusive })},
            {"Academy Prodigy", new UnitCard("Academy Prodigy", Region.PnZ, 2, 3, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Golden Crushbot", new UnitCard("Golden Crushbot", Region.PnZ, 3, 2, 5)},
            {"Amateur Aeronaut", new UnitCard("Amateur Aeronaut", Region.PnZ, 3, 2, 3, new List<Keyword> { Keyword.Elusive })},
            {"Sumpworks Map", new SpellCard("Sumpworks Map", Region.PnZ, 2, SpellType.Burst, new List<TargetType>{TargetType.AlliedUnit})},
            {"Mystic Shot", new SpellCard("Mystic Shot", Region.PnZ, 2, SpellType.Fast, new List<TargetType>{TargetType.Anything})},
            {"Unlicensed Innovation", new SpellCard("Unlicensed Innovation", Region.PnZ, 6, SpellType.Slow, null)},
        };
    }

    /// <summary>
    /// Converts a card name string to its corresponding card.
    /// </summary>
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

    /// <summary>
    /// Creates a deck from a json file given path.
    /// </summary>
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