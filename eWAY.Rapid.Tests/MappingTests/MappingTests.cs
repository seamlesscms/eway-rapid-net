using System.Linq;
using AutoMapper;
using eWAY.Rapid.Internals.Models;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;
using eWAY.Rapid.Internals.Services;
using eWAY.Rapid.Models;
using Xunit;
using BaseResponse = eWAY.Rapid.Internals.Response.BaseResponse;
using CardDetails = eWAY.Rapid.Models.CardDetails;
using Customer = eWAY.Rapid.Models.Customer;
using LineItem = eWAY.Rapid.Models.LineItem;
using Option = eWAY.Rapid.Internals.Models.Option;
using Payment = eWAY.Rapid.Internals.Models.Payment;
using Refund = eWAY.Rapid.Models.Refund;
using ShippingAddress = eWAY.Rapid.Models.ShippingAddress;
using VerificationResult = eWAY.Rapid.Models.VerificationResult;

namespace eWAY.Rapid.Tests.MappingTests
{
    
    public class MappingTests
    {
        readonly IMappingService _mappingService = new MappingService();

        [Fact]
        public void Transaction_To_DirectPaymentRequest_Test()
        { 
            var source = TestUtil.CreateTransaction();
            var dest = _mappingService.Map<Transaction, DirectPaymentRequest>(source);
            Assert.Equal(source.CustomerIP, dest.CustomerIP);
        }

        [Fact]
        public void ErrorMapping_NoError_Test()
        {
            var source = new BaseResponse()
            {
                Errors = null
            };
            var dest = _mappingService.Map<BaseResponse,
                Models.BaseResponse>(source);
            Assert.Null(dest.Errors);
        }

        [Fact]
        public void ErrorMapping_Test()
        {
            var source = new BaseResponse()
            {
                Errors = "D4401,D4403,D4404"
            };

            var dest = _mappingService.Map<BaseResponse,
                Models.BaseResponse>(source);
            Assert.Equal(dest.Errors.Count, 3);
            Assert.Equal(dest.Errors[0], "D4401");
            Assert.Equal(dest.Errors[1], "D4403");
            Assert.Equal(dest.Errors[2], "D4404");
        }

        [Fact]
        public void ErrorMapping_Inheritance_Test()
        {
            var source = TestUtil.CreateDirectPaymentResponse();
            source.Errors = "D4401,D4403,D4404";

            var dest = _mappingService.Map<DirectPaymentResponse, CreateTransactionResponse>(source);
            Assert.Equal(dest.Errors.Count, 3);
            Assert.Equal(dest.Errors[0], "D4401");
            Assert.Equal(dest.Errors[1], "D4403");
            Assert.Equal(dest.Errors[2], "D4404");
        }


        [Fact]
        public void DirectPaymentResponse_To_CreateTransactionResponse_Test()
        {
            var source = TestUtil.CreateDirectPaymentResponse();
            var dest = _mappingService.Map<DirectPaymentResponse, CreateTransactionResponse>(source);
            Assert.Equal(source.TransactionStatus, dest.TransactionStatus.Status);
        }

        [Fact]
        public void TokenCustomer_To_Customer()
        {
            var b2bSource = _mappingService.Map<DirectTokenCustomer, Customer>(new DirectTokenCustomer()
            {
                CardDetails = new Internals.Models.CardDetails()
                {
                    Number = "444433XXXXXX1111",
                    Name = "John Smith",
                    ExpiryMonth = "12",
                    ExpiryYear = "25",
                    StartMonth = null,
                    StartYear = null,
                    IssueNumber = null
                },
                TokenCustomerID = null,
                Reference = "A12345",
                Title = "Mr.",
                FirstName = "John",
                LastName = "Smith",
                CompanyName = "Demo Shop 123",
                JobDescription = "Developer",
                Street1 = "Level 5",
                Street2 = "369 Queen Street",
                City = "Sydney",
                State = "NSW",
                PostalCode = "2000",
                Country = "au",
                Email = "demo@example.org",
                Phone = "09 889 0986",
                Mobile = "09 889 6542",
                Comments = "",
                Fax = "",
                Url = "http://www.ewaypayments.com"
            });
            Assert.NotNull(b2bSource.Address);
        }

