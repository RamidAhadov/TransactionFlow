using System.Net;
using Autofac;
using BenchmarkDotNet.Attributes;
using FluentResults;
using Microsoft.AspNetCore.Http;
using TransactionFlow.BillingSystem.DependencyResolvers.Autofac;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.Tests.Benchmark;

[MemoryDiagnoser]
public class IdempotencyServiceBenchmark
{
    private ILifetimeScope _scope;
    private IIdempotencyService _service;


    private HttpRequest _request;
    private HttpStatusCode _responseCode;
    private object _requestBody;
    private string? _responseBody;
    private int _iterationCount;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _scope = InitializeScope();
        _service = _scope.Resolve<IIdempotencyService>();

        _request = new DefaultHttpContext().Request;
        _request.Path.Add("/api/Customer/CreateCustomerAsync");
        _request.Method = HttpMethod.Post.ToString();
        
        _responseCode = HttpStatusCode.OK;
        _requestBody = new { Data = "{\"FirstName\":\"Murad\",\"MiddleName\":\"\",\"LastName\":\"Ahadov\",\"Address\":\"Azerbaijan, Baku\"}" };
        _responseBody = "{\"FirstName\":\"Murad\",\"MiddleName\":\"Not\"}";

        _iterationCount = 100;
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _scope.Dispose();
    }
    
    [IterationSetup]
    public void IterationSetup()
    {
        _iterationCount++;
        _request.Headers["Idempotency-key"] = $"{_iterationCount}";
    }

    [Benchmark]
    public Result<long> GenerateKey()
    {
        return _service.GenerateKey();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Keys))]
    public Result<string?> GetKey(string key)
    {
        return _service.Get(key);
    }
    
    public IEnumerable<string> Keys()
    {
        yield return "10";
        yield return "20";
        yield return "30";
        yield return "40";
        yield return "50";
    }

    [Benchmark]
    public Result Set()
    {
        return _service.Set(_request, _responseCode, _requestBody, _responseBody);
    }

    private static ILifetimeScope InitializeScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new AutofacBusinessModule());
        var container = builder.Build();
        return container.BeginLifetimeScope();
    }
}