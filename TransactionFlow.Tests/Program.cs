using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using TransactionFlow.Tests.Benchmark;


var config = DefaultConfig.Instance
    .WithOptions(ConfigOptions.DisableOptimizationsValidator)
    .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));
var summary = BenchmarkRunner.Run<IdempotencyServiceBenchmark>(config);