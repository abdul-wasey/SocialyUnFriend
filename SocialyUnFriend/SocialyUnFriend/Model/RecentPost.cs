using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class RecentPost
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImageUri { get; set; }
        public string Platform { get; set; }
        public DateTime? DateTime { get; set; }

        public bool IsBusy { get; set; }
        
        
    }
}
