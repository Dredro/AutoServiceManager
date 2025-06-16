namespace Wpf.Services;

public class EditServiceCommand
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public decimal MinimalPrice { get; set; }
    public decimal MaximalPrice { get; set; }
}