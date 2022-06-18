using EthereumForward.Entity.JSON;
using EthereumForward.Utils.Algorithm.Ethash;

namespace EthereumForward.Utils
{
    public class Classification
    {
        EthAnalysis Eth = null;
        public string DataAnalysis(string data, ForwardItem configEntity)
        {
            if (configEntity.Currency.ToUpper()== "ETH") 
            {
                if (Eth == null) 
                {
                    Eth = new EthAnalysis();
                }
                return Eth.Processing(data, configEntity);
            }
            return data;
        }
    }
}
