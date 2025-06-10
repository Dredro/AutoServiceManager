namespace Wpf.Services;

public class CreateServiceCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public decimal MinimalPrice { get; set; }
    public  decimal MaximalPrice { get; set; }
}