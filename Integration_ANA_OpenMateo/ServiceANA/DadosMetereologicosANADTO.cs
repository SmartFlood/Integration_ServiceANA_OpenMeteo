namespace Integration_ANA_OpenMateo.ServiceANA;

public class DadosMetereologicosANADTO
{
    public string CodEstacao { get; set; }
    public string DataHora { get; set; }
    public decimal Vazao { get; set; }
    public decimal Nivel { get; set; }
    public decimal Chuva { get; set; }
}