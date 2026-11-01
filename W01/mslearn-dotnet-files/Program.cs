using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateSalesSummary(salesFiles, salesTotalDir);

File.AppendAllText(
    Path.Combine(salesTotalDir, "totals.txt"),
    $"{salesTotal}{Environment.NewLine}"
);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(
        folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string outputDirectory)
{
    StringBuilder report = new StringBuilder();

    double totalSales = 0;

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    report.AppendLine();
    report.AppendLine("Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        double fileTotal = data?.Total ?? 0;

        totalSales += fileTotal;

        string fileName = Path.GetFileName(file);

        report.AppendLine($"{fileName}: {fileTotal:C}");
    }

    report.Insert(
        report.ToString().IndexOf("Details:"),
        $" Total Sales: {totalSales:C}{Environment.NewLine}{Environment.NewLine}"
    );

    string reportPath = Path.Combine(outputDirectory, "sales-summary.txt");

    File.WriteAllText(reportPath, report.ToString());
}

record SalesData(double Total);