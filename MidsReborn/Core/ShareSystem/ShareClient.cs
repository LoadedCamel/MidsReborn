using System;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using Mids_Reborn.Core.ShareSystem.RestModels;
using RestSharp.Serializers.NewtonsoftJson;

namespace Mids_Reborn.Core.ShareSystem
{
    internal static class ShareClient
    {
        private static RestClient Client
        {
            get
            {
                var options = new RestClientOptions("https://api.midsreborn.com") { MaxTimeout = -1, };
                var serializerOpt = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                return new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson(serializerOpt));
            }
        }

        public static async Task<OperationResult<TransactionResult>> SubmitBuild(BuildRecordDto buildDto)
        {
            try
            {
                var response = await Client.PostJsonAsync<BuildRecordDto, OperationResult<TransactionResult>>("build/submit", buildDto);
                switch (response)
                {
                    case { IsSuccessful: true }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Data = response.Data,
                            Message = response.Message
                        };
                    case { IsSuccessful: false }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Message = response.Message ?? "Error connecting to server or malformed response."
                        };
                }
            }
            catch (Exception e)
            {
                return new OperationResult<TransactionResult>
                {
                    IsSuccessful = false,
                    Message = $"Exception occurred: {e.Message}"
                };
            }

            return new OperationResult<TransactionResult>
            {
                IsSuccessful = false,
                Message = "Unexpected error occurred."
            };
        }

        public static async Task<OperationResult<TransactionResult>> UpdateBuild(BuildRecordDto buildDto, string id)
        {
            var request = new RestRequest($"build/update/{id}", Method.Patch);
            request.AddJsonBody(buildDto);

            try
            {
                var response = await Client.PatchAsync<OperationResult<TransactionResult>>(request);
                switch (response)
                {
                    case { IsSuccessful: true }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Data = response.Data,
                            Message = response.Message
                        };
                    case { IsSuccessful: false }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Message = response.Message ?? "Error connecting to server or malformed response."
                        };
                }
            }
            catch (Exception e)
            {
                return new OperationResult<TransactionResult>
                {
                    IsSuccessful = false,
                    Message = $"Exception occurred: {e.Message}"
                };
            }

            return new OperationResult<TransactionResult>
            {
                IsSuccessful = false,
                Message = "Unexpected error occurred."
            };
        }

        public static async Task<OperationResult<TransactionResult>> RefreshShare(string id)
        {
            var request = new RestRequest($"build/{id}/refresh", Method.Patch);

            try
            {
                var response = await Client.PatchAsync<OperationResult<TransactionResult>>(request);
                switch (response)
                {
                    case { IsSuccessful: true }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Data = response.Data,
                            Message = response.Message
                        };
                    case { IsSuccessful: false }:
                        return new OperationResult<TransactionResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Message = response.Message ?? "Error connecting to server or malformed response."
                        };
                }
            }
            catch (Exception e)
            {
                return new OperationResult<TransactionResult>
                {
                    IsSuccessful = false,
                    Message = $"Exception occurred: {e.Message}"
                };
            }

            return new OperationResult<TransactionResult>
            {
                IsSuccessful = false,
                Message = "Unexpected error occurred."
            };
        }

        public static async Task<OperationResult<FetchResult>> FetchData(string id)
        {
            try
            {
                var response = await Client.GetJsonAsync<OperationResult<FetchResult>>($"build/{id}/fetch");
                switch (response)
                {
                    case { IsSuccessful: true }:
                        return new OperationResult<FetchResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Data = response.Data,
                            Message = response.Message
                        };
                    case { IsSuccessful: false }:
                        return new OperationResult<FetchResult>
                        {
                            IsSuccessful = response.IsSuccessful,
                            Message = response.Message ?? "Error connecting to server or malformed response."
                        };
                }
            }
            catch (Exception e)
            {
                return new OperationResult<FetchResult>
                {
                    IsSuccessful = false,
                    Message = $"Exception occurred: {e.Message}"
                };
            }

            return new OperationResult<FetchResult>
            {
                IsSuccessful = false,
                Message = "Unexpected error occurred."
            };
        }

        public static async Task<SchemaData?> GetBuild(string code)
        {
            var importResponse = await Client.GetJsonAsync<SchemaData>($"build/{code}");
            return importResponse;
        }
    }
}
