using SocialyUnFriend.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.Repositories
{
    public interface IVenuesRepository
    {
        Task<IEnumerable<Venue>> GetVenuesAsync();

        Task<bool> AddVenueAsync(Venue venue);

        Task<int> AddAllVenuesAsync(IEnumerable<Venue> venues);
       

        Task<bool> DeleteVenueAsync(Venue venue);

        Task<int> DeleteAllVenuesAsync(IEnumerable<Venue> venues);
    }
}
