using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ImageConversionAPI.Dto;

public class ConvertImageQuery
{
    [FromQuery(Name = "w")]
    [Range(0, 60000)]
    public int Width { get; set; }
    
    [FromQuery(Name = "h")]
    [Range(0, 60000)]
    public int Height { get; set; }
    
    [FromQuery(Name = "q")]
    [Range(0, 100)]
    public int? Quality { get; set; }
    
    [FromQuery(Name = "f")]
    public string Format { get; set; } = "JPEG";
}
