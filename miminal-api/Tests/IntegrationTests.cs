using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class IntegrationTests
    {
        private readonly HttpClient _client;

        public IntegrationTests()
        {
            // Configura o HttpClient para testar a API localmente
            _client = new HttpClient
            {
                BaseAddress = new System.Uri("https://localhost:5001") // Ajuste a porta
            };
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken()
        {
            
            var loginDto = new LoginDTO
            {
                Username = "admin",
                Password = "1234"
            };

            
            var response = await _client.PostAsJsonAsync("/login", loginDto);

            
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            ((string)result.token).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetVehicles_ShouldReturnList()
        {
            
            var response = await _client.GetAsync("/vehicles");

            
            response.EnsureSuccessStatusCode();
            var vehicles = await response.Content.ReadFromJsonAsync<dynamic>();
            vehicles.Should().NotBeNull();
        }
    }
}
