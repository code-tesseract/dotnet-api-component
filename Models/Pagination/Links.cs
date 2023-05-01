namespace Component.Models.Pagination;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
public class Links
{
	public Uri? Self  { get; set; }
	public Uri? First { get; set; }
	public Uri? Prev  { get; set; }
	public Uri? Next  { get; set; }
	public Uri? Last  { get; set; }

	public Links(Uri? self, Uri? first, Uri? prev, Uri? next, Uri? last)
	{
		Self  = self;
		First = first;
		Prev  = prev;
		Next  = next;
		Last  = last;
	}
}