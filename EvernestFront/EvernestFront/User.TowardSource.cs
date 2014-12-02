using System;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Errors;

namespace EvernestFront
{
    partial class User
    {
        public CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
        {
            //what if streamId does not exist?
            
            if (UserContract.OwnedSources.ContainsKey(sourceName))
                return new CreateSource(new SourceNameTaken(Id, sourceName));

            var sourceContract = new SourceContract(sourceName, Id, streamId, rights);
            var key = Keys.NewKey();
            var sourceCreated = new SourceCreated(key, sourceContract);

            Projection.Projection.HandleDiff(sourceCreated);
            return new CreateSource(key);
        }

        public Answers.DeleteSource DeleteSource(string sourceName)
        { throw new NotImplementedException(); }
    }
}
