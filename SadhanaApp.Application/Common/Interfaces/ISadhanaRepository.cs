using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces;
public interface ISadhanaRepository : IRepository<ChantingRecord>
{
    void Update(ChantingRecord entity);
}

