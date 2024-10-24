using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyApps.Messages
{
    public class CloseMessage : ValueChangedMessage<bool>
    {
        public CloseMessage(bool value) : base(value)
        {
        }
    }
}