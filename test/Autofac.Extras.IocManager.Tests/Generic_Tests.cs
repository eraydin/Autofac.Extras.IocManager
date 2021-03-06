﻿using System.Reflection;

using Autofac.Extras.IocManager.TestBase;

using FluentAssertions;

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
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IMyModuleRepository<,>), typeof(MyModuleRepositoryBase<,>)));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IMyModuleRepository<>), typeof(MyModuleRepositoryBase<>)));
                builder.RegisterServices(r => r.RegisterGeneric(typeof(IRepository<,>), typeof(EfRepositoryBase<,>)));
                builder.RegisterServices(r => r.RegisterAssemblyByConvention(Assembly.Load(new AssemblyName("Autofac.Extras.IocManager.Tests"))));
            });

            var myGenericResoulition = The<IRepository<MyClass, int>>();
        }

        [Fact]
        public void generic_registration_with_property_injection_should_work()
        {
            Building(builder =>
            {
                builder.RegisterServices(r =>
                {
                    r.RegisterGeneric(typeof(IProvider<>), typeof(Provider<>));
                    r.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
                });
            });

            The<IProvider<int>>().BullShitDependency.Should().NotBeNull();
        }

        public interface IProvider<T>
        {
            BullShitDependency BullShitDependency { get; set; }
        }

        public class Provider<T> : IProvider<T>
        {
            public BullShitDependency BullShitDependency { get; set; }
        }

        public class BullShitDependency : ITransientDependency
        {
        }

        public interface IRepository : ITransientDependency
        {
        }

        public interface IRepository<TEntity, TPrimaryKey> : IRepository
            where TEntity : class
        {
        }

        public interface IRepository<TEntity> : IRepository<TEntity, int>
            where TEntity : class
        {
        }

        public abstract class StoveRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
            where TEntity : class
        {
        }

        public class EfRepositoryBase<TEntity, TPrimaryKey> : StoveRepositoryBase<TEntity, TPrimaryKey>
            where TEntity : class
        {
        }

        public class EfRepositoryBase<TEntity> : StoveRepositoryBase<TEntity, int>
            where TEntity : class
        {
        }

        public interface IMyModuleRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
            where TEntity : class
        {
        }

        public interface IMyModuleRepository<TEntity> : IMyModuleRepository<TEntity, int>
            where TEntity : class
        {
        }

        public class MyModuleRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<TEntity, TPrimaryKey>, IMyModuleRepository<TEntity, TPrimaryKey>
            where TEntity : class
        {
        }

        public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, int>, IMyModuleRepository<TEntity>
            where TEntity : class
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
