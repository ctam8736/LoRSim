public abstract class Player
{
    LoRBoard board;
    int playerNumber;
    Deck deck;
    Bench bench;
    Bench opposingBench;
    Hand hand;
    Hand opposingHand;
    Mana mana;
    Mana opposingMana;
    public abstract Action MakeAction();
    public abstract Deck Deck();
}