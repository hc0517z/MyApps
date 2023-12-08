using CommunityToolkit.Mvvm.Messaging.Messages;
using MyApps.Models;

namespace MyApps.Messages;

public class DeleteGroupMessage : ValueChangedMessage<ObservableGroup>
{
    public DeleteGroupMessage(ObservableGroup value) : base(value)
    {
    }
}