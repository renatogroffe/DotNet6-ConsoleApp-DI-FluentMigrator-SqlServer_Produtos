using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentMigrator.Runner;
using BaseProdutos;
using BaseProdutos.Utils;

Console.WriteLine("**** BaseProdutos no SQL Server - Execucao de Migrations ****");

if (args.Length != 1)
{
    ConsoleExtensions.ShowError(
        "Informe como unico parametro a string de conexao com o SQL Server!");
    return;
}

Console.WriteLine("Numero de produtos a serem incluidos na base:");
var qtdRegistrosCarga = Console.ReadLine();
if (String.IsNullOrWhiteSpace(qtdRegistrosCarga) ||
    !int.TryParse(qtdRegistrosCarga, out var qtdRegistros) ||
    qtdRegistros <= 0)
{
    ConsoleExtensions.ShowError(
        "Informe um numero de produtos maior do que zero para a carga dos dados!");
    return;
}

var services = new ServiceCollection();

Console.WriteLine("Configurando recursos...");

services.AddSingleton(new LoadDataConfigurations() { NumberOfInsertions = qtdRegistros });
services.AddLogging(configure => configure.AddConsole());

services.AddFluentMigratorCore()
    .ConfigureRunner(cfg => cfg
        .AddSqlServer()
        .WithGlobalConnectionString(args[0])
        .ScanIn(typeof(ConsoleApp).Assembly).For.Migrations()
    )
    .AddLogging(cfg => cfg.AddFluentMigratorConsole());

services.AddTransient<ConsoleApp>();

services.BuildServiceProvider()
    .GetService<ConsoleApp>()!.Run();