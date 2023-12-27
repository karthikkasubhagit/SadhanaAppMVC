using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SadhanaApp.Persistence.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _db;

        public ServiceRepository(AppDbContext db)
        {
            _db = db;
        }
        public void Add(ServiceType entity)
        {
            _db.Add(entity);
        }

        public void Delete(ServiceType entity)
        {
            _db.Remove(entity);
        }

        public ServiceType Get(Expression<Func<ServiceType, bool>> filter, string? includeProperties = null)
        {
            IQueryable<ServiceType> query = _db.Set<ServiceType>();

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

        public IEnumerable<ServiceType> GetAll(Expression<Func<ServiceType, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<ServiceType> query = _db.Set<ServiceType>();

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

        public void Update(ServiceType entity)
        {
            _db.ServiceTypes.Update(entity);
        }
    }
}
