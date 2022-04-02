﻿using System.Collections.Generic;
using Bite.Ast;
using Bite.Runtime.Bytecode;
using Bite.Runtime.SymbolTable;
using Bite.SymbolTable;

namespace Bite.Runtime.CodeGen
{
    public class BiteProgram
    {
        private Dictionary<string, Chunk> m_CompilingChunks;
        private Chunk m_CompilingChunk;
        private Stack<Chunk> _savedChunks = new Stack<Chunk>();

        internal BaseScope BaseScope { get; }
        internal Dictionary<string, BinaryChunk> CompiledChunks { get; }
        internal Chunk MainChunk { get;  }
        internal BinaryChunk CompiledMainChunk { get; private set; }
        internal Chunk CurrentChunk
        {
            get => m_CompilingChunk;
            set
            {
                m_CompilingChunk = value;
            }
        }

        public BiteProgram(ModuleNode module)
        {
            var symbolTableBuilder = new SymbolTableBuilder();
            symbolTableBuilder.BuildModuleSymbolTable(module);
            BaseScope = symbolTableBuilder.CurrentScope as BaseScope;
            m_CompilingChunks = new Dictionary<string, Chunk>();
            MainChunk = new Chunk();
            CurrentChunk = MainChunk;
            CompiledChunks = new Dictionary<string, BinaryChunk>();
        }

        public BiteProgram(ProgramNode programNode)
        {
            var symbolTableBuilder = new SymbolTableBuilder();
            symbolTableBuilder.BuildProgramSymbolTable(programNode);
            BaseScope = symbolTableBuilder.CurrentScope as BaseScope;
            m_CompilingChunks = new Dictionary<string, Chunk>();
            MainChunk = new Chunk();
            CurrentChunk = MainChunk;
            CompiledChunks = new Dictionary<string, BinaryChunk>();
        }

        internal bool HasChunk(string moduleName)
        {
            return m_CompilingChunks.ContainsKey(moduleName);
        }

        internal void PushChunk()
        {
            _savedChunks.Push(CurrentChunk);
        }

        internal void PopChunk()
        {
            CurrentChunk = _savedChunks.Pop();
        }

        internal void NewChunk()
        {
            CurrentChunk = new Chunk();
        }

        internal void SaveCurrentChunk(string moduleName)
        {
            m_CompilingChunks.Add(moduleName, CurrentChunk);
        }

        internal void RestoreChunk(string moduleName)
        {
            CurrentChunk = m_CompilingChunks[moduleName];
        }


        internal void Build()
        {

            foreach (KeyValuePair<string, Chunk> compilingChunk in m_CompilingChunks)
            {
                CompiledChunks.Add(compilingChunk.Key, new BinaryChunk(compilingChunk.Value.SerializeToBytes(), compilingChunk.Value.Constants, compilingChunk.Value.Lines));
            }

            CompiledMainChunk = new BinaryChunk(MainChunk.SerializeToBytes(), MainChunk.Constants, MainChunk.Lines);
        }

        /// <summary>
        /// Creates a new <see cref="BiteVm"/> and executes the <see cref="BiteProgram"/>
        /// </summary>
        /// <returns></returns>
        public BiteResult Run()
        {
            var biteVm = new BiteVm();
            var result = biteVm.Interpret(this);
            return new BiteResult()
            {
                InterpretResult = result,
                ReturnValue = biteVm.ReturnValue
            };
        }
    }
}