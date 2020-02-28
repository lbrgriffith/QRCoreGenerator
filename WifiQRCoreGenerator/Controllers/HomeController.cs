using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using WifiQRCoreGenerator.Models;
using static QRCoder.PayloadGenerator;

namespace WifiQRCoreGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Use it when you want to share WiFi credentials.
        /// </summary>
        /// <param name="ssid">SSID of the WiFi network</param>
        /// <param name="ssidpassword">Password of the WiFi network</param>
        /// <returns>Returns generate QR code as JPEG image.</returns>
        [HttpPost]
        public IActionResult Generate(string ssid, string ssidpassword)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                                                new WiFi(ssid, ssidpassword, WiFi.Authentication.WPA).ToString(),
                                                QRCodeGenerator.ECCLevel.Q);
            var qrCodeAsBitmap = new QRCode(qrCodeData).GetGraphic(4);

            // Change image into a byte array to return to calling procedure.
            var ms = new MemoryStream();
            qrCodeAsBitmap.Save(ms, ImageFormat.Jpeg);

            return File(ms.ToArray(), "image/jpeg");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
