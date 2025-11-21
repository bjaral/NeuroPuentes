using BCrypt.Net;

namespace NeuroPuentesAPI.Utils
{
    public static class PasswordHelper
    {
        // Hashea una contraseña en texto plano
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verifica si el hash coincide con la contraseña en texto plano
        public static bool Verify(string hashedPassword, string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}