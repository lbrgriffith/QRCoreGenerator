using System.Diagnostics;
using System.Drawing;
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
        private const int INT_PIXELS_PER_MODULE = 4;
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
        /// <returns>Returns generate QR code as image.</returns>
        [HttpPost]
        public IActionResult Generate(string ssid, string ssidpassword)
        {
            return File(QrCodeToByteArray(
                new WiFi(ssid, ssidpassword, WiFi.Authentication.WPA).ToString()), 
                "image/jpeg");
        }

        /// <summary>
        /// Generates a link payload.
        /// </summary>
        /// <param name="url">Link url target</param>
        /// <returns>Returns generate QR code as image.</returns>
        [HttpPost]
        public IActionResult GenerateUrl(string url)
        {
            return File(QrCodeToByteArray(new Url(url).ToString()), "image/jpeg");
        }

        /// <summary>
        /// Generates a new WhatsApp message QR Code.
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="number">Recipient/contact number</param>
        /// <returns>Returns generate QR code as image.</returns>
        [HttpPost]
        public IActionResult GenerateWhatsAppMessage(string message, string number)
        {
            return File(QrCodeToByteArray(new WhatsAppMessage(number, message).ToString()), "image/jpeg");
        }

        /// <summary>
        /// Generates a new SMS QR Code for the given contact.
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="encoding">Encoding type</param>
        /// <returns>Returns generate QR code as image</returns>
        [HttpPost]
        public IActionResult GenerateSms(string number, string encoding)
        {
            return File(QrCodeToByteArray(new SMS(number, encoding).ToString()), "image/jpeg");
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

        /// <summary>
        /// Generates QR code based on supplied payload.
        /// </summary>
        /// <param name="payload">String representation of data to be encoded into QR code.</param>
        /// <returns>Returns the converted QR code as a Byte Array</returns>
        private byte[] QrCodeToByteArray(string payload)
        {
            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            var qrCodeAsBitmap = new QRCode(qrGenerator.CreateQrCode(
                                                payload,
                                                QRCodeGenerator.ECCLevel.Q)).GetGraphic(INT_PIXELS_PER_MODULE);

            // Change image into a byte array to return to calling procedure.
            using MemoryStream ms = new MemoryStream();
            qrCodeAsBitmap.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }
    }
}
