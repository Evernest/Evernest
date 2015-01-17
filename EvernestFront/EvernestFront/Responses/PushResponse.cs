
namespace EvernestFront.Responses
{
    public class PushResponse : BaseResponse
    {
        public long MessageId { get; private set; }

        internal PushResponse(FrontError err)
            : base(err) { }


        /// <summary>
        /// Sets field success to true and MessageID to id.
        /// </summary>
        /// <param name="id"></param>
        internal PushResponse(long id)
        :base ()
        {
            MessageId = id;
        }

    }
}
