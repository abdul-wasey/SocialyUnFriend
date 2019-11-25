using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{

   

    public class Target2
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Object
    {
        public string id { get; set; }
        public string type { get; set; }
        public Target2 target { get; set; }
        public bool ignorable { get; set; }
    }

    public class Target
    {
        public string type { get; set; }
        public Object @object { get; set; }
    }

    public class Item2
    {
        public string summary { get; set; }
        public string type { get; set; }
        public string reasonName { get; set; }
        public Target target { get; set; }
    }

    public class Reasons
    {
        public int count { get; set; }
        public List<Item2> items { get; set; }
    }

    public class Source
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Posts
    {
        public int count { get; set; }
        public int textCount { get; set; }
    }

    public class Likes
    {
        public int count { get; set; }
        public List<object> groups { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Score2
    {
        public string icon { get; set; }
        public string message { get; set; }
        public int points { get; set; }
    }

    public class Score
    {
        public int total { get; set; }
        public List<Score2> scores { get; set; }
    }

    public class Checkin
    {
        public string id { get; set; }
        public int createdAt { get; set; }
        public string type { get; set; }
        public int timeZoneOffset { get; set; }
        public long editableUntil { get; set; }
        public User user { get; set; }
        public Venue venue { get; set; }
        public Source source { get; set; }
        public Photos photos { get; set; }
        public Posts posts { get; set; }
        public string checkinShortUrl { get; set; }
        public Likes likes { get; set; }
        public bool like { get; set; }
        public Comments comments { get; set; }
        public bool isMayor { get; set; }
        public Score score { get; set; }
    }

    public class Image
    {
        public string prefix { get; set; }
        public List<int> sizes { get; set; }
        public string name { get; set; }
        public string key { get; set; }
    }

    public class Points
    {
        public Image image { get; set; }
        public string message { get; set; }
        public int points { get; set; }
    }

    public class Item4
    {
        public string type { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public Points points { get; set; }
    }

    public class Insights
    {
        public int count { get; set; }
        public List<Item4> items { get; set; }
    }

    public class Item3
    {
        public string message { get; set; }
        public List<object> entities { get; set; }
        public Insights insights { get; set; }
    }

    public class Notification2
    {
        public string type { get; set; }
        public Item3 item { get; set; }
        public bool alert { get; set; }
    }

    public class CheckInResponse
    {
        public Meta meta { get; set; }
        public List<Notification> notifications { get; set; }
        public Response response { get; set; }
    }
}
