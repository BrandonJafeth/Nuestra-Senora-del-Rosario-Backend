using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Security
{
    public class PasswordGenerator
    {
        private static Random random = new Random();

        public static string GenerateRandomPassword()
        {
            const string upperCaseLetters = "BCDFGHJKLMNPQRSTVWXYZ";
            const string numbers = "0123456789";

            string password = new string(Enumerable.Repeat(upperCaseLetters, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            password += new string(Enumerable.Repeat(numbers, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
    }
}