using System;
using System.IO;
using System.Collections;
using InferenceEngine;
using System.Linq.Expressions;

class Program

{
    static void Main(string[] args)
    {
        //reads file
        StreamReader reader = new StreamReader(args[1]);

        // assigns each line to variable
        string tell = reader.ReadLine();
        string kb = reader.ReadLine();
        string ask = reader.ReadLine();
        string query = reader.ReadLine();

        if (query == null)
        {
            try
            {
                throw new Exception();
            }
            catch
            {
                Console.WriteLine("No query was specified");
                Environment.Exit(1);
            }
        }

        // reads line of clauses into KnowledgeBase
        KnowledgeBase KB = new KnowledgeBase(kb);

        switch (args[0].ToLower())
        {
            case "tt":
                TruthTable tt = new TruthTable(KB, query);
                break;
            case "bc":
                BackwardChaining bc = new BackwardChaining(KB, query);
                break;
            case "fc":
                ForwardChaining fc = new ForwardChaining(KB, query);
                break;
            default:
                Console.WriteLine("Error: Invalid Input");
                break;
        }
        


        reader.Close();
    }
}



