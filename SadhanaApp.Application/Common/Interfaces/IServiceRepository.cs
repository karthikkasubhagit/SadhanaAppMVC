using SadhanaApp.Domain;
using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IServiceRepository
    {
        IEnumerable<ServiceType> GetAll(Expression<Func<ServiceType, bool>>? filter = null, string? includeProperties = null);
        ServiceType Get(Expression<Func<ServiceType, bool>> filter, string? includeProperties = null);
        void Add(ServiceType entity);
        void Update(ServiceType entity);
        void Delete(ServiceType entity);
        void Save();
    }
}
