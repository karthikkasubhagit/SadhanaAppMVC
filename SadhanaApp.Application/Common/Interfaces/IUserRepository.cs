using SadhanaApp.Domain;
using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll(Expression<Func<User, bool>>? filter = null, string? includeProperties = null);
        User Get(Expression<Func<User, bool>> filter, string? includeProperties = null);
        void Add(User entity);
        void Update(User entity);
        void Delete(User entity);
        void Save();
    }
}
