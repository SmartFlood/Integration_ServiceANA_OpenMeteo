using System.Globalization;
using System.Text.Json;

namespace Integration_ANA_OpenMateo.OpenMeteo;

public static class OpenMeteoService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public static async Task<List<OpenMateoDTO>> GetOpenMateoAsync(string[] latitudes, string[] longitudes)
    {
        if (latitudes.Length != longitudes.Length)
        {
            throw new ArgumentException("Os arrays de latitude e longitude devem ter o mesmo tamanho.");
        }

        var forecasts = new List<OpenMateoDTO>();

        for (var i = 0; i < latitudes.Length; i++)
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitudes[i]}&longitude={longitudes[i]}&hourly=precipitation";

            try
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MyApp/1.0)");
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var forecast = JsonSerializer.Deserialize<OpenMateoDTO>(content);

                if (forecast != null)
                {
                    if (forecast.hourly?.time != null)
                    {
                        forecast.hourly.time = forecast.hourly.time
                            .Select(date => date.ToLocalTime())
                            .ToList();
                    }

                    forecasts.Add(forecast);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro na requisição para latitude {latitudes[i]}, longitude {longitudes[i]}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar resposta para latitude {latitudes[i]}, longitude {longitudes[i]}: {ex.Message}");
            }
        }

        return forecasts;
    }
}