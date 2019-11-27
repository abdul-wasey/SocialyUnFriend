using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
   


    public class ShareCommentary
    {
        public string text { get; set; }
    }

    public class Description
    {
        public string text { get; set; }
    }

    public class Title
    {
        public string text { get; set; }
    }

    public class Medium
    {
        public string status { get; set; }
        public Description description { get; set; }
        public string media { get; set; }
        public Title title { get; set; }
    }

    public class ComLinkedinUgcShareContent
    {
        public ShareCommentary shareCommentary { get; set; }
        public string shareMediaCategory { get; set; }
        public List<Medium> media { get; set; }
    }

    public class SpecificContent
    {
        [JsonProperty("com.linkedin.ugc.ShareContent")]
        public ComLinkedinUgcShareContent ShareContent { get; set; }
    }

    public class Visibility
    {
        [JsonProperty("com.linkedin.ugc.MemberNetworkVisibility")]
        public string MemberNetworkVisibility { get; set; }
    }

    public class UGCPostRequestModel
    {
        public string author { get; set; }
        public string lifecycleState { get; set; }
        public SpecificContent specificContent { get; set; }
        public Visibility visibility { get; set; }
    }
}
