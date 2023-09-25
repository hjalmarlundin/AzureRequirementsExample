using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.Common;
using requirementsClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        string orgUrl = args.ElementAtOrDefault(1) ?? "https://dev.azure.com/MyOrganization/";
        string pat = args.FirstOrDefault() ?? "ENTER PAT HERE";

        var pullRequestId = args.ElementAtOrDefault(2) != null ? int.Parse(args[2]) : 0;
        var fileNames = GetFileNames(args);
        Predicate<string> filter = fileNames.IsNullOrEmpty() ? x => true : x => fileNames.Contains(x);
        var requirementsFromCode = new RequirementAssemblyScanner().GetRequirementWithAssociatedTestCases(filter);
        var azureRequirementsClient = new RequirementAzureClient(orgUrl, pat);
        var requirementsFromAzure = await azureRequirementsClient.GetRequirementsFromAzure();
        var requirementsFromTests = requirementsFromAzure.Where(x => requirementsFromCode.ContainsKey(x.Id.Value));



        foreach (var requirement in requirementsFromTests)
        {
            var testMethods = requirementsFromCode[requirement.Id.Value];
            await azureRequirementsClient.UpdatePRWithRequirementsComment(requirement, testMethods, pullRequestId);
        }

        var requirementsWithoutAnyTests = requirementsFromAzure.Where(x => !requirementsFromCode.ContainsKey(x.Id.Value));

        foreach (var requirement in requirementsWithoutAnyTests)
        {
            await azureRequirementsClient.UpdatePRWithUntestedRequirementsComment(requirement, pullRequestId);
        }
    }

    private static IEnumerable<string> GetFileNames(string[] args)
    {
        if (!File.Exists("tmp.txt"))
        {
            Console.WriteLine("File does not exist");
            return Array.Empty<string>();
        }

        IEnumerable<string> lines = File.ReadLines("tmp.txt");
        List<string> trimmedLines = new List<string>();

        foreach (var line in lines)
        {
            var lineWithoutFolder = line.Split('/').Last();
            var lineWithoutFileEndning = lineWithoutFolder.Split('.').First();
            trimmedLines.Add(lineWithoutFileEndning);
        }
        Console.WriteLine(String.Join(Environment.NewLine, trimmedLines));
        return trimmedLines;
    }
}