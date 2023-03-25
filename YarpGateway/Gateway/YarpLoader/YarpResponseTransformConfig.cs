namespace Gateway.YarpExtensions
{
    public class YarpResponseTransformConfig : Dictionary<string, ResponseHeadersTransform>
    {
        public const string ConfigurationKey = "YarpResponseTransformConfig";
    }

    /// <summary>
    /// Keys are case insensitve as per Yarp's documentation
    /// </summary>
    public class ResponseHeadersTransform
    {
        public Dictionary<string, string> AddHeaders { get; set; } = new();
        public List<string> RemoveHeaders { get; set; } = new();
    }
}