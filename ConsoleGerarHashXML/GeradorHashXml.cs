using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace ConsoleGerarHashXML
{
    class GeradorHashXml
    {

        /// <summary>
        /// Recebe um objeto de referencia a um arquivo XML realiza a leitura, gera o HASH e retorna.
        /// </summary>
        /// <param name="arquivoXml">Arquivo XML</param>
        /// <param name="nomeTagXml">Nome da Tag do XML que será lida para gerar o HASH, tudo que estiver dentro dela será utilizado para gerar o HASH.</param>
        /// <param name="schemaXml">(Opciona, default = "") Alguns Hash são gerados com o schema do XML, como o do SIB/ANS.</param>
        /// <param name="hasTagHashNoXml">(Opcional, default = "true") Flag para informar se a TagXML HASH está no arquivo XML, pois se estiver temos que retira-lo para gerar o HASH.</param>
        /// <param name="nomeTagXmlHash">(Opcional, default = "HASH") </param>
        /// <returns></returns>
        public string LerXmlGerarHash(FileInfo arquivoXml, string nomeTagXml, string schemaXml = "", bool hasTagHashNoXml = true, string nomeTagXmlHash = "HASH")
        {
            string resultHash;
            try
            {

                StringBuilder conteudoXMLTexo = new StringBuilder();
                if (!string.IsNullOrEmpty(schemaXml))
                    conteudoXMLTexo.Append(schemaXml);

                XDocument xDocument = XDocument.Parse(File.ReadAllText(arquivoXml.FullName));
                string valorXml = xDocument.Element(nomeTagXml).Value;

                string valorAtualHashXml = null;
                if (hasTagHashNoXml)
                {//Recuperar o campo e retirar
                    //Verificar se a Tag do HASH está no conteúdo do XML, pois se estiver temos que retirar para gerar o HASH do conteúdo sem ele. \o/
                    XElement elementHash = LocalizarTagXml(xDocument.Root, nomeTagXmlHash);
                    if (elementHash !=null) { 
                        valorAtualHashXml = elementHash.Value;
                        if (!string.IsNullOrEmpty(valorAtualHashXml))
                        {
                            int tamanhoHash = valorAtualHashXml.Length; //Tamanho do HASH no arquivo.
                            Console.WriteLine("TAG XML [{0}] encontrada >> {1}", nomeTagXmlHash, valorAtualHashXml);
                            valorXml = valorXml.Replace(valorAtualHashXml, "");
                        }
                    }
                }
                
                conteudoXMLTexo.Append(valorXml);
                string valorNovoHashXml = CalcularMD5Hash(conteudoXMLTexo.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                if (hasTagHashNoXml)
                {
                    if (valorAtualHashXml.Equals(valorNovoHashXml))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\\o/ HASH do XML confere com o gerado agora mesmo.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(":( HASH gerado [{0}] não confere com o encontrado no XML.", valorAtualHashXml);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }

                resultHash = valorNovoHashXml;

            }
            catch (Exception erro)
            {
                resultHash = erro.Message;
            }

            return resultHash;
        }

        /// <summary>
        /// Varre os nodes do element enviado a procura da tag informada.
        /// </summary>
        /// <param name="element">Elemento do xml para procura.</param>
        /// <param name="nomeTagXmlHash">Nome da tag.</param>
        /// <returns></returns>
        private XElement LocalizarTagXml(XElement element, string nomeTagXmlHash)
        {
            XElement resultElement = null; 
            foreach (var item in element.Elements())
            {
                if (item.Name.LocalName.ToUpper().Equals(nomeTagXmlHash.ToUpper()))
                    return item;
                if (item.HasElements)
                    resultElement = LocalizarTagXml(item, nomeTagXmlHash);
            }
            return resultElement;
        }

        private string CalcularMD5Hash(string valor)
        {
            return CalcularMD5Hash(valor, Encoding.ASCII);
        }

        private string CalcularMD5Hash(string valor, Encoding encoding)
        {
            using (var md5 = MD5.Create())
            {
                byte[] bytesGerados = encoding.GetBytes(valor);
                byte[] hash = md5.ComputeHash(bytesGerados);
                StringBuilder sbHash = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sbHash.Append(hash[i].ToString("X2"));
                }
                return sbHash.ToString();
            }
        }
    }

}
