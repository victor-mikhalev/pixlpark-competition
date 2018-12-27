using System;
using System.Collections.Generic;
using System.Linq;

namespace StandardReplacer
{
    public static class Replacer
    {
        public static string Replace(string pageContent, Func<string, string> replacementResolver)
        {
            if (pageContent == null) throw new ArgumentNullException(nameof(pageContent));
            var result = pageContent;

            var fr = new FastReplacer("[$", "$]");
            fr.Append(result);

            var tokens = fr.OccurrencesOfToken;
            if (tokens.Count > 0)
            {
                var currentTokens = fr.OccurrencesOfToken.Keys.ToList();
                ReplaceToken(fr, currentTokens, replacementResolver);
                if (tokens.Count > 0)
                {
                    ReplaceToken(fr, fr.OccurrencesOfToken.Keys.ToList(), replacementResolver);
                    if (tokens.Count > 0)
                    {
                        ReplaceToken(fr, fr.OccurrencesOfToken.Keys.ToList(), replacementResolver);
                    }
                }
            }
            
            return fr.ToString();
        }

        private static void ReplaceToken(FastReplacer fr, List<string> tokens, Func<string, string> replacementResolver)
        {
            for (var index = 0; index < tokens.Count; index++)
            {
                var token = tokens[index];
                var replacementCode = token.Substring(2, token.Length - 4); // внутрянка без [$ $]
                var replacement = replacementResolver(replacementCode);
                if (replacement != null)
                {
                    fr.Replace(token, replacement);
                }
            }
        }
    }
}
