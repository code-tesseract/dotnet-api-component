namespace Component.Models.Pagination;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
public class Page
{
	public int Current { get; set; }
	public int Total   { get; set; }

	public Page(int current, int total)
	{
		Current = current;
		Total   = total;
	}
}