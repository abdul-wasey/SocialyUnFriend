using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class RegisterUpload
    {
        public RegisterUploadRequest registerUploadRequest { get; set; }
    }

    public class ServiceRelationship
    {
        public string identifier { get; set; }
        public string relationshipType { get; set; }
    }

    public class RegisterUploadRequest
    {
        public string owner { get; set; }
        public List<string> recipes { get; set; }
        public List<ServiceRelationship> serviceRelationships { get; set; }
    }
}
