using MedicalCareForm.Data;
using MedicalCareForm.Data.Models;
using MedicalCareForm.Share;
using MedicalCareForm.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalCareForm.Repositories
{
    public abstract class GenericRepository<T> : IRepository<T>, IDisposable where T : BaseEntity
    {
        protected readonly AppDbContext _сontextFactory;
        private bool _disposedValue;
        protected GenericRepository(AppDbContext contextFactory)
        {
            _сontextFactory = contextFactory;
        }

        //Получение всех актуальных данных IQueryable GetAll();
        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = _сontextFactory.Set<T>().AsNoTracking().Where(w => !w.IsDeleted);
            return query;
        }

        //Получение всех данных, в том числе и помеченных на удаление IQueryable GetAllWithDeleted();
        public virtual IQueryable<T> GetAllWithDeleted()
        {
            IQueryable<T> query = _сontextFactory.Set<T>();
            return query;
        }

        //Получение экземпляра по ключу (асинхронно) Task<T?> GetByKeyAsync(int id);
        public async Task<T?> GetByKeyAsync(int id)
        {
            return await _сontextFactory.Set<T>().FindAsync(id);
        }
        //Получение экземпляра по ключу(синхронно)T? GetByKey(int id);
        public virtual T? GetByKey(int id)
        {
            var query = _сontextFactory.Set<T>().Find(id);
            return query;
        }
        //Получение актуальной выборки по условию IQueryable FindBy(Expression<Func<T, bool>> predicate);
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _сontextFactory.Set<T>().AsNoTracking().Where(w => !w.IsDeleted).Where(predicate);
            return query;
        }
        //Получение полной выборки(в т.ч.удаленных) по условию Task<T?> FirstAsync(Expression<Func<T, bool>> predicate);
        public async Task<T?> FirstAsync(Expression<Func<T, bool>> predicate)
        {
            return await _сontextFactory.Set<T>().FirstOrDefaultAsync(predicate);
        }
        //Получение первого экземпляра по условию T? First(Expression<Func<T, bool>> predicate);
        public virtual T? First(Expression<Func<T, bool>> predicate)
        {
            return _сontextFactory.Set<T>().FirstOrDefault(predicate);
        }
        //Добавление данных Add(T entity);
        public virtual void Add(T entity)
        {
            entity.CreateDate = DateTime.Today;
            entity.EditDate = DateTime.Today;
            _сontextFactory.Set<T>().Add(entity);
        }
        //Удаление из БД Delete(T entity);
        public virtual void Delete(T entity)
        {
            _сontextFactory.Set<T>().Remove(entity);
        }
        //Пометка на удаление по объекту
        public virtual async Task VirtualDelete(T entity, int userId)
        {
            entity.IsDeleted = true;
            entity.DeleteDate = DateTime.Now;
            entity.DeletedUserId = userId;
            await SaveChangesAsync();
        }
        //Пометка на удаление по ключу объекта
        public virtual async Task VirtualDelete(int Id, int userId)
        {
            T? entity = _сontextFactory.Set<T>().FirstOrDefault(w => w.Id == Id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeleteDate = DateTime.Now;
                entity.DeletedUserId = userId;
                await SaveChangesAsync();
            }
        }
        //Изменение данных void Edit(T entity);
        public virtual void Edit(T entity)
        {
            var entry = _сontextFactory.Entry(entity);
            if (entry.State != EntityState.Added)
                entry.State = EntityState.Modified;
        }
        //Сохранение изменений(синхронно) void Save();
        public virtual void Save()
        {
            _сontextFactory.SaveChanges();
        }

        //Сохранение изменений(асинхронно) Task SaveChangesAsync();
        public async Task SaveChangesAsync()
        {
            await _сontextFactory.SaveChangesAsync();
        }
        //Поиск с пропуском
        public IQueryable<T> FindByWithTake(Expression<Func<T, bool>> predicate, int skip, int take)
        {
            IQueryable<T> query = _сontextFactory.Set<T>().AsNoTracking().Where(w => !w.IsDeleted).Where(predicate).Skip(skip).Take(take);
            return query;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _сontextFactory.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}