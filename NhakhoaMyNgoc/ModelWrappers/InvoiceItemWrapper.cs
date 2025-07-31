using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;

public class InvoiceItemWrapper : ObservableObject
{
    public InvoiceItem Model { get; }

    public InvoiceItemWrapper() : this(new InvoiceItem()) { }

    public InvoiceItemWrapper(InvoiceItem item)
    {
        Model = item;
    }

    public int Quantity
    {
        get => Model.Quantity;
        set
        {
            if (Model.Quantity != value)
            {
                Model.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public int Price
    {
        get => Model.Price;
        set
        {
            if (Model.Price != value)
            {
                Model.Price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public int Discount
    {
        get => Model.Discount;
        set
        {
            if (Model.Discount != value)
            {
                Model.Discount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public int Total => Quantity * Price - Discount;

    public int Id => Model.Id;
    public int InvoiceId { get => Model.InvoiceId; set => Model.InvoiceId = value; }
    public int ServiceId { get => Model.ServiceId; set => Model.ServiceId = value; }

    public Service Service
    {
        get => Model.Service;
        set => Model.Service = value;
    }
}
