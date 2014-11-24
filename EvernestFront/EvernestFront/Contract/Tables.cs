using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class Tables
    {
        [DataMember]
        internal ImmutableDictionary<long, UserContract> UserTable;
        [DataMember]
        internal ImmutableDictionary<long, StreamContract> StreamTable;
        [DataMember]
        internal ImmutableDictionary<string, int> SourceTable; //int should be Contract.Source when it is implemented

        [DataMember]
        internal ImmutableDictionary<string, long> UserNameToId;
        [DataMember]
        internal ImmutableDictionary<string, long> StreamNameToId;



        internal Tables()
        {
            UserTable = ImmutableDictionary<long, UserContract>.Empty;
            StreamTable = ImmutableDictionary<long, StreamContract>.Empty;
            SourceTable = ImmutableDictionary<string, int>.Empty;
            UserNameToId = ImmutableDictionary<string, long>.Empty;
            StreamNameToId = ImmutableDictionary<string, long>.Empty;
        }

        internal Tables(ImmutableDictionary<long, UserContract> usrTbl, ImmutableDictionary<long, StreamContract> strmTbl,
            ImmutableDictionary<string, int> srcTbl, ImmutableDictionary<string, long> usrNtI,
            ImmutableDictionary<string, long> strmNtI)
        {
            UserTable = usrTbl;
            StreamTable = strmTbl;
            SourceTable = srcTbl;
            UserNameToId = usrNtI;
            StreamNameToId = strmNtI;
        }

        

    }
}
