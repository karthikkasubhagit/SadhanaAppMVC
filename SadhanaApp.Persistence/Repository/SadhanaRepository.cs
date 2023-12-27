using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using System.Linq.Expressions;

namespace SadhanaApp.Persistence.Repository
{
    public class SadhanaRepository : ISadhanaRepository
    {
        private readonly AppDbContext _db;

        public SadhanaRepository(AppDbContext db)
        {
            _db = db;
        }
        public void Add(ChantingRecord entity)
        {
            _db.Add(entity);
        }

        public void Delete(ChantingRecord entity)
        {
            _db.Remove(entity);
        }

        public ChantingRecord Get(Expression<Func<ChantingRecord, bool>> filter, string? includeProperties = null)
        {
            IQueryable<ChantingRecord> query = _db.Set<ChantingRecord>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Service, User
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public IEnumerable<ChantingRecord> GetAll(Expression<Func<ChantingRecord, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<ChantingRecord> query = _db.Set<ChantingRecord>();

            if(filter != null)
            {
                query = query.Where(filter);
            }

            if(!string.IsNullOrEmpty(includeProperties))
            {
                // Service, User
                foreach(var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(ChantingRecord entity)
        {
            _db.ChantingRecords.Update(entity);
        }
    }
}
