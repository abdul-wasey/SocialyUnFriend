using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.Services
{
    public interface IGeoLocatorService
    {
        double Latitude { get; set; }

        double Longitude { get; set; }

        Task GetLocationAsync();

        bool IsGpsEnabled();
    }
}
