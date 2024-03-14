using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKToolKit.Spectre.AutoCompletion.Tests.Utilities.DependencyInjection;

public class CustomTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public CustomTypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new CustomTypeResolver(_builder.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _builder.AddSingleton(service, _ => func());
    }
}

public sealed class CustomTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public CustomTypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public object? Resolve(Type? type)
    {
        return type == null ? null : _provider.GetService(type);
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}