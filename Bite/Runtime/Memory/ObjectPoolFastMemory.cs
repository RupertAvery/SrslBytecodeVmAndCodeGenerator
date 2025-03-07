﻿using System;

namespace Bite.Runtime.Memory
{

public class ObjectPoolFastMemory
{
    private readonly FastMemorySpace[] m_FastCallMemorySpaces = new FastMemorySpace[1024];
    private int m_FastMemorySpacePointer = 0;

    #region Public

    public ObjectPoolFastMemory()
    {
        for ( int i = 0; i < 1024; i++ )
        {
            m_FastCallMemorySpaces[i] = new FastMemorySpace( $"$objectpool_{i}", null, 0, null, 0, 0 );
        }
    }

    public FastMemorySpace Get()
    {
        FastMemorySpace fastMemorySpace = m_FastCallMemorySpaces[m_FastMemorySpacePointer];
        fastMemorySpace.Properties = Array.Empty < DynamicBiteVariable >();
        fastMemorySpace.NamesToProperties.Clear();
        fastMemorySpace.CallerChunk = null;
        fastMemorySpace.CallerIntructionPointer = 0;
        fastMemorySpace.StackCountAtBegin = 0;
        if ( m_FastMemorySpacePointer >= 1023 )
        {
            throw new IndexOutOfRangeException( "Memory Pool Overflow" );
        }

        m_FastMemorySpacePointer++;

        return fastMemorySpace;
    }

    public void Return( FastMemorySpace item )
    {
        if ( item == m_FastCallMemorySpaces[m_FastMemorySpacePointer - 1] )
        {
            m_FastMemorySpacePointer--;
        }
    }

    #endregion
}

}
