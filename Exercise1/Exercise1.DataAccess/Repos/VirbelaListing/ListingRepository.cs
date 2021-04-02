using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Exercise1.Common.Tasks;
using Exercise1.Data.Models.VirbelaListing;
using Exercise1.Data.Repos;
using Exercise1.DataAccess.Context;
using Exercise1.DataAccess.Repos.Extension;
using System;

namespace Exercise1.DataAccess.Repos.VirbelaListing
{
    public class ListingRepository : IRepository<Listing>
    {
        private VirbelaListingContext _context;

        public ListingRepository(VirbelaListingContext context)
        {
            this._context = context;
        }
        
        public async Task<IEnumerable<Listing>> GetAsync(
            object parameters = null, 
            int? pageNum = null, 
            int? pageSize = null)
        {
            DbSet<Listing> dbSet =  _context.Listing;
            IQueryable<Listing> query = dbSet.AsNoTracking();

            if (parameters != null) {
                var param = (List<KeyValuePair<string, string>>)parameters;

                // TO DO: Improve the loop by Reflection later
                // TO DO: Extract filtering logics in separte module
                foreach(KeyValuePair<string, string> kv in param) {
                    if (kv.Value == null)
                        continue;
                    else if (kv.Key == "CreatorId" 
                    && int.TryParse(kv.Value, out int creatorId)) {
                        query = query.Where(i => i.CreatorId == creatorId);
                    }
                    else if (kv.Key == "Title") {
                        query = query.Where(i => i.Title == kv.Value);
                    }
                    else if (kv.Key == "Description") {
                        query = query.Where(i => i.Description == kv.Value);
                    }
                    else if (kv.Key == "Price") {
                        if (!decimal.TryParse(kv.Value, out decimal price))
                            return new List<Listing>();
                        query = query.Where(i => i.Price == price);
                    }
                    else if (kv.Key == "CreatedDate") {
                        if (!DateTime.TryParse(kv.Value, out DateTime date)) 
                            return new List<Listing>();
                        query = query.Where(i => i.CreatedDate == date);
                    }
                }
            }

            if ((pageNum??0) >  0 && (pageSize??0) > 0) {
                query = query
                    .Skip((pageNum.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Listing> GetAsync(string id)
        {
            int idNum;
            if (!int.TryParse(id, out idNum))
                return await Task.FromResult<Listing>(null);

            IQueryable<Listing> result = _context.Listing
                .Where(r => r.Id == idNum);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<Listing> PutAsync(string id, Listing updateRequest)
        {
            return await TaskConstants<Listing>.NotImplemented;
        }

        public async Task<Listing> PostAsync(Listing createRequest)
        {
            await _context.Listing.AddAsync(createRequest);
            // _context.Entry(createRequest).State = EntityState.Added;
            return createRequest;
        }

        public async Task<Listing> DeleteAsync(string id)
        {
            return await TaskConstants<Listing>.NotImplemented;
        }

    }
}