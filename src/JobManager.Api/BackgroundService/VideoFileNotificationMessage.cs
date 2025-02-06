using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace JobManager.Api.BackgroundService;

[ExcludeFromCodeCoverage]
public class VideoFileNotificationMessage
{
    [JsonPropertyName("Records")] 
    public Record[] Items { get; set; }
}

[ExcludeFromCodeCoverage]
public class Record
{
    [JsonPropertyName("s3")]
    public S3 VideoDetails { get; set; }
}

[ExcludeFromCodeCoverage]
public class S3
{
    [JsonPropertyName("bucket")] public Bucket Bucket { get; set; }
    [JsonPropertyName("object")] public VideoObject File { get; set; }
}

[ExcludeFromCodeCoverage]
public class Bucket
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }
    
    [JsonPropertyName("arn")] 
    public string Arn { get; set; }
}

[ExcludeFromCodeCoverage]
public class VideoObject
{
    [JsonPropertyName("key")] 
    public string Key { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}