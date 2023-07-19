using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace InferenceEngine
{
    public class KnowledgeBase
    {
        public int symbolCount;
        // public string[] kb;
        public ArrayList searchedList = new ArrayList();
        public string[] knowledgeBaseStrings;

        public string[] kbAtomicSentence;

        // constructor
        public KnowledgeBase(string kb)
        {
            symbolCount = CountSymbols(kb);
            knowledgeBaseStrings = sentenceSplitter(kb);

            kbAtomicSentence = atomicSentenceSplitter(kb);
        }

        // other methods
        public int CountSymbols(string kb)
        {
            // split at symbol
            string[] pattern = { "=>", "~", "&", "||", "<=>", ";", " " };
            ArrayList symbolArray = new ArrayList(kb.Split(pattern, StringSplitOptions.RemoveEmptyEntries));

            foreach (string s in symbolArray)
            {
                if (!searchedList.Contains(s))
                {
                    searchedList.Add(s);
                }

            }

            // returns count of symbols which have not repeated
            return searchedList.Count;

        }
        public string[] sentenceSplitter(string kb)
        {
            return kb.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] atomicSentenceSplitter(string kb)
        {
            string pattern = @"([A-Za-z]\d?)";
            Regex regex = new Regex(pattern);

            // When split, there is still whitespace
            string[] untrimmed = regex.Split(kb);

            List<string> trimmed = new List<string>();

            // Loops over elements and removes whitespace
            foreach (string s in untrimmed)
            {
                if (s.Trim() == "" || s.Trim() == " ")
                {
                    continue;
                }
                else
                {
                    trimmed.Add(s.Trim());
                }
            }

            // returns trimmed List as a string[]
            return trimmed.ToArray();
        }
    }
}