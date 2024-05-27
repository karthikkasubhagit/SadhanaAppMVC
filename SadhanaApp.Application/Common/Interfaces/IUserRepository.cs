using SadhanaApp.Domain;
using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        void Update(User entity);
    }
}
