using MyCalcService.CalcService;
using SoapCore;

namespace MyCalcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSoapCore();
            builder.Services.AddScoped<ISoapService, SoapService>();

            var app = builder.Build();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<ISoapService>("/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
            });

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}