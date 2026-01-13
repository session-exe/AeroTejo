using System.Security.Cryptography;
using System.Text;

namespace AeroTejo.Helpers
{
    /// <summary>
    /// Classe auxiliar para operações de hashing de passwords com salt
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Gera um salt aleatório para ser usado no hashing da password
        /// </summary>
        /// <returns>String com o salt em Base64</returns>
        public static string GenerateSalt()
        {
            // Gera 32 bytes aleatórios para o salt
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            
            // Converte para Base64 para armazenamento
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Cria um hash da password usando o salt fornecido
        /// </summary>
        /// <param name="password">Password em texto simples</param>
        /// <param name="salt">Salt a ser usado no hashing</param>
        /// <returns>Hash da password em Base64</returns>
        public static string HashPassword(string password, string salt)
        {
            // Converte o salt de Base64 para bytes
            byte[] saltBytes = Convert.FromBase64String(salt);
            
            // Converte a password para bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            
            // Combina password e salt
            byte[] passwordWithSalt = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, passwordWithSalt, passwordBytes.Length, saltBytes.Length);
            
            // Calcula o hash usando SHA256
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(passwordWithSalt);
                
                // Converte o hash para Base64 para armazenamento
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifica se a password fornecida corresponde ao hash armazenado
        /// </summary>
        /// <param name="password">Password em texto simples a verificar</param>
        /// <param name="storedHash">Hash armazenado na base de dados</param>
        /// <param name="storedSalt">Salt armazenado na base de dados</param>
        /// <returns>True se a password corresponder, False caso contrário</returns>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            // Gera o hash da password fornecida usando o salt armazenado
            string hashToVerify = HashPassword(password, storedSalt);
            
            // Compara os hashes de forma segura (timing attack resistant)
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(hashToVerify),
                Convert.FromBase64String(storedHash)
            );
        }
    }
}
