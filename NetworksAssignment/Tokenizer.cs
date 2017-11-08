using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Tokenize;

namespace NetworksAssignment
{
    class Tokenizer
    {
        /// <summary>
        /// Tokenizes a piece of text, and tags the tokens to indicate emphasis or something
        /// </summary>
        /// <param name="text">some text, probably a review</param>
        /// <returns>returns a list of tags with their respective tokens</returns>
        public static List<Tuple<string, List<string>>> TokenizeText(string text)
        {
            if (text == string.Empty) return null; //if the input is empty well shit
            EnglishRuleBasedTokenizer opennlpTokenizer = new EnglishRuleBasedTokenizer(false);
            List<Tuple<string, List<string>>> result = new List<Tuple<string, List<string>>>();
            string emphasisChars = "!";

            List<string> negationWords = new List<string>
            {
                "never",
                "no",
                "nothing",
                "nowhere",
                "noone",
                "none",
                "not",
                "havent",
                "hasnt",
                "hadnt",
                "cant",
                "couldnt",
                "shouldnt",
                "wont",
                "wouldnt",
                "dont",
                "dosent",
                "didnt",
                "isnt",
                "arent",
                "aint"
            };

            List<string> clausePunctuation = new List<string>
            {
                ".",
                ":",
                ";",
                "!",
                "?"
            };

            var intermediaryResult = opennlpTokenizer.Tokenize(text).ToList();
            if (intermediaryResult.Count == 0) return result; //we got nothing out of the tokenizer, dunno that its necessary to make this check on top of the list empty check

            //for some reason så efterlader tokenizeren "." og andre i slutningen af ord. Har kun set det med punktum, dont know why
            for (int i = 0; i < intermediaryResult.Count; i++)
            {
                string currentToken = intermediaryResult[i];

                if (currentToken.EndsWith(".") && currentToken.Length > 1)
                {
                    intermediaryResult[i] = intermediaryResult[i].Replace(".", "");//intermediaryResult[i].Remove(intermediaryResult[i].Length - 1);
                    intermediaryResult.Insert(i + 1, ".");
                }
            }

            bool negationTracker = false;
            for (int i = 0; i < intermediaryResult.Count; i++)
            {
                List<string> currentTokenTags = new List<string>();
                string currentToken = intermediaryResult[i];

                //handling emphasis

                if (currentToken.IndexOfAny(emphasisChars.ToCharArray()) != -1)
                {
                    //as suggested by the teacher we keep a max of 3 emphasis tokens. from there it kinda dosent matter anymore? or thats what he said
                    if (currentToken.Length > 3)
                    {
                        currentToken = currentToken.Substring(0, 3);
                        intermediaryResult[i] = currentToken;
                    }
                    for (int j = 0; j < currentToken.Length; j++)
                    {
                        currentTokenTags.Add("EMPH");


                        //ok now we need to add the EMPH tag to every token behind this one, until we meet a clausePunctuation
                        for (int k = i - 1; k > 0; k--)
                        {
                            if (clausePunctuation.Contains(intermediaryResult[k]))
                            {
                                break;
                            }
                            result[k].Item2.Add("EMPH");
                        }
                    }
                }
                else
                {
                    //handling negation
                    if (clausePunctuation.Contains(currentToken)) //we have reached a new sentence, time to reset the negationtracker
                    {
                        negationTracker = false;
                    }

                    if (negationWords.Contains(currentToken)) //is the current word a negator? if so we start to tag words with "NEG" here on out
                    {
                        negationTracker = true;
                    }

                    if (negationTracker) //aka a word somewhere earlier was negative
                    {
                        currentTokenTags.Add("NEG");
                    }
                }
                result.Add(Tuple.Create(currentToken, currentTokenTags));
            }
            return result;
        }
    }
}
