using SadhanaApp.Application.Common.Interfaces;

namespace SadhanaApp.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public ISadhanaRepository SadhanaRepository { get; private set; }
        public IServiceRepository ServiceRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            SadhanaRepository = new SadhanaRepository(_db);
            ServiceRepository = new ServiceRepository(_db);
            UserRepository = new UserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
