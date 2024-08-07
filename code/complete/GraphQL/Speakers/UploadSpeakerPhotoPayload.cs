using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers
{
    public class UploadSpeakerPhotoPayload : SpeakerPayloadBase
    {
        public UploadSpeakerPhotoPayload(Speaker speaker) 
            : base(speaker)
        {
        }

        public UploadSpeakerPhotoPayload(UserError error) 
            : base(new[] { error })
        {
        }

        
    }
}