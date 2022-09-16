using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using NLog.Fluent;
using PoetryGenerator;

namespace PoetryGenerator.Classes
{
    public static class Utility
    {
        /// <summary>
        /// ReadPoertyConfigurationFile -- Reads the data from the poetry configuration file using the C# FileStream class
        /// </summary>
        /// <param name="pathAndFileName">Path and file name of poetry configuration file to be read</param>
        /// <returns>Byte array of character </returns>
        public static byte[] ReadPoertyConfigurationFile(string pathAndFileName)
        {
            int countBytesRead = 0;
            int bufferOffset = 0;

            try
            {
                using (FileStream fileStream = new FileStream(pathAndFileName, FileMode.Open, FileAccess.Read))
                {
                    int fileLengthInBytes = (int)fileStream.Length;
                    byte [] buffer = new byte[fileLengthInBytes];

                    // Read until the Read method returns zero - no more text remaining
                    while ((countBytesRead = fileStream.Read(buffer, bufferOffset, (fileLengthInBytes - bufferOffset))) > 0)
                    {
                        bufferOffset += countBytesRead;  // bufferOffset represents the total bytes read so far
                    }

                    return buffer;
                }
            }
            catch (ArgumentException except)
            {
                // Log exception to error log file
                // Send values countBytesRead and bufferOffset to log file
                string message = except.Message + "; countBytesRead=" + countBytesRead.ToString() + "; bufferOffset=" + bufferOffset.ToString();
                Program.Log.Error(message);

                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                // Log exception
                Program.Log.Error(ex.Message);
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// GenerateRandomNumberForWordSelection -- Generates a random number between one and the number of words (minus one) assigned to the poetry rule
        /// </summary>
        /// <param name="wordCount">count of words assigned to poetry rule</param>
        /// <returns></returns>
        public static int GenerateRandomNumberForWordSelection(int wordCount)
        {
            Random number = new Random();
            return number.Next(1, wordCount);
        }

        /// <summary>
        /// GenerateRandomNumberForRuleSelection -- Generates a random number between one and the number of rules assigned to the poetry rule
        /// </summary>
        /// <param name="ruleCount">count of words assigned to poetry rule</param>
        /// <returns></returns>
        public static int GenerateRandomNumberForRuleSelection(int ruleCount)
        {
            Random number = new Random();
            return number.Next(1, ruleCount);
        }

        /// <summary>
        /// FindThreeJoinedUppercaseCharacters -- Finds three contiguous upper case characters that represent the start of a business rule for constructing poetry
        /// </summary>
        /// <param name="inputString">search string</param>
        /// <returns>Location in string where three upper-case contiguous characters first appear</returns>
        public static int FindThreeJoinedUppercaseCharacters(string inputString)
        {
            string pattern = @"[A-Z]{3}";
            Regex regex = new Regex(pattern);

            Match match = regex.Match(inputString);

            return inputString.IndexOf(match.ToString());
        }

        /// <summary>
        /// BlankCount -- counts blanks or spaces in a string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int BlankCount(string inputString)
        {
            int blankTotal = 0;

            for (int i = 0; i < inputString.Length; i++)
            {
                string placeholder = inputString.Substring(i, 1);

                if (placeholder.Equals(" "))
                {
                    blankTotal++;
                }
            }

            return blankTotal;
        }
    }
}