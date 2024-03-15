using System.Net.Http.Json;
using System.Text.Json;
using Fillout.Assignment.Api.Models;
using Fillout.Assignment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

namespace Fillout.Assignment.Api.Controllers;

[ApiController]
[Route("")]
public class FilloutFormsController : ControllerBase
{
    private readonly IFilloutFormsService _iFilloutFormsService;

    public FilloutFormsController(IFilloutFormsService iFilloutFormsService)
    {
        _iFilloutFormsService = iFilloutFormsService;
    }

    /// <summary>
    /// This API returns a list of weather forecasts.
    /// </summary>
    /// <remarks>
    /// 
    /// Just for demonstration
    /// 
    ///     limit=2&amp;offset=5&amp;filters=[{"id": "dSRAe3hygqVwTpPK69p5td", "condition": "greater_than", "value": "2024-01-01"}]'
    /// 
    ///     GET {formId}/filteredResponses
    ///     {
    ///     }
    ///     curl --location --globoff 'http://ec2-18-191-212-61.us-east-2.compute.amazonaws.com/cLZojxk94ous/filteredResponses?filters=[{%22id%22%3A%20%22dSRAe3hygqVwTpPK69p5td%22%2C%20%22condition%22%3A%20%22greater_than%22%2C%20%20%22value%22%3A%20%222024-01-01%22}]'
    ///
    /// </remarks>
    /// <param name="Filters">Additional Filters, Example: [{"id": "dSRAe3hygqVwTpPK69p5td", "condition": "greater_than", "value": "2024-01-01"}]</param>
    [HttpGet]
    [Route("{formId}/filteredResponses")]
    public async Task<IActionResult> GetFormSubmissionsById(string formId,
        [FromQuery] ExistingFilters? ExistingFilters,
        [FromQuery] List<Filter> Filters)
    {
        try
        {
            string filterStr = HttpContext.Request.Query["Filters"];
            var flts = new List<Filter>();
            if (!string.IsNullOrEmpty(filterStr))
            {
                flts = JsonConvert.DeserializeObject<List<Filter>>(filterStr);
            }

            var response = await _iFilloutFormsService.GetFormSubmissionsById(formId, ExistingFilters, flts);
            if (response.PageCount > 0)
            {
                return Ok(response);
            }
            else
            {
                return NoContent();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Some error occured. Please contact support.");
        }
    }

}