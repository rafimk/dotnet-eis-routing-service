namespace EisRoutingService.Options;

public class EisRoutingOption
{
    public ICollection<Rout> Routs { get; set; } = new List<Rout>();
}

public class Rout
{
    public string Name { get; set; } = string.Empty;
    public string OutboundTopic { get; set; } = string.Empty;
    public string InboundQueue { get; set; } = string.Empty;
}
