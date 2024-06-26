using AutoMapper;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.Business.Models;
using TransactionFlow.Business.Models.Archive;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Archive;

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

        CreateMap<Customer, CustomerArchive>()
            .ForMember(dest => dest.CustomerId, opt => opt
                .MapFrom(src => src.Id));

        CreateMap<CustomerAccount, CustomerAccountArchive>()
            .ForMember(dest => dest.LastBalance, opt => opt
                .MapFrom(src => src.Balance))
            .ForMember(dest => dest.WasMain, opt => opt
                .MapFrom(src => src.IsMain));

        CreateMap<CustomerModel, CustomerArchiveModel>()
            .ForMember(dest => dest.CustomerId, opt => opt
                .MapFrom(src => src.Id));
        
        CreateMap<CustomerArchiveModel, CustomerModel>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.CustomerId));

        CreateMap<CustomerAccountModel, CustomerAccountArchiveModel>()
            .ForMember(dest => dest.LastBalance, opt => opt
                .MapFrom(src => src.Balance))
            .ForMember(dest => dest.WasMain, opt => opt
                .MapFrom(src => src.IsMain));

        CreateMap<CustomerArchive, CustomerArchiveModel>();
        CreateMap<CustomerArchiveModel, CustomerArchive>();
        
        CreateMap<CustomerAccountArchive, CustomerAccountArchiveModel>();
        CreateMap<CustomerAccountArchiveModel, CustomerAccountArchive>();
        
        CreateMap<Transaction, TransactionModel>();
        CreateMap<TransactionModel, Transaction>();
        
        CreateMap<TransactionDetails, TransactionModel>();
        CreateMap<TransactionModel, TransactionDetails>();

        CreateMap<IdempotencyKey, IdempotencyKeyModel>();
        CreateMap<IdempotencyKeyModel, IdempotencyKey>();
        
        CreateMap<IdempotencyKeyModel, IdempotencyKeyDto>();
        CreateMap<IdempotencyKeyDto, IdempotencyKeyModel>();
    }
}