        [Fact]
        public void CreateAccessCodeResponse_To_CreateTransactionResponse()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BaseResponse, Rapid.Models.BaseResponse>()
                    .ForMember(dest => dest.Errors,
                        opt =>
                            opt.ResolveUsing(
                                s => !string.IsNullOrWhiteSpace(s.Errors) ? s.Errors.Split(',').ToList() : null));
                cfg.CreateMap<CreateAccessCodeResponse, CreateTransactionResponse>()
                    .BeforeMap((s, d) =>
                    {
                        //1
                        d.Transaction = new Transaction();
                    })
                    .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src))
                    .IncludeBase<BaseResponse, Rapid.Models.BaseResponse>();

                cfg.CreateMap<CreateAccessCodeResponse, Transaction>()
                    .BeforeMap((s, d) =>
                    {
                        //2
                    })
                    .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                    .ForMember(dest => dest.PaymentDetails, opt => opt.MapFrom(src => src.Payment));
                cfg.CreateMap<TokenCustomer, Customer>()
                    .BeforeMap((s, d) =>
                    {
                        //3
                    })
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src))
                    .ReverseMap();
                cfg.CreateMap<DirectTokenCustomer, Customer>()
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src))
                    .ReverseMap();
                cfg.CreateMap<DirectTokenCustomer, Address>()
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                    .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                    .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                    .ForMember(dest => dest.Street1, opt => opt.MapFrom(src => src.Street1))
                    .ForMember(dest => dest.Street2, opt => opt.MapFrom(src => src.Street2)).ReverseMap();


                cfg.CreateMap<TokenCustomer, Address>()
                    .BeforeMap((s, d) =>
                    {
                        //4
                    })
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                    .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                    .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                    .ForMember(dest => dest.Street1, opt => opt.MapFrom(src => src.Street1))
                    .ForMember(dest => dest.Street2, opt => opt.MapFrom(src => src.Street2)).ReverseMap();

                cfg.CreateMap<PaymentDetails, Payment>().ReverseMap();


                /*cfg.CreateMap<Customer, TokenCustomer>()
                    .ForMember(dest => dest.CardExpiryMonth, opt => opt.MapFrom(src => src.CardDetails.ExpiryMonth))
                    .ForMember(dest => dest.CardExpiryYear, opt => opt.MapFrom(src => src.CardDetails.ExpiryYear))
                    .ForMember(dest => dest.CardIssueNumber, opt => opt.MapFrom(src => src.CardDetails.IssueNumber))
                    .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.CardDetails.Name))
                    .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.CardDetails.Number))
                    .ForMember(dest => dest.CardStartMonth, opt => opt.MapFrom(src => src.CardDetails.StartMonth))
                    .ForMember(dest => dest.CardStartYear, opt => opt.MapFrom(src => src.CardDetails.StartYear))
                    .IncludeBase<Customer, Models.Customer>()
                    .ReverseMap();*/

                cfg.CreateMap<Customer, DirectTokenCustomer>().IncludeBase<Customer, TokenCustomer>();
                cfg.CreateMap<ShippingAddress, Models.ShippingAddress>().ReverseMap();
                cfg.CreateMap<LineItem, Models.LineItem>().ReverseMap();
                cfg.CreateMap<Rapid.Models.Option, Option>().ReverseMap();
                cfg.CreateMap<PaymentDetails, Payment>().ReverseMap();
                cfg.CreateMap<CardDetails, Models.CardDetails>().ReverseMap();
                cfg.CreateMap<Models.VerificationResult, Verification>().ReverseMap();
                cfg.CreateMap<VerificationResult, Verification>().ReverseMap();
                cfg.CreateMap<VerificationResult, Models.VerificationResult>().ReverseMap();
                cfg.CreateMap<Rapid.Models.Payment, Payment>().ReverseMap();
                cfg.CreateMap<Rapid.Models.SettlementSummary, Models.SettlementSummary>().ReverseMap();
                cfg.CreateMap<Rapid.Models.SettlementTransaction, Models.SettlementTransaction>().ReverseMap();
                cfg.CreateMap<Rapid.Models.BalanceSummaryPerCardType, Models.BalanceSummaryPerCardType>().ReverseMap();
            }).CreateMapper();

            CreateAccessCodeResponse input = new CreateAccessCodeResponse()
            {
                AccessCode = "1234",
                AmexECEncryptedData = "amex1234",
                CompleteCheckoutURL = "checkoutlurl",
                FormActionURL = "form action url",
            };
            input.Customer = new TokenCustomer()
            {
                CardExpiryMonth = "12",
                TokenCustomerID = 12354L,
                Country = "au",
                Street1 = "street 1",
                State = "vic",
                City = "some town",
                PostalCode = "1234",
                CardExpiryYear = "2099",
                CardIssueNumber = "1234",
                CardName = "some name",
                Street2 = "street 2",
            };

            var output = mapper.Map<CreateTransactionResponse>(input);

            Assert.NotNull(output.Transaction);
            Assert.NotNull(output.Transaction.Customer);
            Assert.NotNull(output.Transaction.Customer.Address);
        }
    }
}
