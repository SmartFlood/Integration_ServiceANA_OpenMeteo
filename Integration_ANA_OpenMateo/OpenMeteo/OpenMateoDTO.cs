namespace Integration_ANA_OpenMateo.OpenMeteo;

public class OpenMateoDTO
{
    public double? latitude { get; set; }
    public double? longitude { get; set; }
    public double? generationtime_ms { get; set; }
    public int? utc_offset_seconds { get; set; }
    public string? timezone { get; set; }
    public string? timezone_abbreviation { get; set; }
    public double? elevation { get; set; }
    public HourlyUnits? hourly_units { get; set; }
    public HourlyData? hourly { get; set; }
}

public class HourlyUnits
{
    public string? time { get; set; }
    public string? precipitation { get; set; }
}

public class HourlyData
{
    public List<DateTime>? time { get; set; }
    public List<double>? precipitation { get; set; }
}