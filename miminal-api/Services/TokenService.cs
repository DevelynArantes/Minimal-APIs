using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miminal_api.Services
{
    public class TokenService
    {
          private static string secretKey = "SUA_CHAVE_SECRETA_AQUI";

        // Método para gerar token JWT
        public static string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.Name, username) 
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Expiração em 1 hora
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
