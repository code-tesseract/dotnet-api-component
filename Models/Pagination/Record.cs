﻿namespace Component.Models.Pagination;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
public class Record
{
	public int Current { get; set; }

	public int Total { get; set; }

	public Record(int current, int total)
	{
		Current = current;
		Total   = total;
	}
}