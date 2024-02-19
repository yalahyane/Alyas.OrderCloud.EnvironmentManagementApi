namespace Alyas.OrderCloud.EnvironmentManagementApi.Models
{
    public class CloneModel
    {
        public string SourceApiUrl { get; set; }
        public string SourceClientId { get; set; }
        public string SourceSecret { get; set; }
        public string DestinationApiUrl { get; set; }
        public string DestinationClientId { get; set; }
        public string DestinationSecret { get; set; }
    }
}
