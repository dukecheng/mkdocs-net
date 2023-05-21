using System.Linq.Expressions;
using LiteDB;

namespace Niusys.Docs.Core.DataStores
{
    public class MkdocsDatabase
    {
        private enum OperationMode
        {
            Read,
            Write
        }
        private readonly string _connString;
        private ManualResetEvent _writeLocker = new ManualResetEvent(true);
        public MkdocsDatabase(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public void StartupInspect(Action<ILiteDatabase> inspectAction)
        {
            DatabaseAction(inspectAction);
        }

        public IEnumerable<T> Search<T>(Expression<Func<T, bool>> predicate = null, int skip = 0, int limit = int.MaxValue, Func<IEnumerable<T>, IEnumerable<T>> filters = null)
        {
            if (predicate == null)
                predicate = x => true;
            return DatabaseAction(db =>
            {
                var query = db.GetCollection<T>(typeof(T).Name).Find(predicate, skip, limit);
                query = filters == null ? query : filters(query);
                return query.ToList();
            });
        }

        public T Get<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).FindOne(predicate));
        }

        public T Get<T>(BsonValue id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).FindById(id));
        }

        public void Update<T>(T item) => DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).Update(item));

        public void Insert<T>(T item) => DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).Insert(item));
        public void Insert<T>(List<T> items, int batchSize = 5000) => DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).InsertBulk(items, batchSize));

        public void Delete<T>(BsonValue id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).Delete(id));
        }
        public void Delete<T>(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).DeleteMany(predicate));
        }
        public int Truncate<T>()
        {
            return DatabaseAction(db => db.GetCollection<T>(typeof(T).Name).DeleteAll());
        }

        #region Utils
        private TResult DatabaseAction<TResult>(Func<ILiteDatabase, TResult> action, OperationMode operationMode = OperationMode.Write)
        {
            if (operationMode == OperationMode.Write)
            {
                _writeLocker.WaitOne();
            }

            var connectionString = new ConnectionString(_connString);
            connectionString.Connection = ConnectionType.Shared;
            using (var database = new LiteDatabase(connectionString))
            {
                var result = action(database);
                if (operationMode == OperationMode.Write)
                {
                    _writeLocker.Set();
                }
                return result;
            }
        }
        private void DatabaseAction(Action<ILiteDatabase> action, OperationMode operationMode = OperationMode.Write)
        {
            DatabaseAction(db =>
            {
                action(db);
                return string.Empty;
            }, operationMode);
        }
        #endregion
    }
}
