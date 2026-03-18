using Microsoft.AspNetCore.SignalR;
using NhakhoaMyNgoc_Connector.DTOs;

namespace SignalR
{
    public class SyncHub : Hub
    {
        public async Task<bool> SaveCustomer(CustomerDto msg)
        {
            await Clients.Others.SendAsync("OnSaveCustomer", msg);
            return true;
        }

        public async Task<bool> DeleteCustomer(CustomerDto msg)
        {
            await Clients.Others.SendAsync("OnDeleteCustomer", msg);
            return true;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            var app = builder.Build();
            app.MapHub<SyncHub>("/sync");
            app.MapGet("/", () => "SignalR server running"); // test
            app.Run();
        }
    }
}
