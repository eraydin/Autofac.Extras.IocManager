﻿using System;
using System.Reflection;

using Autofac.Extras.IocManager.TestBase;

using FluentAssertions;

using Xunit;

namespace Autofac.Extras.IocManager.Tests
{
    public class IocManager_Tests : TestBaseWithIocBuilder
    {
        [Fact]
        public void IocManagerShouldWork()
        {
            Building(builder => { });

            LocalIocManager.Should().NotBeNull();
            LocalIocManager.Resolver.Should().NotBeNull();
        }

        [Fact]
        public void IocManager_SelfRegistration_ShouldWork()
        {
            Building(builder => { });

            var resolver = The<IIocResolver>();
            var managerByInterface = The<IIocManager>();
            var managerByClass = The<IocManager>();

            managerByClass.Should().BeSameAs(resolver);
            managerByClass.Should().BeSameAs(managerByInterface);
        }

        [Fact]
        public void IocManagerShould_Resolvedependency()
        {
            Building(builder => { builder.RegisterServices(f => f.Register<ISimpleDependency, SimpleDependency>(Lifetime.LifetimeScope)); });

            var simpleDependency = The<ISimpleDependency>();
            simpleDependency.Should().NotBeNull();
        }

        [Fact]
        public void IocManager_ShouldResolveDisposableDependency_And_Dispose_After_Scope_Finished()
        {
            Building(builder => { builder.RegisterServices(f => f.RegisterType<SimpleDisposableDependency>(Lifetime.LifetimeScope)); });

            SimpleDisposableDependency simpleDisposableDependency;
            using (IDisposableDependencyObjectWrapper<SimpleDisposableDependency> simpleDependencyWrapper = LocalIocManager.ResolveAsDisposable<SimpleDisposableDependency>())
            {
                simpleDisposableDependency = simpleDependencyWrapper.Object;
            }

            simpleDisposableDependency.DisposeCount.Should().Be(1);
        }

        [Fact]
        public void IocManager_ShouldInjectAnyDependecy()
        {
            IResolver resolver = Building(builder => { builder.RegisterServices(f => f.RegisterType<SimpleDependencyWithIocManager>(Lifetime.LifetimeScope)); });

            var dependencyWithIocManager = resolver.Resolve<SimpleDependencyWithIocManager>();

            dependencyWithIocManager.GetIocManager().Should().BeSameAs(LocalIocManager);
            dependencyWithIocManager.GetIocResolver().Should().BeSameAs(LocalIocManager);
        }

        [Fact]
        public void IocManager_ScopeShouldWork()
        {
            Building(builder => { builder.RegisterServices(f => f.RegisterType<SimpleDisposableDependency>(Lifetime.LifetimeScope)); });

            SimpleDisposableDependency simpleDisposableDependency;
            using (IIocScopedResolver iocScopedResolver = LocalIocManager.CreateScope())
            {
                simpleDisposableDependency = iocScopedResolver.Resolve<SimpleDisposableDependency>();
            }

            simpleDisposableDependency.DisposeCount.Should().Be(1);
        }

        [Fact]
        public void DefaultInterfaces_registration_should_work()
        {
            Building(builder => builder.RegisterServices(r => r.RegisterAssemblyByConvention(Assembly.Load(new AssemblyName("Autofac.Extras.IocManager.Tests")))));

            The<ICurrentUnitOfWorkProvider>().Should().NotBeNull();
        }

        internal class CallContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ITransientDependency
        {
        }

        internal interface ICurrentUnitOfWorkProvider
        {
        }

        internal interface ISimpleDependency
        {
        }

        internal class SimpleDependency : ISimpleDependency
        {
        }

        internal class SimpleDisposableDependency : IDisposable
        {
            public int DisposeCount { get; set; }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        internal class SimpleDependencyWithIocManager
        {
            private readonly IIocManager _iocManager;
            private readonly IIocResolver _iocResolver;

            public SimpleDependencyWithIocManager(IIocManager iocManager, IIocResolver iocResolver)
            {
                _iocManager = iocManager;
                _iocResolver = iocResolver;
            }

            public IIocManager GetIocManager()
            {
                return _iocManager;
            }

            public IIocResolver GetIocResolver()
            {
                return _iocResolver;
            }
        }
    }
}
