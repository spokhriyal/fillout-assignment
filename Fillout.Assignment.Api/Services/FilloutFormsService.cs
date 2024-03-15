using System.Text;
using Fillout.Assignment.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Fillout.Assignment.Api.Services;

public class FilloutFormsService : IFilloutFormsService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FilloutFormsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private string GetQueryStart(StringBuilder str)
    {
        return str.Length == 0 ? "?" : "&";
    }

    public async Task<FilloutFormSubmissionResponse> GetFormSubmissionsById(string formId, ExistingFilters existingFilters, List<Filter>? filters)
    {
        FilloutFormSubmissionResponse? response = new FilloutFormSubmissionResponse();

        var httpClient = _httpClientFactory.CreateClient("Fillout");

        var requestUrl = $"/v1/api/forms/{formId}/submissions";

        StringBuilder requestParams = new StringBuilder();
        var queryStarted = false;

        if (filters == null || filters.Count == 0)
        {
            if (existingFilters.Limit != null)
            {
                requestParams.Append($"{GetQueryStart(requestParams)}limit={existingFilters.Limit}");
            }

            if (existingFilters.Offset != 0)
            {
                requestParams.Append($"{GetQueryStart(requestParams)}offset={existingFilters.Offset}");
            }
        }

        if (existingFilters.AfterDate != null)
        {
            requestParams.Append($"{GetQueryStart(requestParams)}afterDate={existingFilters.AfterDate}");
        }
        if (existingFilters.BeforeDate != null)
        {
            requestParams.Append($"{GetQueryStart(requestParams)}beforeDate={existingFilters.BeforeDate}");
        }
        if (existingFilters.Status != null)
        {
            requestParams.Append($"{GetQueryStart(requestParams)}status={existingFilters.Status}");
        }
        if (existingFilters.IncludeEditLink != null)
        {
            requestParams.Append($"{GetQueryStart(requestParams)}includeEditLink={existingFilters.IncludeEditLink}");
        }
        if (existingFilters.Sort != "asc")
        {
            requestParams.Append($"{GetQueryStart(requestParams)}sort={existingFilters.Sort}");
        }
        var httpResponseMessage = await httpClient.GetAsync($"{requestUrl}{requestParams.ToString()}");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var contentString =
                await httpResponseMessage.Content.ReadAsStringAsync();

            response = JsonConvert.DeserializeObject<FilloutFormSubmissionResponse>(contentString);

            if (filters != null && filters.Count > 0 && response != null && response.Responses != null)
            {
                if (existingFilters.Limit == null)
                    existingFilters.Limit = 150;

                response = FilterRecords(response, filters);

                response.TotalResponses = response?.Responses?.Count;

                if (existingFilters.Offset != 0)
                {
                    var offset = existingFilters.Offset;
                    response.Responses = response.Responses.Skip(offset).Take(response.TotalResponses.Value - offset).ToList();
                }

                response.PageCount = (int)Math.Ceiling(response.TotalResponses.Value / (double)existingFilters.Limit);
            }
        }

        return response;
    }

    private FilloutFormSubmissionResponse FilterRecords(FilloutFormSubmissionResponse response, List<Filter> filters)
    {
        FilloutFormSubmissionResponse filteredresponse = new FilloutFormSubmissionResponse();
        var responses = new List<Response>();
        var isValid = false;

        foreach (var res in response.Responses)
        {
            isValid = false;
            foreach (var filter in filters)
            {
                var tempRes = res.Questions.FindAll(y => y.Id.ToLower() == filter.Id.ToLower());
                if (tempRes.Count > 0)
                {
                    switch (filter.Condition.ToLower())
                    {
                        case "equals":
                            tempRes = tempRes.FindAll(z => z.Value != null && z.Value.ToLower().Equals(filter.Value.ToLower()));
                            break;
                        case "does_not_equal":
                            tempRes = tempRes.FindAll(z => z.Value != null && !z.Value.ToLower().Equals(filter.Value.ToLower()));
                            break;
                        case "greater_than":
                            tempRes = tempRes.FindAll(
                               z => z.Value != null &&
                                    (
                                   (z.Type == "DatePicker" && Convert.ToDateTime(z.Value) > Convert.ToDateTime(filter.Value))
                                   ||
                                    (z.Type == "NumberInput" && Convert.ToInt32(z.Value) > Convert.ToInt32(filter.Value))
                                   )
                                );
                            break;
                        case "less_than":
                            tempRes = tempRes.FindAll(
                                z => z.Value != null &&
                                (
                                    (z.Type == "DatePicker" && Convert.ToDateTime(z.Value) < Convert.ToDateTime(filter.Value))
                                    ||
                                     (z.Type == "NumberInput" && Convert.ToDecimal(z.Value) < Convert.ToDecimal(filter.Value))
                                    )
                                 );
                            break;
                        // Taking only required conditions, Please add more conditions as needed...
                        default:
                            throw new ArgumentException($"Unsupported condition: {filter.Condition}");
                    }

                    if (tempRes.Count > 0)
                    {
                        isValid = true;
                        continue;
                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (isValid)
            {
                responses.Add(res);
            }
            else
            {
                continue;
            }
        }

        filteredresponse.Responses = responses;

        return filteredresponse;
    }
}