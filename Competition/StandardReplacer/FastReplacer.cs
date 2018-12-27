using System;
using System.Collections.Generic;
using System.Text;

namespace StandardReplacer
{
    /// <summary>
    /// FastReplacer is a utility class similar to StringBuilder, with fast Replace function.
    /// FastReplacer is limited to replacing only properly formatted tokens.
    /// Use ToString() function to get the final text.
    /// </summary>
    internal class FastReplacer
    {
        private readonly string _tokenOpen;
        private readonly string _tokenClose;
        private readonly FastReplacerSnippet _rootSnippet = new FastReplacerSnippet("");

        /// <summary>
        /// All tokens that will be replaced must have same opening and closing delimiters, such as "{" and "}".
        /// </summary>
        /// <param name="tokenOpen">Opening delimiter for tokens.</param>
        /// <param name="tokenClose">Closing delimiter for tokens.</param>
        /// <param name="caseSensitive">Set caseSensitive to false to use case-insensitive search when replacing tokens.</param>
        public FastReplacer(string tokenOpen, string tokenClose, bool caseSensitive = true)
        {
            if (string.IsNullOrEmpty(tokenOpen) || string.IsNullOrEmpty(tokenClose))
                throw new ArgumentException("Token must have opening and closing delimiters, such as \"{\" and \"}\".");

            _tokenOpen = tokenOpen;
            _tokenClose = tokenClose;

            var stringComparer = caseSensitive ? StringComparer.Ordinal : StringComparer.InvariantCultureIgnoreCase;
            OccurrencesOfToken = new Dictionary<string, List<TokenOccurrence>>(stringComparer);
        }

        

        public class TokenOccurrence
        {
            public FastReplacerSnippet Snippet;
            public int Start; // Position of a token in the snippet.
            public int End; // Position of a token in the snippet.
        }

        public readonly Dictionary<string, List<TokenOccurrence>> OccurrencesOfToken;

        public void Append(string text)
        {
            var snippet = new FastReplacerSnippet(text);
            _rootSnippet.Append(snippet);
            ExtractTokens(snippet);
        }

        /// <returns>Returns true if the token was found, false if nothing was replaced.</returns>
        public bool Replace(string token, string text)
        {
            List<TokenOccurrence> occurrences;
            if (OccurrencesOfToken.TryGetValue(token, out occurrences) && occurrences.Count > 0)
            {
                OccurrencesOfToken.Remove(token);
                var snippet = new FastReplacerSnippet(text);
                foreach (var occurrence in occurrences)
                    occurrence.Snippet.Replace(occurrence.Start, occurrence.End, snippet);
                ExtractTokens(snippet);
                return true;
            }
            return false;
        }

        private void ExtractTokens(FastReplacerSnippet snippet)
        {
            int last = 0;
            while (last < snippet.Text.Length)
            {
                // Find next token position in snippet.Text:
                int start = snippet.Text.IndexOf(_tokenOpen, last, StringComparison.Ordinal);
                if (start == -1)
                    return;
                int end = snippet.Text.IndexOf(_tokenClose, start + _tokenOpen.Length, StringComparison.Ordinal);
                if (end == -1) // closingTokenNotFound
                {
                    return;
                    //throw new ArgumentException(string.Format("Token is opened but not closed in text \"{0}\".", snippet.Text));
                }
                int eol = snippet.Text.IndexOf('\n', start + _tokenOpen.Length);
                if (eol != -1 && eol < end)
                {
                    last = eol + 1;
                    continue;
                }

                // Take the token from snippet.Text:
                end += _tokenClose.Length;
                string token = snippet.Text.Substring(start, end - start);
                string context = snippet.Text;

                // Add the token to the dictionary:
                var tokenOccurrence = new TokenOccurrence { Snippet = snippet, Start = start, End = end };
                List<TokenOccurrence> occurrences;
                if (OccurrencesOfToken.TryGetValue(token, out occurrences))
                    occurrences.Add(tokenOccurrence);
                else
                    OccurrencesOfToken.Add(token, new List<TokenOccurrence> { tokenOccurrence });

                last = end;
            }
        }

        private void ValidateToken(string token, string context, bool alreadyValidatedStartAndEnd)
        {
            if (!alreadyValidatedStartAndEnd)
            {
                if (!token.StartsWith(_tokenOpen, StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException(string.Format("Token \"{0}\" shoud start with \"{1}\". Used with text \"{2}\".", token, _tokenOpen, context));
                int closePosition = token.IndexOf(_tokenClose, StringComparison.InvariantCultureIgnoreCase);
                if (closePosition == -1)
                    throw new ArgumentException(string.Format("Token \"{0}\" should end with \"{1}\". Used with text \"{2}\".", token, _tokenClose, context));
                if (closePosition != token.Length - _tokenClose.Length)
                    throw new ArgumentException(string.Format("Token \"{0}\" is closed before the end of the token. Used with text \"{1}\".", token, context));
            }

            if (token.Length == _tokenOpen.Length + _tokenClose.Length)
                throw new ArgumentException(string.Format("Token has no body. Used with text \"{0}\".", context));
            //if (token.Contains("\n"))
            //    throw new ArgumentException(string.Format("Unexpected end-of-line within a token. Used with text \"{0}\".", context));
            //if (token.IndexOf(TokenOpen, TokenOpen.Length, StringComparison.InvariantCultureIgnoreCase) != -1)
            //{
            //    throw new ArgumentException(string.Format("Next token is opened before a previous token was closed in token \"{0}\". Used with text \"{1}\".", token, context));
            //}
        }

        public override string ToString()
        {
            int totalTextLength = _rootSnippet.GetLength();
            var sb = new StringBuilder(totalTextLength);
            _rootSnippet.ToString(sb);
            if (sb.Length != totalTextLength)
                throw new InvalidOperationException(string.Format(
                    "Internal error: Calculated total text length ({0}) is different from actual ({1}).",
                    totalTextLength, sb.Length));
            return sb.ToString();
        }
    }
}
