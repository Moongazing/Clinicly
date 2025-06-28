using Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog.Logger;

public class PostgreSqlLogger : LoggerServiceBase
{
    public PostgreSqlLogger(PostgreSqlConfiguration postgreSqlConfig) : base(logger: null!)
    {
        if (postgreSqlConfig == null)
        {
            throw new Exception("PostgreSQL configuration cannot be null.");
        }

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter() },
            { "TimeStamp", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() }
        };

        Logger = new LoggerConfiguration()
            .WriteTo.PostgreSQL(
                connectionString: postgreSqlConfig.ConnectionString,
                tableName: postgreSqlConfig.TableName,
                columnOptions: columnWriters,
                needAutoCreateTable: postgreSqlConfig.AutoCreateSqlTable)
            .CreateLogger();
    }
}
