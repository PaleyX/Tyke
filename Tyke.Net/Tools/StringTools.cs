using System;

namespace Tyke.Net.Tools
{
    internal static class StringTools
    {
        /// <summary>
        /// Returns a positive non zero number - or zero if an error
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int GetPositiveNumber(string value)
        {
            if (!int.TryParse(value, out var result))
                result = 0;

            if (result <= 0)
            {
                Errors.Error.ReportError("Positive non zero number expected");
                result = 0;
            }

            return result;
        }

        internal static string GetQuotedString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Errors.Error.SyntaxError("Expected quoted string");
                return string.Empty;
            }

            if (text.StartsWith("\"") && text.EndsWith("\""))
            {
                return text.Substring(1, text.Length - 2);
            }
            else
            {
                Errors.Error.SyntaxError("Expected quoted string");
                return string.Empty;
            }
        }

        internal static bool IsQuotedString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.Length > 1 && text.StartsWith("\"") && text.EndsWith("\"");
        }

        internal static bool CanConvert<T>(string s) where T : struct
        {
            // some standard checks first to avoid exception throwing of ChangeType
            if (typeof(T) == typeof(uint))
            {
                uint result;
                return uint.TryParse(s, out result);
            }

            if (typeof(T) == typeof(ushort))
            {
                ushort result;
                return ushort.TryParse(s, out result);
            }

            try
            {
                var x = (T)System.Convert.ChangeType(s, typeof(T));
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal static T Convert<T>(string s, bool showError = true) where T : struct
        {
            T result = default(T);

            try
            {
                result = (T)System.Convert.ChangeType(s, typeof(T));
            }
            catch (Exception e)
            {
                if(showError)
                    Errors.Error.ReportError("Cannot convert '{0}' to {1}: {2}", s, typeof(T).ToString(), e.Message);
            }

            return result;
        }

        internal static Tuple<string, string> SplitOnEquals(string line, bool reportError = true)
        {
            // split on '='
            int pos = line.IndexOf("=");
            if (pos < 0)
            {
                if(reportError)
                    Errors.Error.ReportError("Syntax error, expected '='");
                return null;
            }

            string lhs = line.Substring(0, pos).Trim();
            string rhs = line.Substring(pos + 1).Trim();

            return new Tuple<string, string>(lhs, rhs);
        }

        internal static bool ValidateNonEmpty(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Errors.Error.SyntaxError("Cannot be empty");
                return false;
            }

            return true;
        }

        internal static bool ValidateOneWord(string text)
        {
            if (!ValidateNonEmpty(text))
                return false;

            if (text.Contains(' '))
            {
                Errors.Error.SyntaxError("Only one word allowed");
                return false;
            }

            return true;
        }

        internal static bool GetBoolean(string value)
        {
            switch (value)
            {
                case "true":
                case "yes":
                    return true;
                case "false":
                case "no":
                    return false;
            }

            Errors.Error.SyntaxError("Must be a boolean literal");
            return false;
        }
    }
}
