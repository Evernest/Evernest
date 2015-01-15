using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront
{
    partial class User
    {
        public CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
        {
            //TODO: what if streamId does not exist?
            
            if (InternalSources.ContainsKey(sourceName))
                return new CreateSource(FrontError.SourceNameTaken);

            var sourceContract = new SourceContract(sourceName, Id, streamId, rights);
            var key = Keys.NewKey();
            var sourceCreated = new SourceCreated(key, sourceContract);

            Projection.Projection.HandleDiff(sourceCreated);
            return new CreateSource(key);
        }



        public DeleteSource DeleteSource(string sourceName)
        {
            
            string key;
            if (InternalSources.TryGetValue(sourceName, out key))
            {
                var sourceDeleted = new SourceDeleted(key, Id, sourceName);
                Projection.Projection.HandleDiff(sourceDeleted);
                //TODO: system stream
                return new DeleteSource();
            }
            else
                return new DeleteSource();
                //TODO: error?

        }
    }
}
