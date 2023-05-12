namespace EisRoutingService.Contracts;

public class Payload
{
    public List<ChangedAttributes> ChangedAttributes { get; set; } = new List<ChangedAttributes>();
    public object Content { get; set; } = string.Empty;
    public object OldContent { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string SourceSystemName { get; set; } = string.Empty;
}