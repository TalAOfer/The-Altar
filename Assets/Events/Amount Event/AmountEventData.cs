public class AmountEventData : IEventData
{
    public Card Inflictor {  get; private set; }
    public Card Reciever {  get; private set; }
    public int Amount {  get; private set; }

    public AmountEventData (Card reciever, Card inflictor, int amount)
    {
        Reciever = reciever;
        Inflictor = inflictor;
        Amount = amount;
    }
}

public interface IEventData
{

}