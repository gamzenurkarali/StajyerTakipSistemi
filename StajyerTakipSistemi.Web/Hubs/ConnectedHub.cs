using StajyerTakipSistemi.Web.Models;
using Microsoft.AspNetCore.SignalR;
using MessagePack;

namespace StajyerTakipSistemi.Hubs
{
    public class ConnectedHub : Hub
    {
        private readonly StajyerTakipSistemiDbContext _context;
        private static bool isChatPageActive = true;
        public ConnectedHub(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderGuid, string receiverGuid, string text)
        {
            // Mesajı veritabanına kaydet
            var message = new Message
            {
                Sender = Guid.Parse(senderGuid),
                Receiver = Guid.Parse(receiverGuid),
                MessageText = text,
            };

            _context.Messages.Add(message);

            var newMessage = new NewMessages
            {
                Guid = Guid.NewGuid(),
                Sender = Guid.Parse(senderGuid),
                Receiver = Guid.Parse(receiverGuid),
                UnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };

            _context.NewMessages.Add(newMessage);

            await _context.SaveChangesAsync();

            if (!isChatPageActive)
            {
                
            } 
             
            // Mesajı alıcıya gönder
            await Clients.All.SendAsync("ReceiveMessage", senderGuid, receiverGuid, text, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public override async Task OnConnectedAsync()
        {
            isChatPageActive = true;
            await base.OnConnectedAsync();
        }

        // Kullanıcı sayfadan ayrıldığında
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            isChatPageActive = false;
            await base.OnDisconnectedAsync(exception);
        }
    }

}
