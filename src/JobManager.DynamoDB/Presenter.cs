using Amazon.DynamoDBv2.Model;
using JobManager.Domain.Entities;

namespace JobManager.DynamoDB;

public static class Presenter
{
    internal static Dictionary<string, AttributeValue> ToDynamoDbItem(this Job job)
    {
        return new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue(job.UserId.ToString()) },
            { "Id", new AttributeValue(job.Id.ToString()) },
            {
                "Snapshots", new AttributeValue { N = job.Snapshots.ToString() }
            },
            {
                "Status", new AttributeValue(job.Status.ToString())
            },
            {
                "SnapshotsProcessed", new AttributeValue { N = job.SnapshotsProcessed.ToString() }
            },
        };
    }
}