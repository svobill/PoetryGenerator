using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoetryGenerator.Interfaces;

namespace PoetryGenerator.Classes
{
    /// <summary>
    /// Poem Rule Definition -- Class that describes a poem rule
    /// </summary>
    public class PoemRuleDefinition : IPoemRuleDefinition
    {
        public string PoemRuleName { get; set; } = string.Empty;
        public bool EndOfLine { get; set; } = false;
        public bool LineFeed { get; set; } = false;
        public List<string> Rules { get; set; } = new List<string>();
        public List<string> Words { get; set; } = new List<string>();

        /// <summary>
        /// SetWordsForType -- Extracts pipe-delimited word list for poem rule type.
        /// </summary>
        /// <param name="wordList">string or words to be used in poem separated by a common character</param>
        /// <param name="separator">common character used to separate poetry words</param>
        /// <returns></returns>
        public List<string> SetWordsForType(string wordList, string separator = "|")
        {
            try
            {
                // Remove any remaining embedded blanks from CrLf or newline replacement
                if (!separator.Equals(" ") && wordList.Contains(" "))
                {
                    wordList = wordList.Remove(' ');
                }

                List<string> listWordsTemp = wordList.Split(separator).ToList<string>();
                List<string> listWords = new List<string>();

                // Clean-up excess blanks (shouldn't exists but file may have typo) - this code is fine for List<string> with only a few items
                foreach (string item in listWordsTemp)
                {
                    listWords.Add(item.Trim());
                }

                this.Words = listWords;

                return listWords;
            }
            catch (Exception ex)
            {
                // Log exception
                Program.Log.Error(ex.Message);
                return new List<string>();
            }
        }

        /// <summary>
        /// SetBusinessRule -- Extracts the poem or poem type business rule
        /// </summary>
        /// <param name="ruleList">string of rules separated by a common character</param>
        /// <param name="separator">common character used to separate rules</param>
        /// <returns></returns>
        public List<string> SetBusinessRule(string ruleList, string separator = "|")
        {
            try
            {
                // Remove any remaining embedded blanks from CrLf or newline replacement
                if (!separator.Equals(" ") && ruleList.Contains(" "))
                {
                    ruleList = ruleList.Replace(" ", string.Empty);
                }

                List<string> businessRulesTemp = ruleList.Split(separator).ToList<string>();
                List<string> businessRules = new List<string>();

                // Clean-up excess blanks - this code is fine for List<string> with only a few items
                // ListItems should contain a leading '<' and a trailing '>' for all poem item business rules
                foreach (string item in businessRulesTemp)
                {
                    businessRules.Add(item.Trim().Trim('<').Trim('>'));
                }

                this.Rules = businessRules;

                return businessRules;
            }
            catch (Exception ex)
            {
                // Log exception
                Program.Log.Error(ex.Message);
                return new List<string>();
            }
        }
    }
}