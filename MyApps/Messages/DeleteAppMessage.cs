using CommunityToolkit.Mvvm.Messaging.Messages;
using MyApps.Models;

namespace MyApps.Messages;

public class DeleteAppMessage : ValueChangedMessage<ObservableApp>
{
    public DeleteAppMessage(ObservableApp value) : base(value)
    {
    }
}