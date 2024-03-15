using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

namespace Fillout.Assignment.Api.Models;

public class ExistingFilters
{
    [Range(1, 100, ErrorMessage = "Limit must be a number between 1 and 150")]
    public int? Limit { get; set; } = null;

    public string? AfterDate { get; set; }

    public string? BeforeDate { get; set; }

    public int Offset { get; set; } = 0;

    public string? Status { get; set; }

    public bool? IncludeEditLink { get; set; }

    public string? Sort { get; set; } = "asc";
}

public class Filter
{
    public string? Id { get; set; }

    public string? Condition { get; set; }

    public string? Value { get; set; }
}