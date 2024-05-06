using System.Net;

namespace TransactionFlow.BillingSystem.Services.Abstraction;

public interface IIdempotencyService
{
    void Set(HttpRequest request, HttpStatusCode responseCode, object requestBody, string? responseBody = default);
    string? Get(string key);
    string GenerateKey();
    string GenerateKey(params string[] parameters);
}