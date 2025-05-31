using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace WinFormsApp1.Localization
{
    public static class LocalizationManager
    {
        private static ResourceManager _resourceManager;
        private static CultureInfo _currentCulture;

        static LocalizationManager()
        {
            _resourceManager = new ResourceManager("WinFormsApp1.Localization.GameResources", typeof(LocalizationManager).Assembly);
            _currentCulture = CultureInfo.CurrentCulture;
        }

        public static string GetString(string key)
        {
            try
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                return value ?? $"[Missing: {key}]";
            }
            catch
            {
                return $"[Error: {key}]";
            }
        }

        public static string GetString(string key, params object[] args)
        {
            try
            {
                var format = GetString(key);
                return string.Format(format, args);
            }
            catch
            {
                return $"[Format Error: {key}]";
            }
        }

        public static void SetCulture(string cultureName)
        {
            try
            {
                var culture = new CultureInfo(cultureName);
                SetCulture(culture);
            }
            catch (CultureNotFoundException)
            {
                // Fallback to English if culture not supported
                SetCulture(new CultureInfo("en-US"));
            }
        }

        public static void SetCulture(CultureInfo culture)
        {
            _currentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static CultureInfo GetCurrentCulture()
        {
            return _currentCulture;
        }

        public static string[] GetSupportedCultures()
        {
            return new string[]
            {
                "en-US", // English (United States)
                "zh-CN", // Chinese (Simplified, China)
                "zh-TW", // Chinese (Traditional, Taiwan)
                "ja-JP", // Japanese
                "ko-KR", // Korean
                "es-ES", // Spanish
                "fr-FR", // French
                "de-DE", // German
                "ru-RU"  // Russian
            };
        }

        public static string GetCultureDisplayName(string cultureName)
        {
            try
            {
                var culture = new CultureInfo(cultureName);
                return culture.NativeName;
            }
            catch
            {
                return cultureName;
            }
        }

        public static bool IsRightToLeft()
        {
            return _currentCulture.TextInfo.IsRightToLeft;
        }

        public static string FormatNumber(int number)
        {
            return number.ToString("N0", _currentCulture);
        }

        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", _currentCulture);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("d", _currentCulture);
        }

        public static string FormatTime(DateTime time)
        {
            return time.ToString("t", _currentCulture);
        }
    }
} 