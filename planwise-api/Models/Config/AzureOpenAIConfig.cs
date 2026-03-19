namespace PlanWiseApi.Models.Config
{
    public class AzureOpenAIConfig
    {
        public string Endpoint { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string ChatDeployment { get; set; } = "";
        public string EmbeddingDeployment { get; set; } = "";
    }
}
