using CommunityToolkit.Mvvm.Messaging.Messages;
using MyApps.Models;

namespace MyApps.Messages;

public class OpenDirectoryAppMessage : ValueChangedMessage<ObservableApp>
{
    public OpenDirectoryAppMessage(ObservableApp value) : base(value)
    {
    }
}