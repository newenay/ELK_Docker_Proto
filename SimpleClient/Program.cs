using Serilog;
using Serilog.Sinks.Elasticsearch;
using TestELK8;
using CreateJSON;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    string checkForAppSettings = @".\appsettings.Local.json";
    //Console.WriteLine(File.Exists(checkForAppSettings) ? "File exists." : "File does not exist.");
    if( File.Exists(checkForAppSettings) ) {
        Console.WriteLine("File exists.");
    }
    else
    {
        Console.WriteLine("Need to create the file");
        CreateNewJSON();
    }
});

static void CreateNewJSON(){
    // May need switcher to write different types of appSettings
    CreateLocalJSON newJSON = new CreateLocalJSON
    {
        FirstName = null, 
        LastName = null,
        employeeID = 0,
        Designation = null 
    };

    string JSONresult = JsonSerializer.Serialize(newJSON);

    string path = @".\appsettings.Local.json";

    using (var tw = new StreamWriter(path, true))
    {
        tw.WriteLine(JSONresult.ToString());
        tw.Close();
    }



}

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");

app.Run();