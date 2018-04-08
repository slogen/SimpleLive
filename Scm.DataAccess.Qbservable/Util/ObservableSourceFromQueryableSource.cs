﻿using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ObservableSourceFromQueryableSource<TEntity> : AbstractObservableSourceFromQueryableSource<TEntity>
    {
        public override IQueryableSource<TEntity> Source { get; }
        public ObservableSourceFromQueryableSource(IQueryableSource<TEntity> source) { Source = source; }
    }
}
