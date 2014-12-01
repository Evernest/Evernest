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
        internal ImmutableDictionary<string, SourceContract> SourceTable;

        [DataMember]
        internal ImmutableDictionary<string, long> UserNameToId;
        [DataMember]
        internal ImmutableDictionary<string, long> StreamNameToId;



        internal Tables()
        {
            UserTable = ImmutableDictionary<long, UserContract>.Empty;
            StreamTable = ImmutableDictionary<long, StreamContract>.Empty;
            SourceTable = ImmutableDictionary<string, SourceContract>.Empty;
            UserNameToId = ImmutableDictionary<string, long>.Empty;
            StreamNameToId = ImmutableDictionary<string, long>.Empty;
        }

        internal Tables(ImmutableDictionary<long, UserContract> usrTbl, ImmutableDictionary<long, StreamContract> strmTbl,
            ImmutableDictionary<string, SourceContract> srcTbl, ImmutableDictionary<string, long> usrNtI,
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
