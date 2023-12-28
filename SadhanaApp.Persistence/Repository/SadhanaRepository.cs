using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using System.Linq.Expressions;

namespace SadhanaApp.Persistence.Repository
{
    public class SadhanaRepository : Repository<ChantingRecord>, ISadhanaRepository
    {
        private readonly AppDbContext _db;

        public SadhanaRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ChantingRecord entity)
        {
            _db.ChantingRecords.Update(entity);
        }
    }
}
