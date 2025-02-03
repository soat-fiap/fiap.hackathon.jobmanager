using System.Text.Json.Serialization;

namespace JobManager.Api.BackgroundService;

public class VideoFileNotificationMessage
{
    [JsonPropertyName("Records")] 
    public Record[] Items { get; set; }
}

public class Record
{
    [JsonPropertyName("s3")]
    public S3 VideoDetails { get; set; }
}

public class S3
{
    [JsonPropertyName("bucket")] public Bucket Bucket { get; set; }
    [JsonPropertyName("object")] public VideoObject File { get; set; }
}

public class Bucket
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }
    
    [JsonPropertyName("arn")] 
    public string Arn { get; set; }
}

public class VideoObject
{
    [JsonPropertyName("key")] 
    public string Key { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}