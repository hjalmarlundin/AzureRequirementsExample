using System.Text;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Comment = Microsoft.TeamFoundation.SourceControl.WebApi.Comment;

namespace requirementsClient;
public class RequirementAzureClient
{
    private readonly VssConnection connection;

    public RequirementAzureClient(string orgUrl, string pat)
    {
        connection = new VssConnection(new Uri(orgUrl), new VssBasicCredential(string.Empty, pat));
    }

    public async Task<IEnumerable<WorkItem>> GetRequirementsFromAzure()
    {
        var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
        Wiql wiql = new()
        {
            Query = "Select [State], [Title] " +
                    "From WorkItems " +
                    "Where [Work Item Type] = 'Requirement' " +
                    "Order By [State] Asc, [Changed Date] Desc"
        };

        WorkItemQueryResult queryResult = await witClient.QueryByWiqlAsync(wiql);

        int[] workItemIds = queryResult.WorkItems.Select(wif => { return wif.Id; }).ToArray();

        IEnumerable<WorkItem> workItems = await witClient.GetWorkItemsAsync(workItemIds, expand: WorkItemExpand.All);
        return workItems;
    }

    public async Task UpdatePRWithRequirementsComment(WorkItem item, List<string> methods, int pullRequestId)
    {
        var comment = CreateComment(item, methods);
        await CreateCommentOnPullRequest(comment, pullRequestId);
    }

    public async Task UpdatePRWithUntestedRequirementsComment(WorkItem item, int pullRequestId)
    {
        var comment = CreateCommentOfUntestedRequirements(item);
        await CreateCommentOnPullRequest(comment, pullRequestId);
    }

    private static string CreateCommentOfUntestedRequirements(WorkItem item)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Requirements analysis example ");
        builder.AppendLine();
        builder.AppendLine("This repository contains the following requirements without any associated tests:");
        builder.AppendLine($"[Requirement {item.Id.Value}]({item.Url}): *{item.Fields.Single(x => x.Key == "System.Title").Value}*");
        var comment = builder.ToString();
        return comment;
    }

    private static string CreateComment(WorkItem item, List<string> methods)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Requirements analysis example ");
        builder.AppendLine("The modified files in this PR contains the following requirements:");
        builder.AppendLine();
        builder.AppendLine($"[Requirement {item.Id.Value}]({item.Url}): *{item.Fields.Single(x => x.Key == "System.Title").Value}*");
        builder.AppendLine();

        builder.AppendLine($"Which is tested in {string.Join(", ", methods)}");
        builder.AppendLine();
        builder.AppendLine("**PR reviewer:** Verify that the requirement and test is still valid");
        var comment = builder.ToString();
        return comment;
    }

    private async Task CreateCommentOnPullRequest(string content, int pullRequestId)
    {
        var gitClient = connection.GetClient<GitHttpClient>();
        var pullrequest = await gitClient.GetPullRequestByIdAsync(pullRequestId);
        Comment comment = new() { Author = new IdentityRef() { DisplayName = "RequirementsBot", UniqueName = "RequirementsBot" }, CommentType = CommentType.Text, Content = content, };
        var commentThread = new GitPullRequestCommentThread() { Comments = new List<Comment>() { comment }, Status = CommentThreadStatus.Active };
        await gitClient.CreateThreadAsync(commentThread, pullrequest.Repository.Id, pullRequestId);
    }

}