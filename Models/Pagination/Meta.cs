namespace Component.Models.Pagination;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
public class Meta
{
	public Record Record { get; set; }
	public Page   Page   { get; set; }
	public Links  Links  { get; set; }

	public Meta(Record record, Page page, Links links)
	{
		Record = record;
		Page   = page;
		Links  = links;
	}
}