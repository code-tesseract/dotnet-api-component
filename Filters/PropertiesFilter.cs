// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Filters;

public class FilterProperties
{
    public string Field { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public object Value { get; set; } = null!;
}