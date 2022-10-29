using Serilog;
using Serilog.Sinks.Elasticsearch;
using TestELK8;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilder, loggerConfiguration) =>
{
    var elasticsearchSettings = hostBuilder.Configuration.GetSection(nameof(ElasticsearchSettings)).Get<ElasticsearchSettings>();

    var envName = builder.Environment.EnvironmentName.ToLower().Replace(".", "-");
    var yourAppName = "ELK_Docker_Proto";
    var yourTemplateName = "your-template-name";

    loggerConfiguration
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchSettings.Url))
        {
            IndexFormat = $"{yourAppName}-{envName}-{DateTimeOffset.Now:yyyy-MM}",
            AutoRegisterTemplate = true,
            OverwriteTemplate = true,
            TemplateName = yourTemplateName,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            TypeName = null,
            BatchAction = ElasticOpType.Create
        });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");

app.Run();