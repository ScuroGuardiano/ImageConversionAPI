using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ImageConversionAPI.Services;

using SixLabors.ImageSharp;

public class ImageSharpConversionService : IConversionService
{
    public async Task<ConversionResult> Convert(Stream input, Stream output, ConversionOptions options,
        CancellationToken cancellationToken = default)
    {
        var encoder = GetEncoder(options.Format, options.Quality);
        if (encoder is null)
        {
            return ConversionResult.UnsupportedOutputFormat;
        }

        try
        {
            using var image = await Image.LoadAsync(input, cancellationToken);
            if (options.Width != 0 || options.Height != 0)
            {
                image.Mutate(x => x.Resize(options.Width, options.Height));
            }

            await image.SaveAsync(output, encoder, cancellationToken);
            return ConversionResult.Success;
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case NotSupportedException or UnknownImageFormatException:
                    return ConversionResult.UnsupportedInputFormat;
                case InvalidImageContentException:
                    return ConversionResult.InvalidInputImage;
                default:
                    throw;
            }
        }
    }

    public List<string> SupportedFormats { get; } = ["PNG", "JPEG", "WEBP", "GIF", "BMP", "PBM", "TIFF", "TGA", "QOI"];

    private IImageEncoder? GetEncoder(string format, int? quality)
    {
        return format switch
        {
            "PNG" => new PngEncoder(),
            "JPEG" => quality is not null ? new JpegEncoder { Quality = quality.Value } : new JpegEncoder(),
            "WEBP" => quality is not null ? new WebpEncoder { Quality = quality.Value, FileFormat = WebpFileFormatType.Lossy } : new WebpEncoder(),
            "GIF" => new GifEncoder(),
            "BMP" => new BmpEncoder(),
            "PBM" => new PbmEncoder(),
            "TIFF" => new TiffEncoder(),
            "TGA" => new TgaEncoder(),
            "QOI" => new QoiEncoder(),
            _ => null
        };
    }
}