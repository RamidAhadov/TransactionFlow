using System.Net;
using FluentResults;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IIdempotencyService
{
    Result Set(HttpRequest request, HttpStatusCode responseCode, object requestBody, string? responseBody = default);
    Result<string?> Get(string key);
    Result<long> GenerateKey();
}