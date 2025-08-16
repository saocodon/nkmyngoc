using CommunityToolkit.Mvvm.Messaging.Messages;
using NhakhoaMyNgoc.Models;

namespace NhakhoaMyNgoc
{
    public class SelectedCustomerChangedMessage : ValueChangedMessage<Customer>
    {
        public SelectedCustomerChangedMessage(Customer value) : base(value) { }
    }

    public class AddCustomerImageMessage : ValueChangedMessage<(Customer customer, string[] fileNames)>
    {
        public AddCustomerImageMessage((Customer, string[]) value) : base(value) { }
    }

    public class SaveCustomerMessage : ValueChangedMessage<object?>
    {
        public SaveCustomerMessage(object? value = null) : base(value) { }
    }
}
