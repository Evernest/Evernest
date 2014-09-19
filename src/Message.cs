using System;


namespace Cloud14
{
    class Message
    {
        protected String message;
        protected int id;
        public Message(String s, int id)
        {
            message = s;
            this.id = id;
        }

        public int getID()
        {
            return id;
        }

        public String getMessage()
        {
            return message;
        }

        public override string ToString()
        {
            return "Id : " + this.id + " - " + message;
        }
    }

    class MessageBuilder : Message
    {
        public MessageBuilder(String s, int id) : base(s, id) { }

        public void setID(int newID){
            base.id = newID;
        }
    }
}
