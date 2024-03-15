public class DamageEventData
{
    public Card Inflictor {  get; private set; }
    public Card Reciever {  get; private set; }
    public int Amount {  get; private set; }

    public DamageEventData (Card reciever, Card inflictor, int amount)
    {
        Reciever = reciever;
        Inflictor = inflictor;
        Amount = amount;
    }
}
