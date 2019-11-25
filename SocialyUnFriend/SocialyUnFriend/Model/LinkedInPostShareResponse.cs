using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class LinkedInPostShareResponse
    {
        public string owner { get; set; }
        public string activity { get; set; }
        public bool edited { get; set; }
        public string subject { get; set; }
        public string serviceProvider { get; set; }
        public string id { get; set; }
        public Text text { get; set; }
        public Distribution distribution { get; set; }
        public Content content { get; set; }

    }

    public class Created
    {
        public string actor { get; set; }
        public long time { get; set; }
    }

    public class LastModified
    {
        public string actor { get; set; }
        public long time { get; set; }
    }


    public class ImageSpecificContent
    {
    }

}
