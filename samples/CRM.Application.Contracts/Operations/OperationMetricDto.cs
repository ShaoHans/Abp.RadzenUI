namespace CRM.Operations;

public class OperationMetricDto
{
    public string Name { get; set; } = default!;

    public decimal Value { get; set; }

    public string? Color { get; set; }
}
