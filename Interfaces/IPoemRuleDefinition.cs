using System;
using System.Collections.Generic;
using System.Text;

namespace PoetryGenerator.Interfaces
{
    /// <summary>
    /// IPoemRuleDefinition -- Interface for poem rule definition
    /// </summary>
    public interface IPoemRuleDefinition
    {
        public string PoemRuleName { get; set; }
        public bool EndOfLine { get; set; }
        public bool LineFeed { get; set; }
        public List<string> Rules { get; }
        public List<string> Words { get; }

        public List<string> SetWordsForType(string wordList, string separator = "|");
        public List<string> SetBusinessRule(string ruleList, string separator = "|");
    }
}