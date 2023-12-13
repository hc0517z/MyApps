using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyApps.Messages;

public class RefreshMessage : ValueChangedMessage<bool>
{
    public RefreshMessage(bool value) : base(value)
    {
    }
}