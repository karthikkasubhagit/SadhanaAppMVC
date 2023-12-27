using System.Linq.Expressions;

namespace SadhanaApp.Application.Common.Interfaces;
public interface ISadhanaRepository
{
    IEnumerable<ChantingRecord> GetAll(Expression<Func<ChantingRecord, bool>>? filter = null, string? includeProperties = null);
    ChantingRecord Get(Expression<Func<ChantingRecord, bool>> filter, string? includeProperties = null);
    void Add(ChantingRecord entity);
    void Update(ChantingRecord entity);
    void Delete(ChantingRecord entity);
    void Save();
}

