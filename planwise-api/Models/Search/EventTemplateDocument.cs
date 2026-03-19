using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace PlanWiseApi.Models.Search;

public class EventTemplateDocument
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = "";

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Title { get; set; } = "";

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public string Category { get; set; } = "";

    [SearchableField]
    public string Description { get; set; } = "";

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public string LocationType { get; set; } = "";

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public int SuggestedGroupSize { get; set; }

    [SearchableField]
    public string FoodIdeas { get; set; } = "";

    [SearchableField]
    public string Activities { get; set; } = "";

    [SearchableField]
    public string Decorations { get; set; } = "";

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public string BudgetLevel { get; set; } = "";

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public string Tags { get; set; } = "";

    [SearchableField]
    public string ContentText { get; set; } = "";

    [VectorSearchField(VectorSearchDimensions = 1536, VectorSearchProfileName = "vector-profile")]
    public float[] ContentVector { get; set; } = [];
}
