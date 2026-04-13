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

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Update;

public class GaussDBModificationCommandBatchFactoryTest
{
    [Fact]
    public void Uses_MaxBatchSize_specified_in_GaussDBOptionsExtension()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseGaussDB("Database=Crunchie", b => b.MaxBatchSize(1));

        var typeMapper = CreateTypeMapper();

        var factory = new GaussDBModificationCommandBatchFactory(
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
            optionsBuilder.Options);

        var batch = factory.Create();

        Assert.True(batch.TryAddCommand(CreateModificationCommand("T1", null, false)));
        Assert.False(batch.TryAddCommand(CreateModificationCommand("T1", null, false)));
    }

    [Fact]
    public void MaxBatchSize_is_optional()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseGaussDB("Database=Crunchie");

        var typeMapper = CreateTypeMapper();

        var factory = new GaussDBModificationCommandBatchFactory(
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
            optionsBuilder.Options);

        var batch = factory.Create();

        Assert.True(batch.TryAddCommand(CreateModificationCommand("T1", null, false)));
        Assert.True(batch.TryAddCommand(CreateModificationCommand("T1", null, false)));
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
