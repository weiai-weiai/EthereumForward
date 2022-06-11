using EthereumForward.JSON;
using EthereumForward.SSL;
using EthereumForward.TCP;
using EthereumForward.Utils;

using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
ConfigEntity configData = null;
try
{
    configData = JsonConvert.DeserializeObject<ConfigEntity>(ReadTxt.ReadTXT(System.IO.Directory.GetCurrentDirectory() + "\\Config.json"));
    foreach (var item in configData.Forward)
    {
        if (item.ServerAgreement.Equals("SSL"))
        {
            SslSocketServer sslServer = new SslSocketServer();
            sslServer.Init(item);
        }
        else 
        {
            SocketServer tcpServer = new SocketServer();
            tcpServer.Init(item);
        }
    }
}
catch (Exception ex) 
{
    Console.WriteLine("开启服务出现错误："+ex.ToString());
    return;
}
//Web最后开启
app.Run("http://*:" + configData.WebPort);
