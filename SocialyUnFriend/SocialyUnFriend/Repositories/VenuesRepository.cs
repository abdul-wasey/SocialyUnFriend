using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialyUnFriend.LocalDB;
using SocialyUnFriend.Model;

namespace SocialyUnFriend.Repositories
{
    public class VenuesRepository : IVenuesRepository
    {
        private readonly DatabaseContext _databaseContext;
        public VenuesRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<IEnumerable<Venue>> GetVenuesAsync()
        {
            try
            {
               
                var venues = await _databaseContext.Venues.ToListAsync();

                return venues;
               

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddVenueAsync(Venue venue)
        {
            try
            {

                var tracking = await _databaseContext.Venues.AddAsync(venue).ConfigureAwait(false);

                await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                var isAdded = tracking.State == EntityState.Added;

                return isAdded;


            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> AddAllVenuesAsync(IEnumerable<Venue> venues)
        {
            try
            {

                await _databaseContext.Venues.AddRangeAsync(venues).ConfigureAwait(false);

                var isAdded = await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                return isAdded;


            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> DeleteVenueAsync(Venue venue)
        {
            try
            {
                
                var tracking = _databaseContext.Venues.Remove(venue);

                await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                var isDeleted = tracking.State == EntityState.Deleted;

                return isDeleted;
                
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> DeleteAllVenuesAsync(IEnumerable<Venue> venues)
        {
            try
            {
               
                _databaseContext.Venues.RemoveRange(venues);
                
                var isDeleted = await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

                return isDeleted;
               
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
