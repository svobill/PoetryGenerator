using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using PoetryGenerator.Classes;

namespace PoetryGenerator
{
    /// <summary>
    /// Program -- Entry point class for C# console application
    /// </summary>
    public static class Program
    {
        public static Logger Log { get; set; }
        public readonly static string poemItemTextNotFound = "Text not found (POEM:).";
        public readonly static string poemKeyTextNotFound = "Key not found (POEM).";
        public readonly static string crlfTextNotFound = "Text not found (CrLf).";
        public readonly static string splitFailed = "Splitting poem rule definition on the file line has failed.";

        public static PoemRuleDefinition PoemRuleDefinition { get; set; } = new PoemRuleDefinition();
        public static PoetryBuilder PoetryBuilder { get; set; }
        public static Dictionary<string, PoemRuleDefinition> PoemRuleDefinitions { get; set; } = new Dictionary<string, PoemRuleDefinition>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                Log = LogManager.GetCurrentClassLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                byte[] configFileInfo = Utility.ReadPoertyConfigurationFile(args[0]);
                string configFileData = Encoding.Default.GetString(configFileInfo);

                // Replace all CrLfs and newlines in the file with blanks as these may place an unexpected CrLf in the poem
                // Trim the text if there are unexpected spaces at the beginning of each Poem Rule
                configFileData = configFileData.Replace("\r\n", " ").Trim();

                // Search configuration data file and get definition of POEM business rule from any location from within the file
                // The POEM rule defines the layout of the random words to follow
                int startPoemLocation = configFileData.IndexOf("POEM:");

                // Check for missing POEM: keyword or empty file
                if (startPoemLocation < 0 || string.IsNullOrWhiteSpace(configFileData))
                {
                    throw new ArgumentOutOfRangeException(poemItemTextNotFound);
                }

                bool quitLoop = false;
                string poemRuleSeparator = " ";
                string poemRuleDefinitionSeparator = "|";

                int startLineLocation = configFileData.IndexOf(':', 0) + 1; // Advance start to blank after the colon on the first poem rule definition
                int endLineLocation = configFileData.IndexOf(':', startLineLocation);

                int startNewLineKeywordLocation = 0;
                int endNewLineKeywordLocation = configFileData.IndexOf(':', 0);

                string ruleType = configFileData.Substring(0, endNewLineKeywordLocation).Trim();

                while (!quitLoop)
                {
                    // Rule line is in the bottom of the file, so we set the end line location to the last character position in the file
                    if (endLineLocation == -1)
                    {
                        quitLoop = true;
                        endLineLocation = configFileData.Length;
                    }
                    else
                    {
                        // Search backwards to the first blank just before the next rule name
                        for (int i = endLineLocation; i > startLineLocation; i--)
                        {
                            if (i == 0 || configFileData.Substring(i, 1).Equals(" "))
                            {
                                endLineLocation = i;
                                break;
                            }
                        }
                    }

                    // Find the business rule for the object - trim leading and trailing blanks, and split the text on a blank
                    string poemRule = configFileData.Substring(startLineLocation, endLineLocation - startLineLocation).Trim();

                    // Check for $END or $LINEBREAK for the rule
                    if (poemRule.Contains(" $END"))
                    {
                        PoemRuleDefinition.EndOfLine = true;
                        poemRule = poemRule.Replace("$END", string.Empty);
                    }
                    else
                    {
                        PoemRuleDefinition.EndOfLine = false;
                    }

                    if (poemRule.Contains(" $LINEBREAK"))
                    {
                        PoemRuleDefinition.LineFeed = true;
                        poemRule = poemRule.Replace("$LINEBREAK", string.Empty);
                    }
                    else
                    {
                        PoemRuleDefinition.LineFeed = false;
                    }

                    poemRule = poemRule.Replace("<", string.Empty).Replace(">", string.Empty).Trim();

                    string wordList;
                    string ruleList;

                    if (Utility.BlankCount(poemRule) > 1 && !poemRuleDefinitionSeparator.Equals(" "))
                    {
                        // Remove excess blanks from string -- should be one or zero
                        // If all uppercase letters and piping character -- no blanks
                        int upperCaseStartLocation = Utility.FindThreeJoinedUppercaseCharacters(poemRule);
                        int blankSeparatorLocation = 0;

                        ruleList = poemRule[upperCaseStartLocation..].Trim();

                        if (upperCaseStartLocation > 0)
                        {
                            blankSeparatorLocation = upperCaseStartLocation - 1;
                            wordList = poemRule.Substring(0, blankSeparatorLocation).Trim();
                        }
                        else
                        {
                            blankSeparatorLocation = -1;
                            wordList = string.Empty;
                        }

                        if (blankSeparatorLocation > -1)
                        {
                            wordList = wordList.Replace(" ", string.Empty);
                        }
                    }
                    else if (Utility.BlankCount(poemRule) == 1)
                    {
                        int blankLocation = poemRule.IndexOf(" ");
                        ruleList = poemRule.Substring(blankLocation + 1);
                        wordList = poemRule.Substring(0, blankLocation);
                    }
                    else
                    {
                        ruleList = poemRule;
                        wordList = string.Empty;
                    }

                    // The first part of the if statement defines the POEM: line
                    if (!poemRule.Contains('|'))
                    {
                        PoemRuleDefinition.PoemRuleName = ruleType;

                        PoemRuleDefinition.SetBusinessRule(poemRule, poemRuleSeparator);

                        // Add the poem rule definition collection to the Dictionary object
                        PoemRuleDefinitions.Add(PoemRuleDefinition.PoemRuleName, PoemRuleDefinition);
                    }
                    else  // This defines the poem rule definition lines that don't contain words as well as those that do
                    {
                        if (wordList.Length == 0 && ruleList.Length > 0) // A LINE type rule containing business rules and no words
                        {
                            PoemRuleDefinition.SetBusinessRule(ruleList, poemRuleDefinitionSeparator);  
                        }
                        else if (wordList.Length > 0 && ruleList.Length > 0) // Words and business rules
                        {
                            PoemRuleDefinition.SetWordsForType(wordList, poemRuleDefinitionSeparator);
                            PoemRuleDefinition.SetBusinessRule(ruleList, poemRuleDefinitionSeparator);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(splitFailed);
                        }

                        ruleType = configFileData.Substring(startNewLineKeywordLocation, endNewLineKeywordLocation - startNewLineKeywordLocation).Trim();
                        PoemRuleDefinition.PoemRuleName = ruleType;

                        // Add the poem rule definition collection to the Dictionary object
                        PoemRuleDefinitions.Add(PoemRuleDefinition.PoemRuleName, PoemRuleDefinition);
                    }

                    // Advance the character pointers in the file text
                    startNewLineKeywordLocation = endLineLocation + 1;
                    endNewLineKeywordLocation = configFileData.IndexOf(':', endLineLocation);

                    startLineLocation = configFileData.IndexOf(':', endLineLocation) + 1;
                    endLineLocation = configFileData.IndexOf(':', startLineLocation);

                    PoemRuleDefinition = new PoemRuleDefinition();
                }

                PoemRuleDefinition poemRuleDefinition;

                // Now generate the poem using poetry rules
                if (PoemRuleDefinitions.ContainsKey("POEM"))
                {
                    PoemRuleDefinitions.TryGetValue("POEM", out poemRuleDefinition);
                }
                else
                {
                    throw new KeyNotFoundException(poemKeyTextNotFound);
                }

                PoetryBuilder = new PoetryBuilder(PoemRuleDefinitions);
                PoetryBuilder.BuildPoem();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}