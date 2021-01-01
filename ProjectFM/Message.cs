namespace ProjectFM
{
    /**
     * Class used to simulate a message sent between Services and the Resource Provider
     */
    public class Message
    {
        public EnumMessage Type { get; set; }

        public ISender Sender { get; set; }

        public Message(ISender sender, EnumMessage type)
        {
            Sender = sender;
            Type = type;
        }

        public override string ToString()
        {
            return "Message : " + Type + " by " + Sender.Name;
        }
    }
}