using AutoMapper;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.BillingSystem.Utilities.AutoMapper;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Customer, CustomerModel>();
        CreateMap<CustomerModel, Customer>();
    }
}