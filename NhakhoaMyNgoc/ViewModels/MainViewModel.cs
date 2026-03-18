using Microsoft.AspNetCore.SignalR.Client;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NhakhoaMyNgoc.ViewModels
{
    public class MainViewModel
    {
        public AppViewModel AppVM { get; }
        public CustomerViewModel CustomerVM { get; }
        public InvoiceViewModel InvoiceVM { get; }
        public ServiceViewModel ServiceVM { get; }
        public ImageViewModel ImageVM { get; }
        public ProductService ProductSvc { get; }
        public IDNViewModel IdnVM { get; }
        public ProductViewModel ProductVM { get; }
        public ExpenseViewModel ExpenseVM { get; }

        HubConnection syncConn;

        public MainViewModel(DataContext db)
        {
            // TODO: đổi
            syncConn = new HubConnectionBuilder()
                .WithUrl("http://localhost:5081/sync")
                .WithAutomaticReconnect()
                .Build();

            CustomerVM = new(db, syncConn);
            ServiceVM = new(db, syncConn, loadDeleted: false);
            InvoiceVM = new(db, ServiceVM.Services, syncConn);
            ImageVM = new(db, syncConn);

            ProductSvc = new ProductService(db, syncConn);
            AppVM = new(db, syncConn, ProductSvc);

            IdnVM = new(db, ProductSvc, syncConn);
            ProductVM = new(ProductSvc, syncConn, loadDeleted: false)
                { IsReadOnly = true };

            ExpenseVM = new(db, syncConn);
        }

        // gọi sau khi tạo VM
        public async Task<bool> StartSyncAsync()
        {
            try
            {
                await syncConn.StartAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
