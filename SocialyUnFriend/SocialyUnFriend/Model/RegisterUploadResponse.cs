using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class RegisterUploadResponse
    {
        public Value value { get; set; }
    }

    public class Value
    {
        public UploadMechanism uploadMechanism { get; set; }
        public string mediaArtifact { get; set; }
        public string asset { get; set; }
    }

    public class UploadMechanism
    {
        [JsonProperty("com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest")]
        public ComLinkedinDigitalmediaUploadingMediaUploadHttpRequest MediaUploadHttpRequest { get; set; }
    }

    public class ComLinkedinDigitalmediaUploadingMediaUploadHttpRequest
    {
        public Headers headers { get; set; }

        [JsonProperty("uploadUrl")]
        public string UpLoadUrl { get; set; }
    }

    public class Headers
    {
        [JsonProperty("media-type-family")]
        public string MediaType { get; set; }  //can be image , video and file etc
    }
}
