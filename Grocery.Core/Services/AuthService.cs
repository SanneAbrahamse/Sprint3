using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientService _clientService;
        public AuthService(IClientService clientService)
        {
            _clientService = clientService;
        }
        public Client? Login(string email, string password)
        {
            Client? client = _clientService.Get(email);
            if (client == null) return null;
            if (PasswordHelper.VerifyPassword(password, client.Password)) return client;
            return null;
            
        }
        
        public bool Register(Client client)
        {
            Console.WriteLine($"AuthService.Register called for: {client.EmailAddress}");
    
            if (_clientService.Get(client.EmailAddress) != null)
            {
                Console.WriteLine("Client already exists");
                return false;
            }
    
            Console.WriteLine("Hashing password");
            client.Password = PasswordHelper.HashPassword(client.Password);
            Console.WriteLine("Adding client to service");
            _clientService.Add(client);
            Console.WriteLine("Client added successfully");
            return true;
        }
    }
}
