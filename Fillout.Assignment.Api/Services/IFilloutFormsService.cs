using Fillout.Assignment.Api.Models;

namespace Fillout.Assignment.Api.Services;

public interface IFilloutFormsService
{
    Task<FilloutFormSubmissionResponse> GetFormSubmissionsById(string formId, ExistingFilters existingFilters, List<Filter>? filters);
}