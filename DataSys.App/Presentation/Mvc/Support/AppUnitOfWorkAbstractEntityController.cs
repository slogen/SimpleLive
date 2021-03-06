﻿using System;
using DataSys.App.DataAccess;
using DataSys.App.DataModel;
using Scm.DataAccess;

namespace DataSys.App.Presentation.Mvc.Support
{
    public abstract class
        AppUnitOfWorkAbstractEntityController<TEntity> : AppUnitOfWorkAbstractEntityController<TEntity, TEntity>
        where TEntity: AbstractEntity
    {
        protected override TEntity ToProtocol(TEntity entity) => entity;
        protected override TEntity FromProtocol(TEntity item) => item;

        protected AppUnitOfWorkAbstractEntityController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public abstract class AppUnitOfWorkAbstractEntityController<TEntity, TResult> : UnitOfWorkAbstractEntityController<IAppUnitOfWork, TEntity, TResult>
        where TEntity: AbstractEntity
    {
        protected AppUnitOfWorkAbstractEntityController(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override IIdRepository<Guid, TEntity> Source => UnitOfWork.IdRepository<Guid, TEntity>(x => x.Id);
    }
}