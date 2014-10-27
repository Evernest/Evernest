using System;


namespace Cloud14
{
    class Message
    {
        protected String message;
        protected Int64 id;
        public Message(String s, Int64 id)
        {
            message = s;
            this.id = id;
        }

        public Int64 getID()
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
