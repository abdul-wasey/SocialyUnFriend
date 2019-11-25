using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialyUnFriend.Model
{

    public class LabeledLatLng
    {
        public string label { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    
    public class Icon
    {
        public int IconId { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string pluralName { get; set; }
        public string shortName { get; set; }
        public Icon icon { get; set; }
        public bool primary { get; set; }
    }

    public class Venue
    {
        [Key]
        public int VenueId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string Image { get; set; }
        public string City { get; set; }

        [NotMapped]
        public Location location { get; set; }

        [NotMapped]
        public List<Category> categories { get; set; }

        [NotMapped]
        public string referralId { get; set; }

        [NotMapped]
        public bool hasPerk { get; set; }

        [NotMapped]
        public List<object> hierarchy { get; set; }

       
    }

    public class Location
    {
        public int LocationId { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }

        [NotMapped]
        public List<LabeledLatLng> labeledLatLngs { get; set; }
        public int distance { get; set; }
        public string cc { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }

        [NotMapped]
        public List<string> formattedAddress { get; set; }
        public string crossStreet { get; set; }
    }



    public class Venues
    {
        public Meta meta { get; set; }
        public List<Notification> notifications { get; set; }
        public Response response { get; set; }
    }
}
