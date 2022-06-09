namespace EthereumForward.Utils
{
    public class ReadTxt
    {
        public static string ReadTXT(string url) 
        {
            return System.IO.File.ReadAllText(url);
        }
    }
}
