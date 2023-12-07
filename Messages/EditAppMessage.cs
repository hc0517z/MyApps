using CommunityToolkit.Mvvm.Messaging.Messages;
using MyApps.Models;

namespace MyApps.Messages;

public class EditAppMessage : ValueChangedMessage<ObservableApp>
{
    public EditAppMessage(ObservableApp value) : base(value)
    {
    }
}