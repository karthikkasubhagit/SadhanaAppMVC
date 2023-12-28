using SadhanaApp.Domain;
using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IServiceRepository : IRepository<ServiceType>
    {
        void Update(ServiceType entity);
    }
}
