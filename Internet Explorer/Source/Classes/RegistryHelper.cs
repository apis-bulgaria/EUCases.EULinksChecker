using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace EUCases.Classes
{
    public static class RegistryHelper
    {
        private readonly static string registryPath = "Software\\ConsumerCases\\IEAddin";
        private readonly static string userName = "UserName";
        private readonly static string password = "Password";
        private readonly static string language = "UILanguage";

        public static bool SetValues(string user, string pass, string lang)
        {
            bool isOk = true;
            try
            {
                using (RegistryKey ieAddinKey = Registry.CurrentUser.CreateSubKey(registryPath))
                {
                    ieAddinKey.SetValue(userName, user, RegistryValueKind.String);
                    var pw = XorPassword(pass);
                    ieAddinKey.SetValue(password, pw, RegistryValueKind.String);
                    ieAddinKey.SetValue(language, lang, RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                // log?
                isOk = false;
            }

            return isOk;
        }

        public static bool ChangeLanguage(string newLang)
        {
            bool isOk = true;
            try
            {
                using (RegistryKey ieAddinKey = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (ieAddinKey != null)
                    {
                        ieAddinKey.SetValue(language, newLang, RegistryValueKind.String);
                    }
                    else
                    {
                        isOk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isOk = false;
            }

            return isOk;
        }

        public static bool ChangeCredentials(string newUser, string newPassword)
        {
            bool isOk = true;
            try
            {
                using (RegistryKey ieAddinKey = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (ieAddinKey != null)
                    {
                        ieAddinKey.SetValue(userName, newUser, RegistryValueKind.String);
                        var pw = XorPassword(newPassword);
                        ieAddinKey.SetValue(password, pw, RegistryValueKind.String);
                    }
                    else
                    {
                        isOk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isOk = false;
            }

            return isOk;
        }

        public static bool GetUserSettings(out UserRegistryData userSettings)
        {
            userSettings = new UserRegistryData();
            bool isOk = true;
            try
            {
                using (RegistryKey ieAddinKey = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    var un = ieAddinKey.GetValue(userName) as string;
                    if (!string.IsNullOrEmpty(un))
                    {
                        userSettings.UserName = un;
                    }
                    else
                    {
                        isOk = false;
                    }

                    var pw = ieAddinKey.GetValue(password) as string;
                    if (!string.IsNullOrEmpty(pw))
                    {
                        var xorpw = XorPassword(pw);
                        userSettings.Password = xorpw;
                    }
                    else
                    {
                        isOk = false;
                    }

                    var ln = ieAddinKey.GetValue(language) as string;
                    if (!string.IsNullOrEmpty(ln))
                    {
                        userSettings.Language = ln;
                    }
                    else
                    {
                        isOk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isOk = false;
            }

            return isOk;
        }

        public static bool GetLanguage(out string lang)
        {
            lang = string.Empty;
            bool isOk = true;
            try
            {
                RegistryKey ieAddinKey = Registry.CurrentUser.OpenSubKey(registryPath);
                if (ieAddinKey != null)
                {
                    var ln = ieAddinKey.GetValue(language) as string;
                    if (!string.IsNullOrEmpty(ln))
                    {
                        lang = ln;
                    }
                    else
                    {
                        isOk = false;
                    }
                }
                else
                {
                    isOk = false;
                }
            }
            catch (Exception ex)
            {
                isOk = false;
            }

            return isOk;
        }

        private static string XorPassword(string pass)
        {
            char[] xorPass = new char[pass.Length];
            for (int i = 0; i < pass.Length; i++)
            {
                xorPass[i] = (char)((int)pass[i] ^ 17);
            }

            return new string(xorPass);
        }

        public static bool Delete()
        {
            bool isOk = true;
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(registryPath);
                if (rk != null)
                {
                    rk.Close();
                    Registry.CurrentUser.DeleteSubKey(registryPath);
                }
            }
            catch (Exception)
            {
                isOk = false;
            }

            return isOk;
        }
    }
}
