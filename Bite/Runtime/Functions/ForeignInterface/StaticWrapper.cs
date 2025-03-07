﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bite.Runtime.Functions.ForeignInterface
{

public class StaticWrapper
{
    private readonly Type _type;
    private readonly Dictionary < string, FastMethodInfo > CachedMethods = new Dictionary < string, FastMethodInfo >();

    #region Public

    public StaticWrapper( Type type )
    {
        _type = type;
    }

    public object InvokeMember( string name, object[] args, Type[] argsTypes )
    {
        if ( !CachedMethods.ContainsKey( name ) )
        {
            MethodInfo method = _type.GetMethod(
                name,
                BindingFlags.Static | BindingFlags.Public,
                null,
                argsTypes,
                null );

            FastMethodInfo fastMethodInfo = new FastMethodInfo( method );

            CachedMethods.Add( name, fastMethodInfo );

            return fastMethodInfo.Invoke( null, args );
        }

        return CachedMethods[name].Invoke( null, args );
    }

    #endregion
}

}
