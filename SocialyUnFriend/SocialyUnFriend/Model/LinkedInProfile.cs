using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class LinkedInProfile
    {
        [JsonProperty("id")]
        public string UserProfileID { get; set; }

        [JsonProperty("localizedFirstName")]
        public string FirstName { get; set; }

        [JsonProperty("localizedLastName")]
        public string LastName { get; set; }

        [JsonProperty("profilePicture")]
        public ProfilePicture UserProfilePicture { get; set; }

    }

    public class ProfilePicture
    {
        [JsonProperty("displayImage")]
        public string DisplayImageUrn { get; set; }

        [JsonProperty("displayImage~")]
        public DisplayImage DisplayImage { get; set; }
    }

    public class DisplayImage
    {

        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        [JsonProperty("identifiers")]
        public List<Identifier> Identifiers { get; set; }
    }


    public class Identifier
    {
        public string identifier { get; set; }
    }








}
