using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using eWAY.Rapid.Internals.Enums;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace eWAY.Rapid.Internals.Services
{
    /// <summary>
    /// Internal Client class that does the invocation of Rapid native API call
    /// </summary>
    internal class RapidService : IRapidService
    {
        private string _rapidEndpoint;
        private string _authenticationHeader;
        private int? _version;
        private bool _isValidEndPoint;
        private bool _isValidCredentials;

        private const string ACCESS_CODES = "AccessCodes";
        private const string ACCESS_CODE_RESULT = "AccessCode/{0}";
        private const string CANCEL_AUTHORISATION = "CancelAuthorisation";
        private const string DIRECT_PAYMENT = "Transaction";
        private const string ACCESS_CODES_SHARED = "AccessCodesShared";
        private const string CAPTURE_PAYMENT = "CapturePayment";
        private const string REFUND_PAYMENT = "Transaction/{0}/Refund";
        private const string QUERY_TRANSACTION = "Transaction/{0}";
        private const string QUERY_CUSTOMER = "Customer/{0}";
        private const string TRANSACTION_FILTER_INVOICE_NUMBER = "Transaction/InvoiceNumber/{0}";
        private const string TRANSACTION_FILTER_INVOICE_REF = "Transaction/InvoiceRef/{0}";
        private const string SETTLEMENT_SEARCH = "Search/Settlement?{0}";


        public IMappingService MappingService { get; set; }
        
        public RapidService(string apiKey, string password, string endpoint)
        {
            SetCredentials(apiKey, password);
            SetRapidEndpoint(endpoint);
        }


        public async Task<DirectCancelAuthorisationResponse> CancelAuthorisation(DirectCancelAuthorisationRequest request)
        {
            return await JsonPost<DirectCancelAuthorisationRequest, DirectCancelAuthorisationResponse>(request, CANCEL_AUTHORISATION);
        }

        public async Task<DirectCapturePaymentResponse> CapturePayment(DirectCapturePaymentRequest request)
        {
            return await JsonPost<DirectCapturePaymentRequest, DirectCapturePaymentResponse>(request, CAPTURE_PAYMENT);
        }

        public async Task<CreateAccessCodeResponse> CreateAccessCode(CreateAccessCodeRequest request)
        {
            return await JsonPost<CreateAccessCodeRequest, CreateAccessCodeResponse>(request, ACCESS_CODES);
        }

        public async Task<CreateAccessCodeResponse> UpdateCustomerCreateAccessCode(CreateAccessCodeRequest request)
        {
            request.Method = Method.UpdateTokenCustomer;
            return await JsonPut<CreateAccessCodeRequest, CreateAccessCodeResponse>(request, ACCESS_CODES);
        }

        public async Task<CreateAccessCodeSharedResponse> CreateAccessCodeShared(CreateAccessCodeSharedRequest request)
        {
            return await JsonPost<CreateAccessCodeSharedRequest, CreateAccessCodeSharedResponse>(request, ACCESS_CODES_SHARED);
        }

        public async Task<CreateAccessCodeSharedResponse> UpdateCustomerCreateAccessCodeShared(CreateAccessCodeSharedRequest request)
        {
            request.Method = Method.UpdateTokenCustomer;
            return await JsonPut<CreateAccessCodeSharedRequest, CreateAccessCodeSharedResponse>(request, ACCESS_CODES_SHARED);
        }

        public async Task<GetAccessCodeResultResponse> GetAccessCodeResult(GetAccessCodeResultRequest request)
        {
            return await JsonGet<GetAccessCodeResultResponse>(string.Format(ACCESS_CODE_RESULT, request.AccessCode));
        }

        public async Task<DirectPaymentResponse> DirectPayment(DirectPaymentRequest request)
        {
            return await JsonPost<DirectPaymentRequest, DirectPaymentResponse>(request, DIRECT_PAYMENT);
        }

        public async Task<DirectPaymentResponse> UpdateCustomerDirectPayment(DirectPaymentRequest request)
        {
            request.Method = Method.UpdateTokenCustomer;
            return await JsonPut<DirectPaymentRequest, DirectPaymentResponse>(request, DIRECT_PAYMENT);
        }

        public async Task<DirectAuthorisationResponse> DirectAuthorisation(DirectAuthorisationRequest request)
        {
            return await JsonPost<DirectAuthorisationRequest, DirectAuthorisationResponse>(request, DIRECT_PAYMENT);
        }

        public async Task<DirectCustomerResponse> DirectCustomerCreate(DirectCustomerRequest request)
        {
            return await JsonPost<DirectCustomerRequest, DirectCustomerResponse>(request, DIRECT_PAYMENT);
        }

        public async Task<DirectCustomerSearchResponse> DirectCustomerSearch(DirectCustomerSearchRequest request)
        {
            return await JsonGet<DirectCustomerSearchResponse>(string.Format(QUERY_CUSTOMER, request.TokenCustomerID));
        }

        public async Task<DirectRefundResponse> DirectRefund(DirectRefundRequest request)
        {
            return await JsonPost<DirectRefundRequest, DirectRefundResponse>(request, string.Format(REFUND_PAYMENT, request.Refund.TransactionID));
        }

        public async Task<TransactionSearchResponse> QueryTransaction(long transactionID)
        {
            var method = string.Format(QUERY_TRANSACTION, transactionID);
            return await JsonGet<TransactionSearchResponse>(method);
        }

        public async Task<TransactionSearchResponse> QueryTransaction(string accessCode)
        {
            var method = string.Format(QUERY_TRANSACTION, accessCode);
            return await JsonGet<TransactionSearchResponse>(method);
        }

        public async Task<TransactionSearchResponse> QueryInvoiceRef(string invoiceRef)
        {
            var method = string.Format(TRANSACTION_FILTER_INVOICE_REF, invoiceRef);
            return await JsonGet<TransactionSearchResponse>(method);
        }

        public async Task<TransactionSearchResponse> QueryInvoiceNumber(string invoiceNumber)
        {
            var method = string.Format(TRANSACTION_FILTER_INVOICE_NUMBER, invoiceNumber);
            return await JsonGet<TransactionSearchResponse>(method);
        }

        public async Task<DirectSettlementSearchResponse> SettlementSearch(string request)
        {
            var method = string.Format(SETTLEMENT_SEARCH, request);
            return await JsonGet<DirectSettlementSearchResponse>(method);
        }

        public async Task<TResponse> JsonPost<TRequest, TResponse>(TRequest request, string method)
            where TRequest : class
            where TResponse : BaseResponse, new()
        {
            var jsonString = JsonConvert.SerializeObject(request,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new JsonConverter[] {new StringEnumConverter()}
                });

            var endpointUrl = _rapidEndpoint + method;
            // create a webrequest
            var webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl);
            var response = new TResponse();
            try
            {
                AddHeaders(webRequest, HttpMethods.POST.ToString());
                var result = await GetWebResponse(webRequest, jsonString);
                response = JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (WebException ex)
            {
                var errors = HandleWebException(ex);
                response.Errors = errors;
            }
            return response;
        }

        public async Task<TResponse> JsonPut<TRequest, TResponse>(TRequest request, string method)
            where TRequest : class
            where TResponse : BaseResponse, new()
        {
            var jsonString = JsonConvert.SerializeObject(request,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new JsonConverter[] { new StringEnumConverter() }
                });

            var endpointUrl = _rapidEndpoint + method;
            // create a webrequest
            var webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl);
            var response = new TResponse();
            try
            {
                //TODO:
                //This should be a PUT
                AddHeaders(webRequest, HttpMethods.POST.ToString());
                var result = await GetWebResponse(webRequest, jsonString);
                response = JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (WebException ex)
            {
                var errors = HandleWebException(ex);
                response.Errors = errors;
            }
            return response;
        }

        public async Task<TResponse> JsonGet<TResponse>(string method)
            where TResponse : BaseResponse, new()
        {
            var endpointUrl = _rapidEndpoint + method;
            // create a webrequest
            var webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl);
            var response = new TResponse();
            try
            {
                AddHeaders(webRequest, HttpMethods.GET.ToString());
                string result = await GetWebResponse(webRequest);
                if (String.IsNullOrEmpty(result))
                {
                    var errors = RapidSystemErrorCode.COMMUNICATION_ERROR;
                    response.Errors = errors;
                }
                else
                {
                    response = JsonConvert.DeserializeObject<TResponse>(result);
                }
            }
            catch (WebException ex)
            {
                var errors = HandleWebException(ex);
                response.Errors = errors;
            }

            return response;
        }

        private void AddHeaders(HttpWebRequest webRequest, string httpMethod)
        {
            // add authentication to request
            webRequest.Headers["Authorization"] = _authenticationHeader;
            webRequest.Headers["User-Agent"] = "eWAY SDK .NET " + typeof(RapidService).GetTypeInfo().Assembly.GetName().Version;
            webRequest.Method = httpMethod;
            webRequest.ContentType = "application/json";

            if (_version.HasValue)
            {
                webRequest.Headers["X-EWAY-APIVERSION"] = _version.ToString();
            }
        }

        private string HandleWebException(WebException ex)
        {
            if (ex.Response == null)
            {
                return RapidSystemErrorCode.COMMUNICATION_ERROR;
            }

            var errorCode = ((HttpWebResponse)ex.Response).StatusCode;

            if (errorCode == HttpStatusCode.Unauthorized ||
                errorCode == HttpStatusCode.Forbidden ||
                errorCode == HttpStatusCode.NotFound)
            {
                _isValidCredentials = false;
                return RapidSystemErrorCode.AUTHENTICATION_ERROR;
            }
            return RapidSystemErrorCode.COMMUNICATION_ERROR;
        }

        public virtual async Task<string> GetWebResponse(WebRequest webRequest, string content = null)
        {
            if (content != null)
            {
                using (new MemoryStream())
                {
                    using (var writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
                    {
                        writer.Write(content);
                        await writer.FlushAsync();
                    }
                }
            }

            string result;
            var webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null) return null;
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        public string GetRapidEndpoint()
        {
            return _rapidEndpoint;
        }

        public void SetRapidEndpoint(string value)
        {
            switch (value)
            {
                case "Production":
                    _rapidEndpoint = GlobalEndpoints.PRODUCTION;
                    break;
                case "Sandbox":
                    _rapidEndpoint = GlobalEndpoints.SANDBOX;
                    break;
                default:
                    if (!value.EndsWith("/"))
                    {
                        value += "/";
                    }
                    _rapidEndpoint = value;
                    break;
            }
            _isValidEndPoint = Uri.IsWellFormedUriString(_rapidEndpoint, UriKind.Absolute);
        }

        public void SetCredentials(string apiKey, string password)
        {
            _authenticationHeader = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":" + password));
            _isValidCredentials = !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(password);
        }

        public void SetVersion(int version)
        {
            _version = version;
        }

        public bool IsValid()
        {
            return _isValidCredentials && _isValidEndPoint;
        }

        public List<string> GetErrorCodes()
        {
            var errors = new List<string>();

            if (!_isValidEndPoint)
            {
                errors.Add(RapidSystemErrorCode.INVALID_ENDPOINT_ERROR);
            }

            if (!_isValidCredentials)
            {
                errors.Add(RapidSystemErrorCode.INVALID_CREDENTIAL_ERROR);
            }

            return errors;
        }
    }
}
