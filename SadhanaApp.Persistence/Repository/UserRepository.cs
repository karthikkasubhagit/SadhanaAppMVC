using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using System.Linq;
using System.Linq.Expressions;

namespace SadhanaApp.Persistence.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }        
        public void Update(User entity)
        {
            _db.Users.Update(entity);
        }
    }
}
