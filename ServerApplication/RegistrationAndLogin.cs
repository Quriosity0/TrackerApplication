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
            bool Result = false;
            foreach (string name in RegistrationName)
            {
                if (name == Login)
                {

                    Result = false;

                }
                else
                {
                    Result = true;
                }
            }
            return Result;
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
            bool Result = false;
            foreach (string name in LoginNames)
            {
                if (name == Login)
                {

                    Result = true;

                }
                else
                {
                    Result = false;
                }
            }
            return Result;
        }
        public static bool LoginPassword(string password)
        {
            LoginPass = TrakerBD.GetPassword();
            bool Result = false;
            foreach (string pass in LoginPass)
            {
                if (pass == password)
                {

                    Result = true;

                }
                else
                {
                    Result = false;
                }
            }
            return Result;
        }
    }
}