namespace ImageConversionAPI.Configuration;

public class ConversionSettings
{
    public const string Key = "ConversionSettings";
    
    public IList<string> AllowedUploadExtensions { get; set; } = [];
}