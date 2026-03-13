using System.Collections.Generic;
using DBNamespace;

namespace RegistrationAndLogin
{
    class RegistrationAndLogin
    {
        private static List<string> RegistrationName = new List<string>();
        private static List<string> LoginNames = new List<string>();
        private static List<string> LoginPass = new List<string>();
        public static bool RegistrName(string Login)
        {
            RegistrationName = TrakerBD.GetName();
            foreach (string name in RegistrationName)
            {
                if (name == Login)
                {

                    return false;

                }

            }
            return true;
        }
        public static bool RegistrPassword(string password)
        {
            if (password.Length >= 8 && password.Length <= 14)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool LoginName(string Login)
        {
            LoginNames = TrakerBD.GetName();
            foreach (string name in LoginNames)
            {
                if (name == Login)
                {

                    return true;

                }
            }
            return false;
        }
        public static bool LoginPassword(string password)
        {
            LoginPass = TrakerBD.GetPassword();
            foreach (string pass in LoginPass)
            {
                if (pass == password)
                {

                    return true;

                }
            }
            return false;
        }
    }
}
