using System.Diagnostics;
using ImageConversionAPI.Dto;
using ImageConversionAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageConversionAPI.Controllers;

[ApiController]
[Route("convert")]
public class ImageConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    private readonly Dictionary<string, string> _formatToMimeMap = new Dictionary<string, string>()
    {
        ["PNG"] = "image/png",
        ["JPEG"] = "image/jpeg",
        ["WEBP"] = "image/webp",
        ["GIF"] = "image/gif",
        ["BMP"] = "image/bmp",
        ["TIFF"] = "image/tiff",
        ["PBM"] = "image/pbm",
        ["TGA"] = "image/tga",
        ["QOI"] = "image/qoi",
    };

    public ImageConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpPost]
    public async Task ConvertImage([FromForm] ConvertImageForm form, [FromQuery] ConvertImageQuery query,
        CancellationToken cancellationToken)
    {
        query.Format = query.Format.ToUpperInvariant();
        HttpContext.Response.ContentType = FormatToMimeMap(query.Format);
        await using var fileStream = form.File.OpenReadStream();

        var options = new ConversionOptions
        {
            Format = query.Format,
            Width = query.Width,
            Height = query.Height,
            Quality = query.Quality,
        };
        

        using var ms = new MemoryStream();
        var result = await _conversionService.Convert(fileStream, ms, options, cancellationToken);

        if (result == ConversionResult.Success)
        {
            HttpContext.Response.ContentLength = ms.Length;
            ms.Position = 0;
            await ms.CopyToAsync(HttpContext.Response.Body, cancellationToken);
            return;
        }

        HttpContext.Response.StatusCode = 400;
        HttpContext.Response.ContentType = "application/json";

        switch (result)
        {
            case ConversionResult.InvalidInputImage:
                await HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = "InvalidInputImage",
                    Message = "Invalid input image content",
                }, cancellationToken: cancellationToken);
                break;
            
            case ConversionResult.UnsupportedInputFormat:
                await HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = "UnsupportedInputFormat",
                    Message = "Unsupported input format",
                }, cancellationToken: cancellationToken);
                break;
            
            case ConversionResult.UnsupportedOutputFormat:
                await HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = "UnsupportedOutputFormat",
                    Message = "Unsupported output format",
                }, cancellationToken: cancellationToken);
                break;
            
            default:
                throw new UnreachableException();
        }
    }

    [HttpGet]
    public IList<string> GetSupportedFormats()
    {
        return _conversionService.SupportedFormats;
    }

    private string FormatToMimeMap(string format)
    {
        return _formatToMimeMap.GetValueOrDefault(format, "application/octet-stream");
    }
}