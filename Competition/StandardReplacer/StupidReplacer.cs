using System;
using System.Collections.Generic;
using System.Linq;

namespace StandardReplacer
{
    public static class StupidReplacer
    {
        public static string Replace(string pageContent, Func<string, string> replacementResolver)
        {
            if (pageContent == null) throw new ArgumentNullException(nameof(pageContent));
            var result = pageContent;

            
            var tokens = FindReplacementBlocks(result);
            if (tokens.Count > 0)
            {
                foreach (var token in tokens)
                {
                    result = result.Replace(token.Replace, replacementResolver(token.TokenValue));
                }
                tokens = FindReplacementBlocks(result);
                if (tokens.Count > 0)
                {
                    foreach (var token in tokens)
                    {
                        result = result.Replace(token.Replace, replacementResolver(token.TokenValue));
                    }

                    tokens = FindReplacementBlocks(result);
                    if (tokens.Count > 0)
                    {
                        foreach (var token in tokens)
                        {
                            result = result.Replace(token.Replace, replacementResolver(token.TokenValue));
                        }
                    }
                }
                
            }
            
            return result;
        }

        private class Token
        {
            public string Replace { get; }
            public string TokenValue { get; }

            public Token(string replace)
            {
                Replace = replace;
                TokenValue = replace.Substring(2, replace.Length - 4);
            }
        }

        private static List<Token> FindReplacementBlocks(string whereToSearch)
        {
            var result = new List<Token>();

            int currentPosition = 0;
            while (true)
            {
                var startIndex = whereToSearch.IndexOf("[$", currentPosition, StringComparison.Ordinal);
                if (startIndex < 0) break;
                var endIndex = whereToSearch.IndexOf("$]", startIndex, StringComparison.Ordinal);
                if (endIndex < 0) break;
                if (endIndex - startIndex > 2000) break; //подозрительно длинная подстрока

                var substring = whereToSearch.Substring(startIndex, endIndex - startIndex + 2);
                result.Add(new Token(substring));
                currentPosition = endIndex;
            }

            return result;
        }
    }
}