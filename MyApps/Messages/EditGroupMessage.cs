using CommunityToolkit.Mvvm.Messaging.Messages;
using MyApps.Models;

namespace MyApps.Messages;

public class EditGroupMessage : ValueChangedMessage<ObservableGroup>
{
    public EditGroupMessage(ObservableGroup value) : base(value)
    {
    }
}