namespace EvernestFront.Contract
{
    public enum FrontError
    {
        AdminAccessDenied=0,
        BackendError=1,
        CannotDestituteAdmin=2,
        EventStreamIdDoesNotExist=3,
        EventStreamNameTaken=4,
        InvalidEventId=5,
        InvalidString=6,
        ReadAccessDenied=7,
        SourceIdDoesNotExist=8,
        SourceKeyDoesNotExist=9,
        SourceNameDoesNotExist=10,
        SourceNameTaken=11,
        UnknownFrontError=12,
        UserIdDoesNotExist=13,
        UserKeyDoesNotExist=14,
        UserKeyNameDoesNotExist=21,
        UserKeyNameTaken=15,
        UserNameDoesNotExist=16,
        UserNameTaken=17,
        UserOwningSourceDoesNotExist=18,
        WriteAccessDenied=19,
        WrongPassword=20
    }
}
