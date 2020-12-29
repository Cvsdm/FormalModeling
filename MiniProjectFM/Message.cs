using System.ComponentModel.Design;

namespace MiniProjectFM
{
    /**
     * Class used to simulate a message sent between Services and the Resource Manager
     */
    public class Message
    {
        private EnumMessage Type { get; set; }

        private ISender Sender { get; set; }
        
        public Message(ISender sender, EnumMessage type)
        {
            Sender = sender;
            Type = type;
        }
    }
}