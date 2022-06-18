using EthereumForward.Entity.JSON;
using EthereumForward.Entity.Socket.Algorithm.Ethash;

using Newtonsoft.Json;

namespace EthereumForward.Utils.Algorithm.Ethash
{
    public class EthAnalysis
    {
        /// <summary>
        /// 矿机名
        /// </summary>
        public string MinerName { get; set; }
        /// <summary>
        /// 钱包名称
        /// </summary>
        public string WalletName { get; set; }
        /// <summary>
        /// 提交数量
        /// </summary>
        public int SubmitCount { get; set; }
        /// <summary>
        /// 哈希率（算力/m）
        /// </summary>
        public double HashRate { get; set; }
        Random random = new Random();
        public string Processing(string data, ForwardItem configEntity)
        {
            EthPacketEntity packetEntity = JsonConvert.DeserializeObject<EthPacketEntity>(data);
            if (packetEntity != null)
            {
                if (packetEntity.method == "eth_submitWork") 
                {
                    if (configEntity.pumps != null) 
                    {
                        if ((configEntity.pumps.Percent * 100) <= random.Next(0, 10000))
                        {

                        }
                    }

                }
                else if (packetEntity.method == "eth_submitLogin")
                {
                    string[] name = packetEntity.@params[0].Split(".");
                    if (name.Length >= 2)
                    {
                        MinerName = name[1];
                        WalletName = name[0];
                    }
                    else
                    {
                        MinerName = packetEntity.worker;
                        WalletName = name[0];
                    }
                    Console.WriteLine($"获取到钱包：{WalletName} ， 矿机名：{MinerName}");
                }
                else if (packetEntity.method == "eth_submitHashrate")
                {
                    HashRate = (double)(Convert.ToInt32(packetEntity.@params[0], 16)) / 1000 / 1000;
                    Console.WriteLine($"收到提交算力，钱包：{WalletName} 算力：{HashRate}");
                }
                //统一钱包的话直接替换就好
                if (configEntity.IsUnifiedWallet)
                {
                    data = data.Replace(WalletName, configEntity.Wallet);
                }
            }
            else
            {
                return data;
            }
            return data;
        }
    }
}
