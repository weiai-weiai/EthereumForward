namespace EthereumForward.Entity.Miner.Algorithm.Ethash
{
    public class Eth
    {
        /// <summary>
        /// 矿机名
        /// </summary>
        public string MinerName{ get; set; }
        /// <summary>
        /// 钱包名称
        /// </summary>
        public string WalletName { get; set; }
        /// <summary>
        /// 提交数量
        /// </summary>
        public int SubmitCount { get; set; }
        /// <summary>
        /// 哈希率（算力）
        /// </summary>
        public double HashRate { get; set; }


    }
}
