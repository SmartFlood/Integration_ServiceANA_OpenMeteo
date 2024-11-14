namespace Integration_ANA_OpenMateo.OpenMeteo;

public static class ProcessamentoOpenMeteo
{
    public static void Processar(List<OpenMateoDTO> dados, string outputFolder)
    {
        int fileCounter = 1;

        foreach (var modelo in dados)
        {
            if (modelo.hourly is not { time: not null, precipitation: not null }) continue;

            string baseFileName = $"openMeteo_{fileCounter}";
            string caminhoGeral = Path.Combine(outputFolder, $"{baseFileName}_{modelo.latitude}_{modelo.longitude}.txt");
            string caminhoTotais = Path.Combine(outputFolder, $"{baseFileName}_{modelo.latitude}_{modelo.longitude}_totais.txt");

            using (var writerGeral = new StreamWriter(caminhoGeral))
            using (var writerTotais = new StreamWriter(caminhoTotais))
            {
                writerGeral.WriteLine("Data_Hora; Precipitacao;");
                writerTotais.WriteLine("Data; Soma_Precipitacao;");
                
                var dadosPorDia = modelo.hourly.time
                    .Zip(modelo.hourly.precipitation, (time, precipitation) => new { time, precipitation })
                    .GroupBy(entry => entry.time.Date);

                foreach (var grupo in dadosPorDia)
                {
                    double somaPrecipitation = 0;

                    foreach (var item in grupo)
                    {
                        writerGeral.WriteLine(
                            $"{item.time:dd/MM/yyyy HH:mm:ss}; {item.precipitation:F2}"
                                .Replace(',', '.'));

                        somaPrecipitation += item.precipitation;
                    }

                    writerTotais.WriteLine(
                        $"{grupo.Key:dd/MM/yyyy}; {somaPrecipitation:F2}"
                                .Replace(',', '.'));
                }
            }

            Console.WriteLine($"Arquivo geral gerado: {caminhoGeral}");
            Console.WriteLine($"Arquivo de totais gerado: {caminhoTotais}");

            fileCounter++;
        }
    }
}