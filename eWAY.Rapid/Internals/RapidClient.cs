using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;
using eWAY.Rapid.Internals.Services;
using eWAY.Rapid.Models;
using Microsoft.AspNetCore.WebUtilities;
using BaseResponse = eWAY.Rapid.Models.BaseResponse;

namespace eWAY.Rapid.Internals
{
    internal class RapidClient : IRapidClient
    {
        private readonly IRapidService _rapidService;
        private readonly IMappingService _mappingService;

        public List<string> ErrorCodes
        {
            get
            {
                return _rapidService.GetErrorCodes();
            }
        }

        public RapidClient(IRapidService rapidService)
        {
            _rapidService = rapidService;
            _mappingService = new MappingService();
        }

        public void SetCredentials(string apiKey, string password)
        {
            _rapidService.SetCredentials(apiKey, password);
        }

        public void SetVersion(int version)
        {
            _rapidService.SetVersion(version);
        }

        private async Task<CreateTransactionResponse> CreateInternal(PaymentMethod paymentMethod, Transaction transaction)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.Direct:
                    return await CreateTransaction<DirectPaymentRequest, DirectPaymentResponse>(_rapidService.DirectPayment, transaction);
                case PaymentMethod.TransparentRedirect:
                    return await CreateTransaction<CreateAccessCodeRequest, CreateAccessCodeResponse>(_rapidService.CreateAccessCode, transaction);
                case PaymentMethod.ResponsiveShared:
                    return await CreateTransaction<CreateAccessCodeSharedRequest, CreateAccessCodeSharedResponse>(_rapidService.CreateAccessCodeShared, transaction);
                case PaymentMethod.Authorisation:
                    return await CreateTransaction<DirectAuthorisationRequest, DirectAuthorisationResponse>(_rapidService.DirectAuthorisation, transaction);
                case PaymentMethod.Wallet:
                    return
                        await
                        (transaction.Capture
                            ? CreateTransaction<DirectPaymentRequest, DirectPaymentResponse>(
                                _rapidService.DirectPayment, transaction)
                            : CreateTransaction<DirectAuthorisationRequest, DirectAuthorisationResponse>(
                                _rapidService.DirectAuthorisation, transaction));
            }
            throw new NotSupportedException("Invalid PaymentMethod");
        }

        private async Task<CreateCustomerResponse> CreateInternal(PaymentMethod paymentMethod, Customer customer)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.Direct:
                    return await CreateCustomer<DirectPaymentRequest, DirectPaymentResponse>(_rapidService.DirectPayment, customer);
                case PaymentMethod.TransparentRedirect:
                    return await CreateCustomer<CreateAccessCodeRequest, CreateAccessCodeResponse>(_rapidService.CreateAccessCode, customer);
                case PaymentMethod.ResponsiveShared:
                    return await CreateCustomer<CreateAccessCodeSharedRequest, CreateAccessCodeSharedResponse>(_rapidService.CreateAccessCodeShared, customer);
            }
            throw new NotSupportedException("Invalid PaymentMethod");
        }

        private async Task<CreateCustomerResponse> UpdateInternal(PaymentMethod paymentMethod, Customer customer)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.Direct:
                    return await CreateCustomer<DirectPaymentRequest, DirectPaymentResponse>(_rapidService.UpdateCustomerDirectPayment, customer);
                case PaymentMethod.TransparentRedirect:
                    return await CreateCustomer<CreateAccessCodeRequest, CreateAccessCodeResponse>(_rapidService.UpdateCustomerCreateAccessCode, customer);
                case PaymentMethod.ResponsiveShared:
                    return await CreateCustomer<CreateAccessCodeSharedRequest, CreateAccessCodeSharedResponse>(_rapidService.UpdateCustomerCreateAccessCodeShared, customer);
            }
            throw new NotSupportedException("Invalid PaymentMethod");
        }


        public async Task<CreateTransactionResponse> Create(PaymentMethod paymentMethod, Transaction transaction)
        {
            if (!IsValid) return SdkInvalidStateErrorsResponse<CreateTransactionResponse>();
            var response = await CreateInternal(paymentMethod, transaction);
            return response;
        }

        public async Task<CreateCustomerResponse> Create(PaymentMethod paymentMethod, Customer customer)
        {
            if (!IsValid) return SdkInvalidStateErrorsResponse<CreateCustomerResponse>();
            var response = await CreateInternal(paymentMethod, customer);
            return response;
        }

        public async Task<CreateCustomerResponse> UpdateCustomer(PaymentMethod paymentMethod, Customer customer)
        {
            if (!IsValid) return SdkInvalidStateErrorsResponse<CreateCustomerResponse>();
            var response = await UpdateInternal(paymentMethod, customer);
            return response;
        }

        TResponse SdkInternalErrorsResponse<TResponse>() where TResponse : BaseResponse, new()
        {
            return new TResponse()
            {
                Errors = new List<string>(new[] { RapidSystemErrorCode.INTERNAL_SDK_ERROR })
            };
        }
        TResponse SdkInvalidStateErrorsResponse<TResponse>() where TResponse : BaseResponse, new()
        {
            return new TResponse()
            {
                Errors = _rapidService.GetErrorCodes()
            };
        }

        private async Task<CreateTransactionResponse> CreateTransaction<TRequest, TResponse>(Func<TRequest, Task<TResponse>> invoker, Transaction transaction)
        {
            var request = _mappingService.Map<Transaction, TRequest>(transaction);
            var response = await invoker(request);
            return _mappingService.Map<TResponse, CreateTransactionResponse>(response);
        }

        private async Task<CreateCustomerResponse> CreateCustomer<TRequest, TResponse>(Func<TRequest, Task<TResponse>> invoker, Customer customer)
        {
            var request = _mappingService.Map<Customer, TRequest>(customer);
            var response = await invoker(request);
            return _mappingService.Map<TResponse, CreateCustomerResponse>(response);
        }

        public async Task<QueryTransactionResponse> QueryTransaction(TransactionFilter filter)
        {
            if (!filter.IsValid)
            {
                return SdkInternalErrorsResponse<QueryTransactionResponse>();
            }

            var response = new TransactionSearchResponse();
            if (filter.IsValidTransactionID)
            {
                response = await _rapidService.QueryTransaction(filter.TransactionID);
            }
            else if (filter.IsValidAccessCode)
            {
                response = await _rapidService.QueryTransaction(filter.AccessCode);
            }
            else if (filter.IsValidInvoiceRef)
            {
                response = await _rapidService.QueryInvoiceRef(filter.InvoiceReference);
            }
            else if (filter.IsValidInvoiceNum)
            {
                response = await _rapidService.QueryInvoiceNumber(filter.InvoiceNumber);
            }

            return _mappingService.Map<TransactionSearchResponse, QueryTransactionResponse>(response);
        }

        public async Task<QueryTransactionResponse> QueryTransaction(int transactionId)
        {
            return await QueryTransaction(Convert.ToInt64(transactionId));
        }

        public async Task<QueryTransactionResponse> QueryTransaction(long transactionId)
        {
            var response = await _rapidService.QueryTransaction(transactionId);
            return _mappingService.Map<TransactionSearchResponse, QueryTransactionResponse>(response);
        }

        public async Task<QueryTransactionResponse> QueryTransaction(string accessCode)
        {
            var response = await _rapidService.QueryTransaction(accessCode);
            return _mappingService.Map<TransactionSearchResponse, QueryTransactionResponse>(response);
        }
        public async Task<QueryTransactionResponse> QueryInvoiceNumber(string invoiceNumber)
        {
            var response = await _rapidService.QueryInvoiceNumber(invoiceNumber);
            return _mappingService.Map<TransactionSearchResponse, QueryTransactionResponse>(response);
        }
        public async Task<QueryTransactionResponse> QueryInvoiceRef(string invoiceRef)
        {
            var response = await _rapidService.QueryInvoiceRef(invoiceRef);
            return _mappingService.Map<TransactionSearchResponse, QueryTransactionResponse>(response);
        }

        public async Task<QueryCustomerResponse> QueryCustomer(long tokenCustomerId)
        {
            var request = new DirectCustomerSearchRequest() { TokenCustomerID = tokenCustomerId.ToString() };
            var response = await _rapidService.DirectCustomerSearch(request);
            return _mappingService.Map<DirectCustomerSearchResponse, QueryCustomerResponse>(response);
        }

        public async Task<RefundResponse> Refund(Refund refund)
        {
            var request = _mappingService.Map<Refund, DirectRefundRequest>(refund);
            var response = await _rapidService.DirectRefund(request);
            return _mappingService.Map<DirectRefundResponse, RefundResponse>(response);
        }

        public async Task<CapturePaymentResponse> CapturePayment(CapturePaymentRequest captureRequest)
        {
            var request = _mappingService.Map<CapturePaymentRequest, DirectCapturePaymentRequest>(captureRequest);
            var response = await _rapidService.CapturePayment(request);
            return _mappingService.Map<DirectCapturePaymentResponse, CapturePaymentResponse>(response);
        }

        public async Task<CancelAuthorisationResponse> CancelAuthorisation(CancelAuthorisationRequest cancelRequest)
        {
            var request = _mappingService.Map<CancelAuthorisationRequest, DirectCancelAuthorisationRequest>(cancelRequest);
            var response = await _rapidService.CancelAuthorisation(request);
            return _mappingService.Map<DirectCancelAuthorisationResponse, CancelAuthorisationResponse>(response);
        }

        public async Task<SettlementSearchResponse> SettlementSearch(SettlementSearchRequest settlementSearchRequest)
        {
            if (!IsValid) return SdkInvalidStateErrorsResponse<SettlementSearchResponse>();
            var query = new Dictionary<string, string>();
            var properties = settlementSearchRequest.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var value = prop.GetValue(settlementSearchRequest, null);
                if (value != null && !String.IsNullOrWhiteSpace(value.ToString()))
                {
                    if ((!prop.Name.Equals("Page") && !prop.Name.Equals("PageSize")) || !value.Equals(0))
                    {
                        query[prop.Name] = value.ToString();
                    }
                }
            }

            var response = await _rapidService.SettlementSearch(QueryHelpers.AddQueryString(string.Empty, query).Substring(1));
            return _mappingService.Map<DirectSettlementSearchResponse, SettlementSearchResponse>(response);
        }

        public bool IsValid
        {
            get
            {
                return _rapidService.IsValid();
            }
        }

        public string RapidEndpoint
        {
            get { return _rapidService.GetRapidEndpoint(); }
            set { _rapidService.SetRapidEndpoint(value); }
        }
    }
}
