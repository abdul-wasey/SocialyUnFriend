using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{

    public class FourSquareProfile
    {
        public Meta meta { get; set; }
        public List<Notification> notifications { get; set; }
        public Response response { get; set; }
    }


    public class Meta
    {
        public int code { get; set; }
        public string requestId { get; set; }
    }

    public class Item
    {
        public int unreadCount { get; set; }
    }

    public class Notification
    {
        public string type { get; set; }
        public Item item { get; set; }
    }

    public class Photo
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Group
    {
        public string type { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Friends
    {
        public int count { get; set; }
        public List<Group> groups { get; set; }
    }

    public class Tips
    {
        public int count { get; set; }
    }

    public class Contact
    {
        public string phone { get; set; }
        public string verifiedPhone { get; set; }
        public string email { get; set; }
    }

    public class Photos
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Mayorships
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Checkins
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Requests
    {
        public int count { get; set; }
    }

    public class Group2
    {
        public string type { get; set; }
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Lists
    {
        public int count { get; set; }
        public List<Group2> groups { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string relationship { get; set; }
        public string canonicalUrl { get; set; }
        public Photo photo { get; set; }
        public Friends friends { get; set; }
        public int birthday { get; set; }
        public Tips tips { get; set; }
        public string homeCity { get; set; }
        public string bio { get; set; }
        public Contact contact { get; set; }
        public Photos photos { get; set; }
        public string checkinPings { get; set; }
        public bool pings { get; set; }
        public string type { get; set; }
        public Mayorships mayorships { get; set; }
        public Checkins checkins { get; set; }
        public Requests requests { get; set; }
        public Lists lists { get; set; }
        public string blockedStatus { get; set; }
        public int createdAt { get; set; }
        public List<object> lenses { get; set; }
        public string referralId { get; set; }
    }

    public class Response
    {
        
        public User user { get; set; }
        public List<Venue> venues { get; set; }
        public bool confident { get; set; }
        public Checkin checkin { get; set; }
        public List<Notification2> notifications { get; set; }
        public List<string> notificationsOrder { get; set; }
    }

    
}
