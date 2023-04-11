using Microsoft.EntityFrameworkCore.Migrations;

namespace Component.Helpers;

public static class MigrationHelper
{
    public static void SetPrimaryKey(
        this MigrationBuilder builder,
        string table,
        object column,
        string? schema = null
    )
    {
        switch (column)
        {
            case string[] columns:
                builder.AddPrimaryKey(
                    name: SetPrimaryKeyName(table, column),
                    table: table,
                    columns: columns,
                    schema: schema
                );
                break;
            case string columnString:
                builder.AddPrimaryKey(
                    name: SetPrimaryKeyName(table, column),
                    table: table,
                    column: columnString,
                    schema: schema
                );
                break;
            default:
                throw new Exception("Invalid primary key format.");
        }
    }
    
    public static void SetIndex(
        this MigrationBuilder builder,
        string table,
        object column,
        string? schema = null,
        bool unique = false,
        string? filter = null,
        bool[]? descending = null
    )
    {
        switch (column)
        {
            case string[] columns:
            {
                builder.CreateIndex(
                    name: SetIndexName(table, columns, unique),
                    table: table,
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
                    name: SetIndexName(table, col, unique),
                    table: table,
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

    public static void SetForeignKey(
        this MigrationBuilder builder,
        string table,
        object column,
        string principalTable,
        string? schema = null,
        string? principalSchema = null,
        object? principalColumn = null,
        ReferentialAction onUpdate = ReferentialAction.Cascade,
        ReferentialAction onDelete = ReferentialAction.NoAction
    )
    {
        switch (column)
        {
            case string[] columns when principalColumn is string[] principalColumns:
                builder.AddForeignKey(
                    name: SetForeignKeyName(table, column, principalTable, principalColumn),
                    table: table,
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
                    name: SetForeignKeyName(table, column, principalTable, principalColumn),
                    table: table,
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

    private static string SetPrimaryKeyName(string table, object column)
    {
        if (!string.IsNullOrEmpty(table) && table.Length > 1)
            table = char.ToUpper(table[0]) + table[1..].ToLower();
        else table = char.ToUpper(table[0]).ToString();

        string columnString;
        if (column is string[] columns) columnString = string.Join('-', columns);
        else columnString = (string)column;

        return $"PK_{table}__{columnString}";
    }

    private static string SetIndexName(string table, object column, bool unique)
    {
        var indexPrefix = unique ? "UQ" : "IX";

        if (!string.IsNullOrEmpty(table) && table.Length > 1)
            table = $"{table[0].ToString().ToUpper() + table[1..].ToLower()}";
        else table = $"{table[0].ToString().ToUpper()}";

        string columnString;
        if (column is string[] columns) columnString = string.Join('-', columns);
        else columnString = (string)column;

        return $"{indexPrefix}_{table}__{columnString}";
    }

    private static string SetForeignKeyName(string table, object column, string principalTable, object principalColumn)
    {
        if (!string.IsNullOrEmpty(table) && table.Length > 1)
            table = char.ToUpper(table[0]) + table[1..].ToLower();
        else table = char.ToUpper(table[0]).ToString();

        string columnString;
        if (column is string[] columns) columnString = string.Join('-', columns);
        else columnString = (string)column;

        string principalColumnString;
        if (principalColumn is string[] principalColumns) principalColumnString = string.Join('-', principalColumns);
        else principalColumnString = (string)column;

        return $"FK_{table}__{columnString}_{principalTable}__{principalColumnString}";
    }
}