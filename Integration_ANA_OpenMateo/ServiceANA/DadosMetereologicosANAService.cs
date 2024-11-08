using System.Globalization;
using System.Text;
using System.Xml;

namespace Integration_ANA_OpenMateo.ServiceANA;

public class DadosMetereologicosANAService
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceUrl = "https://telemetriaws1.ana.gov.br/ServiceANA.asmx?op=DadosHidrometeorologicos";
    
    public DadosMetereologicosANAService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<DadosMetereologicosANADTO>> ObterDadosAsync(string codEstacao, DateTime dataInicio, DateTime dataFim)
    {
        try
        {
            var soapRequest = GerarSoapRequest(codEstacao, dataInicio, dataFim);
            var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");

            var response = await _httpClient.PostAsync(_serviceUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return ProcessarResposta(responseString);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter dados metereológicos da ANA", ex);
        }
    }

    protected virtual string GerarSoapRequest(string codEstacao, DateTime dataInicio, DateTime dataFim)
    {
        return $"""
                    <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:mrcs="http://MRCS/">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <mrcs:DadosHidrometeorologicos>
                             <mrcs:codEstacao>{codEstacao}</mrcs:codEstacao>
                             <mrcs:dataInicio>{dataInicio:dd/MM/yyyy}</mrcs:dataInicio>
                             <mrcs:dataFim>{dataFim:dd/MM/yyyy}</mrcs:dataFim>
                          </mrcs:DadosHidrometeorologicos>
                       </soapenv:Body>
                    </soapenv:Envelope>
                """;
    }
    
    private static List<DadosMetereologicosANADTO> ProcessarResposta(string xmlResponse)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlResponse);

        var nodes = xmlDoc.GetElementsByTagName("DadosHidrometereologicos");
        var culturaBrasileira = new CultureInfo("pt-BR");

        var dados = (from XmlNode node in nodes
            select new
            {
                CodEstacao = node["CodEstacao"]?.InnerText,

                DataHora = DateTime.TryParse(
                    node["DataHora"]?.InnerText ?? "1900-01-01T00:00:00Z", 
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out DateTime dataHoraConvertida
                )
                    ? dataHoraConvertida
                    : new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),

                Vazao = decimal.TryParse(node["Vazao"]?.InnerText, NumberStyles.Any, culturaBrasileira, out decimal vazaoConvertida) 
                    ? vazaoConvertida 
                    : 0,

                Nivel = decimal.TryParse(node["Nivel"]?.InnerText, NumberStyles.Any, culturaBrasileira, out decimal nivelConvertido) 
                    ? nivelConvertido 
                    : 0,

                Chuva = decimal.TryParse(node["Chuva"]?.InnerText, NumberStyles.Any, culturaBrasileira, out decimal chuvaConvertida) 
                    ? chuvaConvertida 
                    : 0
            }).ToList();

        var dadosOrdenados = dados.OrderBy(d => d.DataHora).ToList();

        return dadosOrdenados.Select(d => new DadosMetereologicosANADTO
        {
            CodEstacao = d.CodEstacao,
            DataHora = d.DataHora.ToUniversalTime().ToString(),
            Vazao = d.Vazao,
            Nivel = d.Nivel,
            Chuva = d.Chuva
        }).ToList();
    }
}