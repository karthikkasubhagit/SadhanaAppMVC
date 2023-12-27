using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using System.Linq;
using System.Linq.Expressions;

namespace SadhanaApp.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }
        public void Add(User entity)
        {
            _db.Add(entity);
        }

        public void Delete(User entity)
        {
            _db.Remove(entity);
        }

        public User Get(Expression<Func<User, bool>> filter, string? includeProperties = null)
        {
            IQueryable<User> query = _db.Set<User>();

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

        public IEnumerable<User> GetAll(Expression<Func<User, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<User> query = _db.Set<User>();

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

            return query.ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(User entity)
        {
            _db.Users.Update(entity);
        }
    }
}
