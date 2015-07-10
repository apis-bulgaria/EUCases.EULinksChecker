using EUCases.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace EUCases.Classes
{
    public static class LanguagesHelper
    {
        public static Dictionary<string, Dictionary<string, string>> Translations = new Dictionary<string, Dictionary<string, string>>()
        {
            {   
                "en",
                new Dictionary<string, string>
                {
                    {"processText", "Process text"},
                    {"removeLinks", "Remove links"},
                    {"insertLink", "Insert link"},
                    {"removeLinkFromSel", "Remove link from selection"},
                    {"saveXml", "Save xml"},
                    {"settings", "Settings"},
                } 
            },
            {   
                "fr",
                new Dictionary<string, string>
                {
                    {"processText", "Process text"},
                    {"removeLinks", "Remove links"},
                    {"insertLink", "Insert link"},
                    {"removeLinkFromSel", "Remove link from selection"},
                    {"saveXml", "Save xml"},
                    {"settings", "Settings"},
                } 
            },
            {   
                "de",
                new Dictionary<string, string>
                {
                    {"processText", "Process text"},
                    {"removeLinks", "Remove links"},
                    {"insertLink", "Insert link"},
                    {"removeLinkFromSel", "Remove link from selection"},
                    {"saveXml", "Save xml"},
                    {"settings", "Settings"},
                } 
            },
            {   
                "bg",
                new Dictionary<string, string>
                {
                    {"processText", "Провери за връзки"},
                    {"removeLinks", "Премахни връзките"},
                    {"insertLink", "Постави връзка"},
                    {"removeLinkFromSel", "Премахни връзките от избраното"},
                    {"saveXml", "Запиши текста в XML-file"},
                    {"settings", "Настройки"},
                } 
            },
            {   
                "it",
                new Dictionary<string, string>
                {
                    {"processText", "Process text"},
                    {"removeLinks", "Remove links"},
                    {"insertLink", "Insert link"},
                    {"removeLinkFromSel", "Remove link from selection"},
                    {"saveXml", "Save xml"},
                    {"settings", "Settings"},
                } 
            }
        };

        public static Dictionary<string, string> LanguageChangedMessage = new Dictionary<string, string>()
        {
            {"en", "Changes will take effect after restart of the toolbar or Internet Explorer."},
            {"fr", ""},
            {"de", ""},
            {"bg", "Промените ще се отразят след рестартиране на приставката или Internet Explorer."},
            {"it", ""}
        };


        public static string Text(string key, string lang)
        {
            switch(lang)
            {
                case "bg":
                    return TranslationBG.ResourceManager.GetString(key);
                    break;
                case "fr":
                    return TranslationFR.ResourceManager.GetString(key);
                    break;
                case "de":
                    return TranslationDE.ResourceManager.GetString(key);
                    break;
                case "it":
                    return TranslationIT.ResourceManager.GetString(key);
                    break;
            }

            return TranslationEN.ResourceManager.GetString(key);
        }
    }
}
