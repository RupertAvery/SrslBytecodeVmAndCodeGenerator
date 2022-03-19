﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Srsl_Parser;
using Srsl_Parser.Runtime;

namespace SrslInterpreterBacktrackingMemoizingParserBased
{

public class Program
{
    #region Public

    public static void Main( string[] args )
    {

        SrslParser parser = new SrslParser();
     
        parser.ParseSrslProgram( "MainModule", "..\\..\\src\\TestProgram\\" );
        CodeGenerator generator = new CodeGenerator();
        generator.CompileProgram( parser.Program );

        SrslVm srslVm = new SrslVm();
      
        int k = 5;
        long elapsedMillisecondsAccu = 0;
        for ( int i = 0; i < k; i++ )
        {
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            srslVm.Interpret( generator.CompiledMainChunk, generator.CompiledChunks, generator.SymbolTableBuilder );
            stopwatch2.Stop();
            Console.WriteLine("--Elapsed Time for Interpreting Run {0} is {1} ms", i, stopwatch2.ElapsedMilliseconds);
            elapsedMillisecondsAccu += stopwatch2.ElapsedMilliseconds;
            
           
        }  
        Console.WriteLine("--Average Elapsed Time for Interpreting per Run is {0} ms", elapsedMillisecondsAccu / k);
        Console.WriteLine("--Total Elapsed Time for Interpreting {0} Runs is {1} ms", k, elapsedMillisecondsAccu);
        var sortedDict = from entry in ChunkDebugHelper.InstructionCounter orderby entry.Value descending select entry;
        long totalInstructions = 0;
        foreach ( KeyValuePair<string,long> keyValuePair in sortedDict )
        {
            totalInstructions += keyValuePair.Value;
        }
        
        foreach ( KeyValuePair<string,long> keyValuePair in sortedDict )
        {
            Console.WriteLine("--Instruction Count for Instruction {0}: {2}     {1}%", keyValuePair.Key, (100.0 / totalInstructions * keyValuePair.Value).ToString("00.0"), keyValuePair.Value );
        }
            
        ChunkDebugHelper.InstructionCounter.Clear();
    }

    #endregion
}

}
