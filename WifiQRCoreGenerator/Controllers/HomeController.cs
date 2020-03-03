using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using WifiQRCoreGenerator.Models;
using static QRCoder.PayloadGenerator;

namespace WifiQRCoreGenerator.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : Controller
    {
        #region Constants

        private const int INT_PIXELS_PER_MODULE = 4;
        private readonly ILogger<HomeController> _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="logger">Default logger.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        } 

        #endregion

        #region Post Methods

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

        [HttpPost]
        public IActionResult GeneratePhoneNumber(string phone)
        {
            return File(QrCodeToByteArray(new PhoneNumber(phone).ToString()), "image/jpeg");
        }

        /// <summary>
        /// Generates contact data payload in different formats.
        /// </summary>
        /// <param name="firstname">The firstname</param>
        /// <param name="lastname">The lastname</param>
        /// <param name="nickname">The displayname</param>
        /// <param name="phone">Normal phone number</param>
        /// <param name="mobilePhone">Mobile phone</param>
        /// <param name="workPhone">Office phone number</param>
        /// <param name="email">E-Mail address</param>
        /// <param name="birthday">Date of birth</param>
        /// <param name="website">Url of website</param>
        /// <param name="street">Name of the street</param>
        /// <param name="houseNumber">House/Apt. number</param>
        /// <param name="city">Name of the City or Town</param>
        /// <param name="zipCode">Zip or Postal code</param>
        /// <param name="country">Name of country</param>
        /// <param name="note">Short note for this contact</param>
        /// <param name="stateRegion">State \ Region</param>
        /// <returns>Returns a QR code that generates a contact data card.</returns>
        [HttpPost]
        public IActionResult GenerateContactData(
            string firstname,
            string lastname,
            string nickname,
            string phone,
            string mobilePhone,
            string workPhone,
            string email,
            DateTime? birthday,
            string website,
            string street,
            string houseNumber,
            string city,
            string zipCode,
            string country,
            string note,
            string stateRegion
            )
        {
            return File(QrCodeToByteArray(
                new ContactData(ContactData.ContactOutputType.VCard3,
                                firstname,
                                lastname,
                                nickname,
                                phone,
                                mobilePhone,
                                workPhone,
                                email,
                                birthday,
                                website,
                                street,
                                houseNumber,
                                city,
                                zipCode,
                                country,
                                note,
                                stateRegion).ToString()), "image/jpeg");
        }

        /// <summary>
        /// Composes an empty/new mail.
        /// </summary>
        /// <param name="mailReceiver">Receiver's email address</param>
        /// <param name="subject">Subject line of the email</param>
        /// <param name="message">Message content of the email</param>
        /// <returns>Returns generate QR code as image</returns>
        [HttpPost]
        public IActionResult GenerateMail(string mailReceiver, string subject, string message)
        {
            return File(QrCodeToByteArray(
                new Mail(mailReceiver,
                         subject,
                         message).ToString()), "image/jpeg");
        } 

        #endregion
        
        #region Error Handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #region Private Methods

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

        #endregion
    }
}
