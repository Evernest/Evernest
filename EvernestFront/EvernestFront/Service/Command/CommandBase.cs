namespace EvernestFront.Service.Command
{
    abstract class CommandBase
    {
        //TODO: add an id

        private readonly SystemEventProducer _systemEventProducer;

        protected CommandBase(SystemEventProducer systemEventProducer)
        {
            _systemEventProducer = systemEventProducer;
        }

        public void Execute()
        {
            _systemEventProducer.HandleCommand(this);
        }
    }
}
