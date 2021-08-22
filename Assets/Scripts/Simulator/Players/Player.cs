public abstract class Player
{
    protected LoRBoard board;
    protected int playerNumber;
    protected Deck deck;
    protected Bench bench;
    protected Bench opposingBench;
    protected Hand hand;
    protected Hand opposingHand;
    protected Mana mana;
    protected Mana opposingMana;
    public abstract Action MakeAction();
    public abstract Deck Deck();
}