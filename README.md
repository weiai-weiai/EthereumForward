# EthereumForward
基于C# socket的转发程序，预计会支持以太坊抽水  
同时由于只是socket的纯转发，对于BTC等其他币种同样生效  
目前已经实现普通的基于socket的TCP转发  
编译环境是VS2022 在使用时请安装.net 6.0+
由于配置配置界面/抽水还没有完成，所以请手动编辑config

## Liunx一键安装脚本
```bash
bash <(curl -s -L https://cdn.jsdelivr.net/gh/weiai-weiai/EthereumForward@master/install.sh)
```
### 后台运行（注意后面的&）运行完再敲几下回车

```bash
cd /root/EthereumForward
nohup dotnet EthereumForward.dll &
```

### 后台运行时关闭

```bash
killall dotnet EthereumForward.dll
```
