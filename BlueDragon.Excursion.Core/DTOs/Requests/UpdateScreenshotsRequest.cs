using System;

namespace BlueDragon.Excursion.Core.DTOs.Requests;

public class UpdateScreenshotsRequest
{
    public Guid? Id { get; set; }
    public string ScreenshotBefore { get; set; }
    public string ScreenshotAfter { get; set; }
}