using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Update.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Update.Internal;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Tests.Update;

public class GaussDBModificationCommandBatchTest
{
    [Fact]
    public void AddCommand_returns_false_when_max_batch_size_is_reached()
    {
        var typeMapper = CreateTypeMapper();

        var batch = new GaussDBModificationCommandBatch(
            new ModificationCommandBatchFactoryDependencies(
                new RelationalCommandBuilderFactory(
                    TestServiceFactory.Instance.Create<RelationalCommandBuilderDependencies>(
                        (typeof(IRelationalTypeMappingSource), typeMapper),
                        (typeof(ExceptionDetector), new ExceptionDetector()),
                        (typeof(LoggingOptions), new LoggingOptions()))),
                new GaussDBSqlGenerationHelper(
                    new RelationalSqlGenerationHelperDependencies()),
                new GaussDBUpdateSqlGenerator(
                    new UpdateSqlGeneratorDependencies(
                        new GaussDBSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()),
                        typeMapper)),
                new CurrentDbContext(new FakeDbContext()),
                new FakeRelationalCommandDiagnosticsLogger(),
                new FakeDiagnosticsLogger<DbLoggerCategory.Update>()),
            maxBatchSize: 1);

        Assert.True(
            batch.TryAddCommand(
                CreateModificationCommand("T1", null, false)));
        Assert.False(
            batch.TryAddCommand(
                CreateModificationCommand("T1", null, false)));
    }

    private class FakeDbContext : DbContext;

    private static GaussDBTypeMappingSource CreateTypeMapper()
        => new(
            new TypeMappingSourceDependencies(
                new ValueConverterSelector(new ValueConverterSelectorDependencies()),
                new JsonValueReaderWriterSource(new JsonValueReaderWriterSourceDependencies()),
                []),
            new RelationalTypeMappingSourceDependencies([]),
            new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
            new GaussDBSingletonOptions());

    private static INonTrackedModificationCommand CreateModificationCommand(
        string name,
        string schema,
        bool sensitiveLoggingEnabled)
        => new ModificationCommandFactory().CreateNonTrackedModificationCommand(
            new NonTrackedModificationCommandParameters(name, schema, sensitiveLoggingEnabled));
}
