namespace EvernestFront.Service.Command
{
    class UserRightSettingByUser : CommandBase
    {
        internal string TargetName { get; private set; }

        internal long EventStreamId { get; private set; }

        internal string AdminName { get; private set; }

        internal AccessRight Right { get; private set; }

        internal UserRightSettingByUser(CommandReceiver commandReceiver, string targetName, long eventStreamId,
            string adminName, AccessRight right)
            : base(commandReceiver)
        {
            TargetName = targetName;
            EventStreamId = eventStreamId;
            AdminName = adminName;
            Right = right;
        }
    }
}
