using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{

    public class LinkedInPost
    {
        [JsonProperty("content")]
        public Content Content { get; set; }  // Referenced content such as articles and images

        /* Required to set the share as publicly visible. For sponsored content where the targeting is defined when it is sponsored, 
           distribution should be null.*/

        [JsonProperty("distribution")]
        public Distribution Distribution { get; set; }


        [JsonProperty("owner")]
        public string Owner { get; set; } // owner can be person or organization , (it must be in the format of urn) 

        [JsonProperty("subject")]
        public string Subject { get; set; } //Subject of the share,

        [JsonProperty("text")]
        public Text Text { get; set; } //Text of the share
    }
    public class Thumbnail
    {
        [JsonProperty("resolvedUrl")]
        public string ResolvedUrl { get; set; }
    }

    public class ContentEntity
    {
        [JsonProperty("entityLocation")]
        public string EntityLocation { get; set; }

        [JsonProperty("thumbnails")]
        public List<Thumbnail> Thumbnails { get; set; }

        //[JsonProperty("entity")]
        //public string Entity { get; set; }
    }

    public class Content
    {
        [JsonProperty("contentEntities")]
        public List<ContentEntity> ContentEntities { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        //[JsonProperty("shareMediaCategory")]
        //public string ShareMediaCategory { get; set; }
    }

    public class LinkedInDistributionTarget
    {
    }

    public class Distribution
    {
        [JsonProperty("linkedInDistributionTarget")]
        public LinkedInDistributionTarget LinkedInDistributionTarget { get; set; }
    }

    public class Text
    {
        public string text { get; set; }
    }




}
