using EvernestFront.Answers;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;

namespace EvernestFront
{
    partial class User
    {
        public CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
        {
            //TODO: what if streamId does not exist?
            
            if (InternalSources.ContainsKey(sourceName))
                return new CreateSource(new SourceNameTaken(Id, sourceName));

            var sourceContract = new SourceContract(sourceName, Id, streamId, rights);
            var key = Keys.NewKey();
            var sourceCreated = new SourceCreated(key, sourceContract);

            Projection.ProjectionOld.HandleDiff(sourceCreated);
            return new CreateSource(key);
        }



        public DeleteSource DeleteSource(string sourceName)
        {
            
            string key;
            if (InternalSources.TryGetValue(sourceName, out key))
            {
                var sourceDeleted = new SourceDeleted(key, Id, sourceName);
                Projection.ProjectionOld.HandleDiff(sourceDeleted);
                //TODO: system stream
                return new DeleteSource();
            }
            else
                return new DeleteSource();
                //TODO: error?

        }
    }
}
