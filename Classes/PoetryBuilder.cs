using System;
using System.Collections.Generic;
using System.Text;
using PoetryGenerator.Interfaces;

namespace PoetryGenerator.Classes
{
    /// <summary>
    /// PoetryBuilder -- Class responsible for building poem
    /// </summary>
    public class PoetryBuilder
    {
        private readonly Dictionary<string, PoemRuleDefinition> poemRuleDefinitons;
        private readonly PoemRuleDefinition poemRuleDefinition;
        
        public List<string> Words { get; set; } = new List<string>();  // Used to output poem
        public List<string> Rules { get; set; } = new List<string>();  // Used to trace execution tree of application rules to avoid an infinite loop/stack overflow

        /// <summary>
        /// PoetryBuilder -- constructor for class that utilizes dependency injection
        /// </summary></param>
        /// <param name="poemRuleDefinitions">the collection storing all poem rule definitions</param>
        public PoetryBuilder(Dictionary<string, PoemRuleDefinition> poemRuleDefinitions)
        {
            poemRuleDefinitons = poemRuleDefinitions;
            poemRuleDefinition = poemRuleDefinitons["POEM"];
        }

        /// <summary>
        /// BuildPoem -- Builds a randomly generated poem
        /// </summary>
        public void BuildPoem()
        {
            foreach (string rule in poemRuleDefinition.Rules)
            {
                GeneratePoetryLine(rule);
                Rules.Add(rule);

                PoemRuleDefinition workingPoetryRuleNew = poemRuleDefinitons[rule];

                if (workingPoetryRuleNew.LineFeed)
                {
                    Console.Write("\r\n");
                    Console.Out.Flush();
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        /// GeneratePoetryLine -- A recursive routine to build a randomly-generated poem based on provided business rules
        /// </summary>
        /// <param name="rule">text of poetry rule</param>
        public void GeneratePoetryLine(string rule)
        {
            try
            {
                poemRuleDefinitons.TryGetValue(rule, out PoemRuleDefinition workingPoetryRule);

                // Randomly select new poetry rule
                int randomRulesIndex = Utility.GenerateRandomNumberForRuleSelection(workingPoetryRule.Rules.Count);
                string newRule = workingPoetryRule.Rules[randomRulesIndex];
                Rules.Add(newRule);

                int randomWordsIndex = 0;
                string newWord = string.Empty;

               if (newRule.Equals("$END"))
               {
                    Console.Write(" ");
                    Console.Out.Flush();
                    return;
               }

                // Get the new rule
                PoemRuleDefinition workingPoetryRuleNew = poemRuleDefinitons[newRule];

                if (workingPoetryRuleNew.Words.Count != 0)
                {
                    randomWordsIndex = Utility.GenerateRandomNumberForWordSelection(workingPoetryRuleNew.Words.Count);
                    newWord = workingPoetryRuleNew.Words[randomWordsIndex];
                    Words.Add(newWord);
                }

                Console.Write(newWord);
                Console.Write(" ");
                Console.Out.Flush();
                GeneratePoetryLine(newRule);
           }
            catch (Exception ex)
            {
                // Log exception
                Program.Log.Error(ex.Message);
            }
        }
    }
}