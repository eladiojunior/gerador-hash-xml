using System.IO;

namespace ConsoleGerarHashXML
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Console.WriteLine("+--------------------------------------------------------------------------------------------+");
            System.Console.WriteLine("| Ferramenta de Verificação e Geração de Hash de arquivos XML - v1.0 - By Eladio Júnior      |");
            System.Console.WriteLine("+--------------------------------------------------------------------------------------------+");

            GeradorHashXml geradorHashXml = new GeradorHashXml();
            FileInfo fileXml = new FileInfo("exemplo.xml");
            
            //Tag do XML que será usada para geração do Hash
            string tagXml = "mensagemSIB";
            
            //Schema utilizado no XML, alguns HASHs incluem o schema, como no SIB/ANS.
            string schemaXml = "http://www.ans.gov.br/padroes/sib/schemas http://www.ans.gov.br/padroes/sib/schemas/sib.xsd";

            string hash = geradorHashXml.LerXmlGerarHash(fileXml, tagXml, schemaXml);
            System.Console.WriteLine("HASH Gerado [{0}]", hash);

            System.Console.ReadKey();

        }
    }
}
