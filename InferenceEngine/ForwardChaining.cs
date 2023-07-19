using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    public class ForwardChaining
    {
        public KnowledgeBase kb;
        public string query;
        public List<string> queue;
        public List<List<string>> LRpairs;
        public ForwardChaining(KnowledgeBase KB, string QUERY)
        {
            kb = KB;
            query = QUERY;

            queue = new List<string>();

            LRpairs = new List<List<string>>();

            InitialiseFacts();
        }


        public void InitialiseFacts()
        {
            // adds all individual boolean values to queue
            foreach (string sentence in kb.knowledgeBaseStrings)
            {
                if (sentence.Trim().Count() <= 2)
                {
                    queue.Add(sentence.Trim());
                }
                else if (sentence.Trim().Contains("=>"))
                {
                    string[] result = sentence.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);

                    LRpairs.Add(new List<string> { result[0], result[1] });
                }
            }
            // if the queue does not have any individual boolean values it terminates
            if (queue.Any())
            {
                testQuery();
            }
            else
            {
                Console.WriteLine("NO");
                return;
            }

        }

        public void testQuery()
        {
        // pair[0] = Left side
        // pair[1] = Right side
        restart:
            int count = 0;
            foreach (List<string> pair in LRpairs)
            {
                string[] result = pair[0].Trim().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                // if all elements that are in result are in the queue
                if (result.All(r => queue.Contains(r.Trim())))
                {
                    // value = the value in LRpairs which has the same key as before
                    string[] value = pair[1].Trim().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string v in value)
                    {
                        if (!queue.Contains(v.Trim()))
                        {
                            queue.Add(v.Trim());
                            count++;
                        }
                        // if query is in queue, prints out Yes with queue
                        if (queue.Contains(query))
                        {
                            string queryToString = "";
                            for (int i = 0; i < queue.Count; i++)
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
    }
}