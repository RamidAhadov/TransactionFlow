using AutoMapper;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.Business.Models;
using TransactionFlow.Entities.Concrete;

namespace TransactionFlow.BillingSystem.Utilities.AutoMapper;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Customer, CustomerModel>();
        CreateMap<CustomerModel, Customer>();
        
        CreateMap<CustomerAccountModel, CustomerAccount>();
        CreateMap<CustomerAccount, CustomerAccountModel>();
        
        CreateMap<CustomerDto, CustomerModel>();
        CreateMap<CustomerModel, CustomerDto>();
    }
}