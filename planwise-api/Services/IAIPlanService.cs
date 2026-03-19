using PlanWiseApi.DTOs;

public interface IAIPlanService
{
    Task<ExpandPlanSuggestionsDto> ExpandPlanAsync(string title, string description);
}