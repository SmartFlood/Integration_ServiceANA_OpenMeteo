using Integration_ANA_OpenMateo.OpenMeteo;
using Integration_ANA_OpenMateo.ServiceANA;

Console.WriteLine("Iniciando a aplicação");

#region ServiceANA
Console.WriteLine("Processando o Service ANA");

const string codEstacao = "87399000";
var dataInicio = Convert.ToDateTime("06/11/2024");
var dataFim = Convert.ToDateTime("07/11/2024");
const string outputFileANA = "/Users/jhordan/Downloads";

var serviceANA = new DadosMetereologicosANAService(new HttpClient());
var dadosANA = await serviceANA.ObterDadosAsync(codEstacao, dataInicio, dataFim);

Console.WriteLine("Processando o arquivo de dados do ANA");

ProcessamentoANA.Processar(dadosANA, outputFileANA, codEstacao);

Console.WriteLine("Finalizando o Service ANA");

#endregion

#region Open-Meteo

Console.WriteLine("Processando o Open-Meteo");

string[] latitudes = { "-23.5505", "40.7128" };
string[] longitudes = { "-46.6333", "-74.0060" };
const string outputFileOpenMeteo = "/Users/jhordan/Downloads";

var dadosOpenMeteo = await OpenMeteoService.GetOpenMateoAsync(latitudes, longitudes);

Console.WriteLine("Processando o arquivo de dados do Open-Meteo");

ProcessamentoOpenMeteo.Processar(dadosOpenMeteo, outputFileOpenMeteo);

Console.WriteLine("Finalizando o Open-Meteo");

#endregion

Console.WriteLine("Encerrando a aplicação");