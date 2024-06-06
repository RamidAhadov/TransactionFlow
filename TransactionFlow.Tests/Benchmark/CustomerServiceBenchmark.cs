using BenchmarkDotNet.Attributes;
using FluentResults;
using TransactionFlow.BillingSystem.DependencyResolvers.Autofac;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using Autofac;
using System;

namespace TransactionFlow.Tests.Benchmark
{
    [MemoryDiagnoser]
    public class CustomerServiceBenchmark
    {
        private ILifetimeScope _scope;
        private IAccountService _service;
        private CustomerDto _customerDto;
        private static readonly string[] FirstNames = { "John", "Jane", "Jim", "Jill", "Jack" };
        private static readonly string[] LastNames = { "Doe", "Smith", "Brown", "Johnson", "Williams" };
        private static readonly string[] Addresses = { "Baku, Azerbaijan", "New York, USA", "London, UK", "Paris, France", "Berlin, Germany" };
        private Random _random;

        [Params(9996, 9997, 9998, 9999, 1000)]
        public int CustomerId;

        [Params(20297,20298,20299,20300,20301)]
        public int AccountId;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _scope = InitializeScope();
            _service = _scope.Resolve<IAccountService>();
            _random = new Random();
            InitializeCustomerDto();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _scope.Dispose();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            Console.WriteLine($"Starting iteration with CustomerId: {CustomerId}");

            InitializeCustomerDto();
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            Console.WriteLine($"Finished iteration with CustomerId: {CustomerId}");
        }
        
        [Benchmark]
        public async Task<Result> CreateCustomerAsync()
        {
            return await _service.CreateCustomerAsync(_customerDto);
        }
        
        //[WarmupCount(1)]
        //[IterationCount(10)]
        [Benchmark]
        public Result UpdateCustomer()
        {
            return _service.UpdateCustomer(CustomerId, _customerDto);
        }
        
        [Benchmark] public Result CreateAccount()
        {
            return _service.CreateAccount(CustomerId);
        }
        
        [Benchmark] public async Task<Result> DeleteAccountAsync()
        {
            return await _service.DeleteAccountAsync(AccountId);
        }
        
        [Benchmark] public async Task<Result> DeleteCustomerAsync()
        {
            return await _service.DeleteCustomerAsync(CustomerId);
        }
        
        private static ILifetimeScope InitializeScope()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacBusinessModule());
            var container = builder.Build();
            return container.BeginLifetimeScope();
        }

        private void InitializeCustomerDto()
        {
            _customerDto = new CustomerDto
            {
                FirstName = FirstNames[_random.Next(FirstNames.Length)],
                LastName = LastNames[_random.Next(LastNames.Length)],
                Address = Addresses[_random.Next(Addresses.Length)]
            };
        }
    }
}
