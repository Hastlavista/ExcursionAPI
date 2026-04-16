using System;
using FluentMigrator.Runner.VersionTableInfo;

namespace BlueDragon.Excursion.DatabaseMigration.Models;

[VersionTableMetaData]
public class VersionTable : IVersionTableMetaData
{
    public string ColumnName => "version";
    public string SchemaName => "excursion";
    public string TableName => "version_info";
    public string UniqueIndexName => "UC_Version";
    public virtual string AppliedOnColumnName => "applied_on";
    public virtual string DescriptionColumnName => "description";
    public object ApplicationContext { get; set; }
    public bool OwnsSchema => true;
    public bool CreateWithPrimaryKey => false;
}
