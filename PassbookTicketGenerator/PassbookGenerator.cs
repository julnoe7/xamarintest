using ElectronicTicketGenerator;
using Newtonsoft.Json;
using Passbook.Generator;
using PassbookTicketGenerator.Helpers;
using Security.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PassbookTicketGenerator {

    public class PassbookGenerator : IElectronicTicketGenerator {

        #region consts
        private const string PASS_SIGNATURE = "signature";
        private const string PASS_MANIFEST = "manifest.json";
        private const string PASS_FILE = "pass.json";
        private const string PASS_ICON = "icon.png";
        private const string PASS_ICON_2_X = "icon@2x.png";
        private const string PASS_LOGO = "logo.png";
        private const string PASS_LOGO_2_X = "logo@2x.png";
        private const string PASS_STRIP = "strip.png";
        private const string PASS_STRIP_2_X = "strip@2x.png";
        #endregion

        #region properties
        private readonly SortedDictionary<string, byte[]> files;
        private byte[] certContent;
        private readonly HttpClient httpClient;
        private readonly Func<string> createFilePath;
        private readonly Func<string> createCertFilePath;
        private readonly Func<string> createVasCertFilePath;
        private readonly ICryptography cryptographyService;
        private readonly IDictionary<string, string> passbookConfig;
        private readonly IReadOnlyCollection<string> constantFileNames;
        #endregion

        #region constructor
        public PassbookGenerator() { }

        public PassbookGenerator(
            HttpClient client,
            Func<string> getFilePath,
            Func<string> getCertFilePath,
            Func<string> createVasCertFilePath,
            ICryptography cryptographyService,
            IDictionary<string, string> passConfig) {

            httpClient = client;
            createFilePath = getFilePath;
            createCertFilePath = getCertFilePath;
            this.createVasCertFilePath = createVasCertFilePath;
            this.cryptographyService = cryptographyService;
            passbookConfig = passConfig;

            constantFileNames = new[] {
                PASS_ICON,
                PASS_ICON_2_X,
                PASS_LOGO,
                PASS_LOGO_2_X
            };

            files = new SortedDictionary<string, byte[]>();
        }
        #endregion

        #region nfc testing methods
        public async Task<Uri> GenerateBindingRequest(ITicket ticket) {

            var bindingData = new {
                fidoProfile = new {
                    relyingPartyIdentifier = "https://www.eticket.com.mx",
                    //TODO this is the client identifier hashed, this name is just for testing
                    accountHash = cryptographyService.Encrypt(ticket.ClientName),
                    keyHash = string.Empty,
                    creationTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    sessionIdentifier = Guid.NewGuid(),
                    callbackURL = "https://www.eticket.com.mx/securePassSession",
                    teamIdentifier = passbookConfig["TeamIdentifier"],
                    displayableName = "Test eticket wallet"
                }
            };

            var jsonBindingData = JsonConvert.SerializeObject(bindingData, Formatting.None);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonBindingData);
            var signature = CreateSignatureForJsoncontent(jsonBytes, await GetVasCertAsync());

            //https://wallet.apple.com/securePassSession#issuerData&signature
            return new Uri(
                new Uri("https://wallet.apple.com"),
                $"securePassSession#{Convert.ToBase64String(jsonBytes, Base64FormattingOptions.None)}&{Convert.ToBase64String(signature, Base64FormattingOptions.None)}");
        }
        /// <summary>
        /// creates signing file
        /// if I need to debug on a local machine use  IncludeOption = X509IncludeOption.EndCertOnly
        /// for production use  IncludeOption = X509IncludeOption.WholeChain
        /// </summary>
        /// <param name="jsonBytes"></param>
        /// <param name="vasCertContent"></param>
        /// <returns></returns>
        private byte[] CreateSignatureForJsoncontent(byte[] jsonBytes, byte[] vasCertContent) {

            var cert = new X509Certificate2(
                vasCertContent,
                passbookConfig["passbookPassword"],
                X509KeyStorageFlags.MachineKeySet);

            var contentInfo = new ContentInfo(jsonBytes);

            var signedCms = new SignedCms(contentInfo, true);

            var signer = new CmsSigner(cert) {
                //IncludeOption = X509IncludeOption.WholeChain
                IncludeOption = X509IncludeOption.EndCertOnly
            };

            signedCms.ComputeSignature(signer, false);
            // Encode the PKCS#7 message
            byte[] pkcs7DetachedSignature = signedCms.Encode();
            return pkcs7DetachedSignature;
        }

        private async Task<byte[]> GetVasCertAsync() {

            string path = createVasCertFilePath?.Invoke() ?? string.Empty;
            if (!File.Exists(path)) throw new ArgumentNullException(nameof(path), $"VAS cert file does not exists {path}");

            using var fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            var tempCertContent = new byte[fStream.Length];
            _ = await fStream.ReadAsync(tempCertContent, 0, (int)fStream.Length);

            return tempCertContent;
        }
        /*
        public async Task TestNfcAsync(ITicket ticket) {

            var generator = new PassGenerator();

            var request = new PassGeneratorRequest {
                PassTypeIdentifier = "pass.tomsamcguinness.events",
                TeamIdentifier = "RW121242",
                SerialNumber = "121212",
                Description = "My first pass",
                OrganizationName = "Tomas McGuinness",
                LogoText = "My Pass",
                BackgroundColor = "#FFFFFF",
                LabelColor = "#000000",
                ForegroundColor = "#000000"
            };

            request.BackgroundColor = "rgb(255,255,255)";
            request.LabelColor = "rgb(0,0,0)";
            request.ForegroundColor = "rgb(0,0,0)";

            request.Nfc = new Nfc("THE NFC Message", "<encoded private key>");

            //add these with DI installer
            //also add the generator with DI

            //only instance here the passes, so we can bundle them with the generator

            request.AppleWWDRCACertificate = new X509Certificate2(default(byte[]));
            request.PassbookCertificate = new X509Certificate2();

            byte[] generatedPass = generator.Generate(request);
            //return new FileContentResult(generatedPass, "application/vnd.apple.pkpass");


            //if you want you can add multiple passes and bundle them

            //PassGeneratorRequest myFirstRequest = new PassGeneratorRequest();
            //PassGeneratorRequest mySecondRequest = new PassGeneratorRequest();

            //var requests = new List<PassGeneratorRequest> {
            //    myFirstRequest,
            //    mySecondRequest
            //};

            //byte[] generatedBundle = generator.Generate(requests);

            //return new FileContentResult(generatedBundle, "application/vnd.apple.pkpasses") {
            //    FileDownloadName = "tickets.pkpasses.zip"
            //};
        }
        */
        #endregion

        #region interface implementation
        public async Task<IElectronicTicket> GenerateAsync(ITicket ticket) {

            try {

                await InitializePassbookGeneratorAsync(ticket);
                var pass = ticket.ToPassbook(passbookConfig);
                pass.FileContent = CreatePassbookContents(pass);
                CleanSpecificFiles();
                return pass;

            } finally {
                CleanFiles();
            }
        }

        public async Task<IElectronicTicket> GenerateAsync(IReadOnlyCollection<ITicket> tickets) {

            try {

                await InitializePassbookGeneratorAsync(tickets.FirstOrDefault());
                var passBooks = (from ticket in tickets select ticket.ToPassbook(passbookConfig)).ToArray();

                foreach (Models.Passbook pass in passBooks) {
                    pass.FileContent = CreatePassbookContents(pass);
                    CleanSpecificFiles();
                }

                var result = CreateZipPackage(passBooks);

                return new Models.Passbook {
                    FileName = "Boletos",
                    Extension = "pass",
                    FileContent = result
                };

            } finally {
                CleanFiles();
            }
        }

        public async Task<IEnumerable<IElectronicTicket>> GenerateTicketsAsync(IReadOnlyCollection<ITicket> tickets) {

            try {

                await InitializePassbookGeneratorAsync(tickets.FirstOrDefault());
                var passBooks = (from ticket in tickets select ticket.ToPassbook(passbookConfig)).ToArray();

                foreach (Models.Passbook pass in passBooks) {
                    pass.FileContent = CreatePassbookContents(pass);
                    CleanSpecificFiles();
                }

                return passBooks;

            } finally {
                CleanFiles();
            }
        }
        #endregion

        #region private methods
        private byte[] CreatePassbookContents(in Models.Passbook pass) {
            // Create a pass stream
            using (var mStream = new MemoryStream()) {
                new DataContractJsonSerializer(typeof(Models.Passbook)).WriteObject(mStream, pass);
                files.Add(PASS_FILE, mStream.ToArray());
            }
            //create manifest.json
            byte[] manifestData = CreateManifestFile(files);
            files.Add(PASS_MANIFEST, manifestData);
            //create signature file
            files.Add(
                PASS_SIGNATURE,
                Encoding.Convert(Encoding.Default, Encoding.GetEncoding(1252), CreateSignatureFile(manifestData)));
            //creates the zip file, sets it in the pointer of byte array you passed
            return CreateZipPackage(files);
        }
        /// <summary>
        /// creates signing file
        /// if I need to debug on a local machine use  IncludeOption = X509IncludeOption.EndCertOnly
        /// for production use  IncludeOption = X509IncludeOption.WholeChain
        /// </summary>
        /// <param name="manifestData"></param>
        /// <returns></returns>
        private byte[] CreateSignatureFile(byte[] manifestData) {

            var signer = new CmsSigner(
                SubjectIdentifierType.IssuerAndSerialNumber,
                new X509Certificate2(
                    certContent,
                    passbookConfig["passbookPassword"],
                    X509KeyStorageFlags.MachineKeySet)) {

                IncludeOption = X509IncludeOption.WholeChain
            };

            signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));

            var signedCms = new SignedCms(new ContentInfo(new Oid("1.2.840.113549.1.7.2"), manifestData), true);
            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }

        private async Task InitializePassbookGeneratorAsync(ITicket ticket) {
            await InitializeCertificateAsync();
            await InitializeConstantFilesAsync();
            await InitializeEventImageAsync(ticket);
        }

        private async Task InitializeConstantFilesAsync() {

            string relativePath = createFilePath?.Invoke() ?? string.Empty;

            foreach (var fileName in constantFileNames) {

                string path = Path.Combine(relativePath, fileName);
                if (!File.Exists(path)) throw new ArgumentNullException(nameof(path), $@"image file does not exists {path}");
                if (files.ContainsKey(fileName)) return;

                var buffer = default(byte[]);

                try {
                    using var stream = File.Open(path, FileMode.Open);
                    buffer = new byte[stream.Length];
                    _ = await stream.ReadAsync(buffer, 0, (int)stream.Length);

                } catch {
                    // ignored
                }

                if (buffer is not null) files.Add(fileName, buffer);
            }
        }

        private async Task InitializeCertificateAsync() {

            string path = createCertFilePath?.Invoke() ?? string.Empty;
            if (!File.Exists(path)) throw new ArgumentNullException(nameof(path), $"cert file does not exists {path}");

            using var fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            certContent = new byte[fStream.Length];
            _ = await fStream.ReadAsync(certContent, 0, (int)fStream.Length);
        }

        private async Task InitializeEventImageAsync(ITicket ticket) {

            byte[] imageContent;
            string eventImgUri = ticket.EventImage;
            //sometimes Jonathan's SP have a slash at the end, i.e. 
            //https://cdn.eticket.mx/imagenes/imgEventos/210204172108695_electronic_210118103943241_estelar_micristorotoESTELAR.jpg/
            if (eventImgUri.LastIndexOf("/", StringComparison.Ordinal) == eventImgUri.Length - 1) {
                eventImgUri = eventImgUri.Remove(eventImgUri.LastIndexOf("/", StringComparison.Ordinal));
            }

            try {
                imageContent = await httpClient.GetByteArrayAsync(eventImgUri);
            } catch (Exception) {
                throw new ArgumentNullException($"image could not be downloaded {nameof(eventImgUri)} from {eventImgUri}");
            }

            if (!(imageContent?.Length > 0)) throw new ArgumentNullException($"image could not be downloaded {nameof(eventImgUri)} from {eventImgUri}");

            if (files.ContainsKey(PASS_STRIP)) files.Remove(PASS_STRIP);
            if (files.ContainsKey(PASS_STRIP_2_X)) files.Remove(PASS_STRIP_2_X);

            files.Add(PASS_STRIP, imageContent);
            files.Add(PASS_STRIP_2_X, imageContent);
        }

        private void CleanFiles() {
            files.Clear();
        }

        private void CleanSpecificFiles() {
            files.Remove(PASS_FILE);
            files.Remove(PASS_MANIFEST);
            files.Remove(PASS_SIGNATURE);
        }
        #endregion

        #region static methods
        private static byte[] CreateManifestFile(SortedDictionary<string, byte[]> files) {

            using var manifestStream = new MemoryStream();
            using var manifestStreamWriter = new StreamWriter(manifestStream);
            using var jsWriter = new JsonTextWriter(manifestStreamWriter);
            jsWriter.Formatting = Formatting.None;

            jsWriter.WriteStartObject();

            foreach (var file in files ?? Enumerable.Empty<KeyValuePair<string, byte[]>>()) {
                jsWriter.WritePropertyName(file.Key);
                jsWriter.WriteValue(ComputeHash(file.Value));
            }

            jsWriter.WriteEndObject();
            manifestStreamWriter.Flush();

            return manifestStream.ToArray();
        }

        private static byte[] CreateZipPackage(SortedDictionary<string, byte[]> files) {

            using var mStream = new MemoryStream();
            using (var zipArchiveStream = new ZipArchive(mStream, ZipArchiveMode.Update, true)) {

                foreach (KeyValuePair<string, byte[]> file in files ?? Enumerable.Empty<KeyValuePair<string, byte[]>>()) {

                    ZipArchiveEntry entry = zipArchiveStream.CreateEntry(file.Key, CompressionLevel.NoCompression);
                    using var bWriter = new BinaryWriter(entry.Open());
                    bWriter.Write(file.Value);
                }
            }

            return mStream.ToArray();
        }

        private static byte[] CreateZipPackage(IReadOnlyCollection<IElectronicTicket> tickets) {

            using var mStream = new MemoryStream();
            using (var zipArchiveStream = new ZipArchive(mStream, ZipArchiveMode.Update, true)) {

                foreach (IElectronicTicket file in tickets ?? Enumerable.Empty<IElectronicTicket>()) {

                    ZipArchiveEntry entry = zipArchiveStream.CreateEntry(
                        $"{file.FileName}.{file.Extension}",
                        CompressionLevel.NoCompression);

                    using var bWriter = new BinaryWriter(entry.Open());
                    bWriter.Write(file.FileContent);
                }
            }

            return mStream.ToArray();
        }

        private static string ComputeHash(byte[] content) {
            using var shaProvider = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(shaProvider.ComputeHash(content)).Replace("-", string.Empty).ToLower();
        }
        #endregion
    }
}