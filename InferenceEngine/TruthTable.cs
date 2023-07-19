using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace InferenceEngine
{
    public class TruthTable
    {
        public KnowledgeBase kb;
        public int tt_size;
        public bool[,] truth_table;
        public string query;

        // List to hold each boolean value of KB
        List<bool> kbEvaluated = new List<bool>();
        public TruthTable(KnowledgeBase kbase, string Query)
        {
            kb = kbase;
            // size of truth table
            tt_size = (int)Math.Pow(2, kb.symbolCount);
            // each symbol in its own array with all truths
            truth_table = new bool[tt_size, kb.symbolCount];
            // List of results from KB
            kbEvaluated = new List<bool>();

            query = Query;

            // creates truth table
            Initilise_Truth_Table();


            EvaluateKB();
        }
        public string[] postfixConverter()
        {
            List<string> result = new List<string>();
            for(int i = 0; i < kb.kbAtomicSentence.Count(); i++)
            {
                if (kb.kbAtomicSentence[i] != "=>" && kb.kbAtomicSentence[i] != "<=>" && kb.kbAtomicSentence[i] != "&" && kb.kbAtomicSentence[i] != "||")
                {
                    result.Add(kb.kbAtomicSentence[i]);
                }
                // keeps the ';' in sentences
                else if (kb.kbAtomicSentence[i] == ";")
                {
                    result.Add(kb.kbAtomicSentence[i]);
                }
                else
                {
                    result.Add(kb.kbAtomicSentence[i + 1]);
                    result.Add(kb.kbAtomicSentence[i]);
                    i++;
                }
            }
            return result.ToArray();
        }
        public void EvaluateKB()
        {
            // convertes infix sentences to postfix
            string[] postfixSentence = postfixConverter();

            // List which will evaluate each sentence
            Stack<bool> postfixStack = new Stack<bool>();

            for (int i = 0; i < tt_size; i++)
            {
                foreach (string token in postfixSentence)
                {
                    if (token == ";")
                    {
                        continue;                  
                    }
                    // "<=>", "||" are not needed for horn form
                    else if (token != "=>" && token != "<=>" && token != "&" && token != "||")
                    {
                        bool value = truth_table[i, kb.searchedList.IndexOf(token)];
                        postfixStack.Push(value);
                    }
                    else if (token == "=>")
                    {
                        bool b = postfixStack.Pop();
                        bool a = postfixStack.Pop();
                        bool result = INFERS(a, b);
                        postfixStack.Push(result);
                    }
                    else if (token == "&")
                    {
                        bool b = postfixStack.Pop();
                        bool a = postfixStack.Pop();
                        bool result = AND(a, b);
                        postfixStack.Push(result);
                    }
                    else if (token == "<=>")
                    {
                        bool b = postfixStack.Pop();
                        bool a = postfixStack.Pop();
                        bool result = BICONDITIONAL(a, b);
                        postfixStack.Push(result);
                    }
                    else if (token == "||")
                    {
                        bool b = postfixStack.Pop();
                        bool a = postfixStack.Pop();
                        bool result = OR(a, b);
                        postfixStack.Push(result);
                    }
                }
                // & each value in List and stores it into variable
                bool evaluatedResult = true;
                List<bool> postfixList = postfixStack.ToList();
                for(int j = 0; j < postfixList.Count; j++)
                {
                    evaluatedResult = AND(evaluatedResult, postfixList[j]);
                }

                kbEvaluated.Add(evaluatedResult);
                
                // clear stack for next iteration
                postfixStack.Clear();
            }

            // Gives specified output to console
            Console.WriteLine(entailsOutput());
        }

        public string entailsOutput()
        {
            int count = 0;
            string result = "NO";
            bool[] queryColumn = new bool[tt_size];

            // finds query column and increments count based on whether kb entails query
            try
            {
                for (int i = 0; i < tt_size; i++)
                {
                    queryColumn[i] = truth_table[i, kb.searchedList.IndexOf(query)];

                    if (kbEvaluated[i] && !AND(queryColumn[i], kbEvaluated[i]))
                    {
                        count = 0;
                        break;
                    }
                    else if (kbEvaluated[i] && AND(queryColumn[i], kbEvaluated[i]))
                    {
                        count++;
                    }

                }
            }
            catch
            {
                Console.WriteLine("NO");
                Environment.Exit(1);
            }

            // if every instance of KB = false or KB is true while query is false, then KB does not entail query
            if(count == 0)
            {
                return result;
            }
            else
            {
                result = "YES: " + count.ToString();
                return result;
            }
        }
        public void Initilise_Truth_Table()
        {
            for (int j = 0; j < tt_size; j++)
            {
                // puts 0 at start of binary version of the row number
                string binRowNum = Convert.ToString(j, 2).PadLeft(kb.symbolCount, '0');

                bool[] bool_arr = new bool[kb.symbolCount];
                // loops over each tempBool value, if 0 its false, if 1 its true
                // then stores boolean value into truth_table
                for (int i = 0; i < kb.symbolCount; i++)
                {
                    bool_arr[i] = (binRowNum[i] == '1');
                    truth_table[j, i] = bool_arr[i];
                }
            }

            // print table

            /*foreach(string symbol in kb.searchedList)
            {
                Console.Write(symbol + "\t");
                Console.Write("");
            }
            Console.WriteLine();


            for (int i = 0; i < tt_size; i++)
            {
                for (int j = 0; j < kb.symbolCount; j++)
                {
                    Console.Write(truth_table[i, j] + "\t");
                }
                Console.WriteLine();
            }*/
        }



        // operations
        public bool AND(bool a, bool b)
        {
            return a && b;
        }

        public bool OR(bool a, bool b)
        {
            return a || b;
        }

        public bool BICONDITIONAL(bool a, bool b)
        {
            return a == b;
        }

        public bool INFERS(bool a, bool b)
        {
            if(!a)
            {
                return true;
            }
            else if (b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}