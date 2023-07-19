using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    public class BackwardChaining
    {
        public KnowledgeBase kb;
        public string query;

        public List<string> stringToSearch;
        public List<string> queue;
        public List<List<string>> LRpairs;
        public BackwardChaining(KnowledgeBase KB, string QUERY)
        {
            kb = KB;
            query = QUERY;

            stringToSearch = new List<string>();
            // the query is in the queue by default
            queue = new List<string> { query };
            LRpairs = new List<List<string>>();

            establishGoal();
        }

        // individual boolean values are the goal of the program and are stored in stringToSearch
        public void establishGoal()
        {
            foreach (string sentence in kb.knowledgeBaseStrings)
            {
                if (sentence.Trim().Count() <= 2)
                {
                    stringToSearch.Add(sentence.Trim());
                }
                else if (sentence.Trim().Contains("=>"))
                {
                    string[] result = sentence.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);

                    LRpairs.Add(new List<string> { result[0], result[1] });
                }
            }
            // if the queue does not have any individual boolean values it terminates
            if (stringToSearch.Any())
            {
                startSearch();
            }
            else
            {
                Console.WriteLine("NO");
                return;
            }
        }

        public void startSearch()
        {
        // pair[0] = Left side
        // pair[1] = Right side
        restart:
            int count = 0;
            foreach (List<string> pair in LRpairs)
            {
                string[] value = pair[1].Trim().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                // if all elements that are on the right side are in the queue
                if (value.All(v => queue.Contains(v)))
                {
                    // splits the left side and adds it to the queue
                    string[] result = pair[0].Trim().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string r in result)
                    {
                        if (!queue.Contains(r.Trim()))
                        {
                            queue.Add(r.Trim());
                            count++;
                        }
                        //checks whether goal values are in the queue, if they are, it writes queue backwards, then terminates
                        if (verifySearch())
                        {
                            string queryToString = "";
                            for (int i = queue.Count - 1; i >= 0; i--)
                            {
                                queryToString += queue[i] + " ";
                            }
                            Console.WriteLine("YES: " + queryToString);
                            return;
                        }
                    }

                }
            }

            // this restarts the loop from the start only if something was added, if not, it breaks
            if (count > 0)
            {
                goto restart;
            }
            // if each pair has been checked and result was not found
            Console.WriteLine("NO");
        }

        public bool verifySearch()
        {
            foreach (string s in queue)
            {
                if (stringToSearch.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}