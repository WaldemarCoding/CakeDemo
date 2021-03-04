using CakeDemo.Services.Interfaces;

namespace CakeDemo.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
