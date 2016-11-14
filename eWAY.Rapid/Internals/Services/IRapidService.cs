using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;

namespace eWAY.Rapid.Internals.Services
{
    internal interface IRapidService
    {
        Task<DirectCancelAuthorisationResponse> CancelAuthorisation(DirectCancelAuthorisationRequest request);
        Task<DirectCapturePaymentResponse> CapturePayment(DirectCapturePaymentRequest request);
        Task<CreateAccessCodeResponse> CreateAccessCode(CreateAccessCodeRequest request);
        Task<CreateAccessCodeResponse> UpdateCustomerCreateAccessCode(CreateAccessCodeRequest request);
        Task<CreateAccessCodeSharedResponse> CreateAccessCodeShared(CreateAccessCodeSharedRequest request);
        Task<CreateAccessCodeSharedResponse> UpdateCustomerCreateAccessCodeShared(CreateAccessCodeSharedRequest request);
        Task<GetAccessCodeResultResponse> GetAccessCodeResult(GetAccessCodeResultRequest request);
        Task<DirectPaymentResponse> DirectPayment(DirectPaymentRequest request);
        Task<DirectPaymentResponse> UpdateCustomerDirectPayment(DirectPaymentRequest request);
        Task<DirectAuthorisationResponse> DirectAuthorisation(DirectAuthorisationRequest request);
        Task<DirectCustomerResponse> DirectCustomerCreate(DirectCustomerRequest request);
        Task<DirectRefundResponse> DirectRefund(DirectRefundRequest request);
        Task<DirectCustomerSearchResponse> DirectCustomerSearch(DirectCustomerSearchRequest request);
        Task<TransactionSearchResponse> QueryTransaction(long transactionID);
        Task<TransactionSearchResponse> QueryTransaction(string accessCode);
        Task<TransactionSearchResponse> QueryInvoiceRef(string invoiceRef);
        Task<TransactionSearchResponse> QueryInvoiceNumber(string invoiceNumber);
        Task<DirectSettlementSearchResponse> SettlementSearch(string request);

        string GetRapidEndpoint();
        void SetRapidEndpoint(string value);
        void SetCredentials(string apiKey, string password);
        void SetVersion(int version);
        bool IsValid();
        List<string> GetErrorCodes();
    }
}