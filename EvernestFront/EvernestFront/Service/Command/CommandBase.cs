namespace EvernestFront.Service.Command
{
    abstract class CommandBase
    {
        //TODO: add an id

        private readonly CommandReceiver _commandReceiver;

        protected CommandBase(CommandReceiver commandReceiver)
        {
            _commandReceiver = commandReceiver;
        }

        public void Execute()
        {
            _commandReceiver.ReceiveCommand(this);
        }
    }
}
