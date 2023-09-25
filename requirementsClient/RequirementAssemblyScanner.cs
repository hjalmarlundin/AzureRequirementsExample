namespace requirementsClient;

using System.Reflection;
using tests;

public class RequirementAssemblyScanner
{
    public Dictionary<int, List<string>> GetRequirementWithAssociatedTestCases(Predicate<string> filter)
    {
        var dict = new Dictionary<int, List<string>>();
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(y => filter(y.Name))))
        {
            var classMethods = type.GetMethods().Where(x => x.GetCustomAttribute(typeof(Requirement)) != null);
            foreach (var method in classMethods)
            {
                var requirementAttribute = method.GetCustomAttribute<Requirement>();
                var testAndClassName = type.Name + "_" + method.Name;
                AddOrUpdate(dict, requirementAttribute.Number, testAndClassName);
            }
        }
        return dict;
    }

    private static void AddOrUpdate(Dictionary<int, List<string>> dict, int requirementNumber, string testAndClassName)
    {
        if (dict.TryGetValue(requirementNumber, out var methodNames))
        {
            methodNames.Add(testAndClassName);
        }
        else
        {
            dict.Add(requirementNumber, new List<string> { testAndClassName });
        }
    }
}