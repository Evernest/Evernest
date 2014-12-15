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
        internal ImmutableDictionary<long, EventStreamContract> EventStreamTable;

        [DataMember]
        internal ImmutableDictionary<string, SourceContract> SourceTable;

        [DataMember]
        internal ImmutableDictionary<string, long> UserKeyToId;
        [DataMember]
        internal ImmutableDictionary<string, long> UserNameToId;
        [DataMember]
        internal ImmutableDictionary<string, long> EventStreamNameToId;



        internal Tables()
        {
            UserTable = ImmutableDictionary<long, UserContract>.Empty;
            EventStreamTable = ImmutableDictionary<long, EventStreamContract>.Empty;
            SourceTable = ImmutableDictionary<string, SourceContract>.Empty;
            UserKeyToId = ImmutableDictionary<string, long>.Empty;
            UserNameToId = ImmutableDictionary<string, long>.Empty;
            EventStreamNameToId = ImmutableDictionary<string, long>.Empty;
        }

        internal Tables(ImmutableDictionary<long, UserContract> usrTbl, ImmutableDictionary<long, EventStreamContract> strmTbl,
            ImmutableDictionary<string, SourceContract> srcTbl, ImmutableDictionary<string, long> usrKtI,
            ImmutableDictionary<string, long> usrNtI, ImmutableDictionary<string, long> strmNtI)
        {
            UserTable = usrTbl;
            EventStreamTable = strmTbl;
            SourceTable = srcTbl;
            UserKeyToId = usrKtI;
            UserNameToId = usrNtI;
            EventStreamNameToId = strmNtI;
        }

        

    }
}
