using AccessControl.Model.Models;
using RestSharp;

namespace AccessControl.WebApi.Services
{
    public class Worker : BackgroundService
    {
        protected readonly ILogger<Worker> _logger;
        IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _config = config;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    //var _tDeviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                    int time = Int32.Parse(_config["WebSettings:TimeGetLog"]);
                    //try
                    //{
                    //    _logger.LogInformation("Get log");
                    //    string url = _config["AstecSettings:UrlApiDevice"];
                    //    url += "api/aioaccesscontrol/alldevice/synclog";
                    //    RestClient client = new RestClient();
                    //    RestRequest request = new RestRequest(url, Method.POST);
                    //    request.AddHeader("Content-Type", "Application/json");
                    //    IRestResponse response = client.Execute<IRestResponse>(request);
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.LogError("Get log " + DateTime.Now.ToString() + " " + ex.Message);
                    //}


                    //var lstDev = await _tDeviceService.GetAll();
                    //List<Device> dvs = await DeviceMapping.CheckDevicesOnline(lstDev.ToList());
                    //foreach (var device in dvs)
                    //{
                    //    await _tDeviceService.Update(device);
                    //}

                    await Task.Delay(time, stoppingToken);
                }
                catch
                {

                }
            }
        }
    }
    class CheckSync
    {
        public static bool synced;
        public static DateTime dt;
    }
}
