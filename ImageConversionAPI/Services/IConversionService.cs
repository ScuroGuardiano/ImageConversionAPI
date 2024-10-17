namespace ImageConversionAPI.Services;

public class ConversionOptions
{
    public required string Format { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int? Quality { get; set; }
}

public enum ConversionResult
{
    Success = 0,
    UnsupportedInputFormat = 1,
    UnsupportedOutputFormat = 2,
    InvalidInputImage = 3,
}

public interface IConversionService
{
    public Task<ConversionResult> Convert(Stream input, Stream output, ConversionOptions options,
        CancellationToken cancellationToken);

    public List<string> SupportedFormats { get; }
}