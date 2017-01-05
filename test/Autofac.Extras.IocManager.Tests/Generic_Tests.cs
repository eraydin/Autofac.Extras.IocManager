﻿using System.Reflection;

using Xunit;

namespace Autofac.Extras.IocManager.Tests
{
    public class Generic_Tests : TestBaseWithIocBuilder
    {
        [Fact]
        public void GenericTests_Should_Work()
        {
            Building(builder =>
            {
                builder.RegisterServices(r => r.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly()));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IRepository<,>), typeof(StoveRepositoryBase<,>)));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IRepository<,>), typeof(EfRepositoryBase<,>)));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IRepository<>), typeof(EfRepositoryBase<>)));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IMyModuleRepository<,>), typeof(MyModuleRepositoryBase<,>)));
            });

            var myGenericResoulition = LocalIocManager.Resolve<IRepository<MyClass, int>>();
        }

        public interface IRepository : ITransientDependency
        {
        }

        public interface IRepository<TEntity, TPrimaryKey> : IRepository
            where TEntity : class
        {
        }

        public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class
        {
        }

        public abstract class StoveRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
        {
        }

        public abstract class StoveRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
        {
        }

        public class EfRepositoryBase<TEntity, TPrimaryKey> : StoveRepositoryBase<TEntity, TPrimaryKey> where TEntity : class
        {
        }

        public class EfRepositoryBase<TEntity> : StoveRepositoryBase<TEntity> where TEntity : class
        {
        }

        public interface IMyModuleRepository<TEntity> : IRepository<TEntity> where TEntity : class
        {
        }

        public interface IMyModuleRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
        {
        }

        public class MyModuleRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<TEntity, TPrimaryKey>, IMyModuleRepository<TEntity, TPrimaryKey> where TEntity : class
        {
        }

        public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, int> where TEntity : class, IMyModuleRepository<TEntity>
        {
        }

        public class MyClass
        {
        }

        public class MyContext
        {
        }
    }
}
