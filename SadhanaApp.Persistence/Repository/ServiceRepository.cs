using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;

namespace SadhanaApp.Persistence.Repository
{
    public class ServiceRepository : Repository<ServiceType>, IServiceRepository
    {
        private readonly AppDbContext _db;

        public ServiceRepository(AppDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(ServiceType entity)
        {
            _db.ServiceTypes.Update(entity);
        }
    }
}
