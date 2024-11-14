namespace Integration_ANA_OpenMateo.ServiceANA;

public static class ProcessamentoANA
{
    public static void Processar(List<DadosMetereologicosANADTO> dados, string outputFolder, string codEstacao)
    {
        var caminhoArquivoDados = Path.Combine(outputFolder, $"serviceANA_{codEstacao}.txt");
        var caminhoArquivoTotais = Path.Combine(outputFolder, $"serviceANA_totais_{codEstacao}.txt");

        using (var writerDados = new StreamWriter(caminhoArquivoDados, false))
        using (var writerTotais = new StreamWriter(caminhoArquivoTotais, false))
        {
            writerDados.WriteLine("Data_Hora; Vazao; Nivel; Chuva;");
            writerTotais.WriteLine("Data; Chuva_Dia; Vazao_Max; Vazao_Med; Nivel_Max; Nivel_Min;");

            var dadosPorDia = dados.GroupBy(d => DateTime.Parse(d.DataHora).ToString("dd-MM-yyyy"));

            foreach (var grupo in dadosPorDia)
            {
                var dataDoGrupo = grupo.Key;

                decimal maxVazao = decimal.MinValue;
                decimal minNivel = decimal.MaxValue; 
                decimal maxNivel = decimal.MinValue;
                decimal totalChuva = 0;
                decimal somaVazao = 0;
                int countVazao = 0;

                foreach (var dado in grupo)
                {
                    writerDados.WriteLine($"{dado.DataHora}; {dado.Vazao:F2}; {dado.Nivel:F2}; {dado.Chuva:F2}".Replace(',', '.'));

                    if (dado.Vazao > maxVazao) maxVazao = dado.Vazao;
                    if (dado.Nivel > maxNivel) maxNivel = dado.Nivel;
                    if (dado.Nivel < minNivel) minNivel = dado.Nivel;

                    somaVazao += dado.Vazao;
                    totalChuva += dado.Chuva;
                    countVazao++;
                }

                var mediaVazao = countVazao > 0 ? somaVazao / countVazao : 0;
                var dataFormatada = DateTime.ParseExact(dataDoGrupo, "dd-MM-yyyy", null).ToString("dd/MM/yyyy");
                
                writerTotais.WriteLine($"{dataFormatada}; {totalChuva:F2}; {maxVazao:F2}; {mediaVazao:F2}; {maxNivel:F2}; {minNivel:F2}".Replace(',', '.'));
            }
        }

        Console.WriteLine($"Arquivo de dados gerado: {caminhoArquivoDados}");
        Console.WriteLine($"Arquivo de totais gerado: {caminhoArquivoTotais}");
    }
}