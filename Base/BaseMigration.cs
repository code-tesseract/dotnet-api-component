using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Component.Base;

public class BaseMigration<TEntity> : Migration where TEntity : class
{
	private readonly string _tableName;
	protected BaseMigration() => _tableName = typeof(TEntity).Name;

	protected void CreateTable<TColumns>(
		MigrationBuilder               builder,
		Func<ColumnsBuilder, TColumns> columns,
		string?                        schema  = null,
		string?                        comment = null
	) => builder.CreateTable(name: _tableName, columns: columns, schema: schema, comment: comment);

	protected void DropTable(MigrationBuilder builder, string? schema = null)
		=> builder.DropTable(_tableName, schema);

	protected void SetPrimaryKey(
		MigrationBuilder builder,
		object           column,
		string?          schema = null
	)
	{
		switch (column)
		{
			case string[] columns:
				builder.AddPrimaryKey(
					name: SetPrimaryKeyName(column),
					table: _tableName,
					columns: columns,
					schema: schema
				);
				break;
			case string columnString:
				builder.AddPrimaryKey(
					name: SetPrimaryKeyName(column),
					table: _tableName,
					column: columnString,
					schema: schema
				);
				break;
			default:
				throw new Exception("Invalid primary key format.");
		}
	}

	protected void SetIndex(
		MigrationBuilder builder,
		object           column,
		string?          schema     = null,
		bool             unique     = false,
		string?          filter     = null,
		bool[]?          descending = null
	)
	{
		switch (column)
		{
			case string[] columns:
			{
				builder.CreateIndex(
					name: SetIndexName(columns, unique),
					table: _tableName,
					columns: columns,
					schema: schema,
					unique: unique,
					filter: filter,
					descending: descending
				);
				break;
			}
			case string col:
				builder.CreateIndex(
					name: SetIndexName(col, unique),
					table: _tableName,
					column: col,
					schema: schema,
					unique: unique,
					filter: filter,
					descending: descending
				);
				break;
			default:
				throw new Exception("Invalid column data type.");
		}
	}

	public void SetForeignKey(
		MigrationBuilder  builder,
		object            column,
		string            principalTable,
		string?           schema          = null,
		string?           principalSchema = null,
		object?           principalColumn = null,
		ReferentialAction onUpdate        = ReferentialAction.Cascade,
		ReferentialAction onDelete        = ReferentialAction.NoAction
	)
	{
		switch (column)
		{
			case string[] columns when principalColumn is string[] principalColumns:
				builder.AddForeignKey(
					name: SetForeignKeyName(column, principalTable, principalColumn),
					table: _tableName,
					columns: columns,
					principalTable: principalTable,
					schema: schema,
					principalSchema: principalSchema,
					principalColumns: principalColumns,
					onUpdate: onUpdate,
					onDelete: onDelete
				);
				break;
			case string columnString when principalColumn is string principalColumnString:
				builder.AddForeignKey(
					name: SetForeignKeyName(column, principalTable, principalColumn),
					table: _tableName,
					column: columnString,
					principalTable: principalTable,
					schema: schema,
					principalSchema: principalSchema,
					principalColumn: principalColumnString,
					onUpdate: onUpdate,
					onDelete: onDelete
				);
				break;
			default:
				throw new Exception("Invalid foreign key format.");
		}
	}

	private string SetPrimaryKeyName(object column)
	{
		var table = _tableName;
		if (!string.IsNullOrEmpty(table) && table.Length > 1)
			table  = char.ToUpper(table[0]) + table[1..].ToLower();
		else table = char.ToUpper(table[0]).ToString();

		string columnString;
		if (column is string[] columns) columnString = string.Join('-', columns);
		else columnString                            = (string)column;

		return $"PK_{table}__{columnString}";
	}

	private string SetIndexName(object column, bool unique)
	{
		var indexPrefix = unique ? "UQ" : "IX";
		var table       = _tableName;

		if (!string.IsNullOrEmpty(table) && table.Length > 1)
			table  = $"{table[0].ToString().ToUpper() + table[1..].ToLower()}";
		else table = $"{table[0].ToString().ToUpper()}";

		string columnString;
		if (column is string[] columns) columnString = string.Join('-', columns);
		else columnString                            = (string)column;

		return $"{indexPrefix}_{table}__{columnString}";
	}

	private string SetForeignKeyName(object column, string principalTable, object principalColumn)
	{
		var table = _tableName;
		if (!string.IsNullOrEmpty(table) && table.Length > 1)
			table  = char.ToUpper(table[0]) + table[1..].ToLower();
		else table = char.ToUpper(table[0]).ToString();

		string columnString;
		if (column is string[] columns) columnString = string.Join('-', columns);
		else columnString                            = (string)column;

		string principalColumnString;
		if (principalColumn is string[] principalColumns) principalColumnString = string.Join('-', principalColumns);
		else principalColumnString                                              = (string)column;

		return $"FK_{table}__{columnString}_{principalTable}__{principalColumnString}";
	}

	protected override void Up(MigrationBuilder migrationBuilder)
	{
	}
}