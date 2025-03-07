﻿using System.IO;

namespace Bite.Runtime.Bytecode
{

public static class ChunkHelper
{
    #region Public

    public static int DesserializeOpCodeData( ref byte[] data, ref int instructionPointer )
    {
        int result;

        result = data[instructionPointer] |
                 ( data[instructionPointer + 1] << 8 ) |
                 ( data[instructionPointer + 2] << 16 ) |
                 ( data[instructionPointer + 3] << 24 );

        //result = BitConverter.ToInt32( data, instructionPointer );

        instructionPointer += 4;

        return result;
    }

    public static void FreeChunk( this Chunk chunk )
    {
        chunk.Code.Clear();
        chunk.Lines.Clear();
    }

    public static byte[] SerializeToBytes( this Chunk chunk )
    {
        using ( MemoryStream m = new MemoryStream() )
        {
            using ( BinaryWriter writer = new BinaryWriter( m ) )
            {
                foreach ( ByteCode code in chunk.Code )
                {
                    writer.Write( ( byte ) code.OpCode );

                    if ( code.OpCodeData != null )
                    {
                        foreach ( int data in code.OpCodeData )
                        {
                            writer.Write( data );
                        }
                    }
                }
            }

            return m.ToArray();
        }
    }

    public static int WriteToChunk( this Chunk chunk, BiteVmOpCodes code, int line )
    {
        chunk.Code.Add( new ByteCode( code ) );
        chunk.Lines.Add( line );

        return chunk.Code.Count - 1;
    }

    public static int WriteToChunk( this Chunk chunk, ByteCode code, int line )
    {
        chunk.Code.Add( code );
        chunk.Lines.Add( line );

        return chunk.Code.Count - 1;
    }

    public static int WriteToChunk( this Chunk chunk, BiteVmOpCodes code, ConstantValue constant, int line )
    {
        chunk.Code.Add( new ByteCode( code, chunk.Constants.Count ) );
        chunk.Constants.Add( constant );
        chunk.Lines.Add( line );

        return chunk.Code.Count - 1;
    }

    public static int WriteToChunk(
        this Chunk chunk,
        BiteVmOpCodes code,
        ConstantValue constant,
        int opCodeData,
        int line )
    {
        chunk.Code.Add( new ByteCode( code, chunk.Constants.Count, opCodeData ) );
        chunk.Constants.Add( constant );
        chunk.Lines.Add( line );

        return chunk.Code.Count - 1;
    }

    #endregion
}

}
