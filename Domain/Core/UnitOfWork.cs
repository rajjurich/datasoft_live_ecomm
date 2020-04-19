using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
    public class UnitOfWork : IUnitOfWork
    {
        readonly DbContext _entitiesContext;
        DbContextTransaction _transaction;
        public UnitOfWork(DbContext entitiesContext)
        {
            if (entitiesContext == null)
            {
                throw new ArgumentNullException("entitiesContext");
            }
            _entitiesContext = entitiesContext;            
        }

        public void BeginTransaction()
        {
            _transaction = _entitiesContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
