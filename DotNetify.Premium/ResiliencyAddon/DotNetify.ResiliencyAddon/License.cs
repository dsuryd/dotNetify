using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;

[assembly: InternalsVisibleTo("DotNetify.ResiliencyAddon.UnitTests")]

namespace DotNetify.WebApi
{
   internal class LicenseException : Exception
   {
      public LicenseException(string message) : base(message)
      {
      }
   }

   internal static class License
   {
      private static readonly string LICENSE_ERROR_MAX_REACHED = "usage limit of {0} reached";
      private static readonly string LICENSE_ERROR_INVALID_KEY = "invalid license key";
      private static readonly string LICENSE_ERROR_BAD_FILE = "could not open license file";
      private static readonly string LICENSE_ERROR_ONLY_INSTANCE = "license allows only one instance";

      private static readonly string LICENSE_ERROR_NOT_FOUND = "license file not found";
      private static readonly string LICENSE_ERROR_EXPIRED = "license has expired";

      private static readonly object _sync = new object();
      private static Mutex _onlyInstanceMutex;
      private static KeyInfo _licenseKeyInfo;
      private static uint usedLicenses = 0;
      private static readonly KeyInfo _trialKeyInfo = new KeyInfo { UserId = "TRIAL", Level = "trial", Usage = 10, AssemblyName = "*", Expires = DateTime.Now.AddDays(1) };

      private static readonly string LICENSE_FILE = "/dotnetify.lic";
      private static readonly string PUBLIC_KEY = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCs/bd6iZxccDnrBTRQW+MNAlOfpoyE1Wy6mBpolzBQRGFig0y+RV10dj3MeriANE7nuHqwb+wvp0tYw3rK9lDq3AkOMgx7TdrDCQN45pGm7l+asbqCeYHYBJvEq6Fpg97kuBPA4RxoPk3gQbWU9OOTOwGTrK89KAdMBzEWvW2BSQIDAQAB";

      [Serializable]
      internal class KeyInfo
      {
         [JsonPropertyName("id")]
         public string UserId { get; set; }

         [JsonPropertyName("feat")]
         public string Features { get; set; }

         [JsonPropertyName("use")]
         public uint Usage { get; set; }

         [JsonPropertyName("exp")]
         public DateTime Expires { get; set; }

         [JsonPropertyName("asn")]
         public string AssemblyName { get; set; }

         [JsonPropertyName("hs")]
         public byte[] Hash { get; set; }

         [JsonPropertyName("lvl")]
         public string Level { get; set; }
      }

      internal static KeyInfo CheckUsage()
      {
         lock (_sync)
         {
            usedLicenses++;
            if (usedLicenses <= 1)
               return null;

            var keyInfo = ReadLicense();

            if (keyInfo.Level == "pro")
               keyInfo.Usage = 100;
            else if (keyInfo.Level == "team")
               keyInfo.Usage = uint.MaxValue;
            else if (keyInfo.Level == "enterprise")
               keyInfo.Usage = uint.MaxValue;
            else
               keyInfo.Usage = 10;

            if (DateTime.Now > keyInfo.Expires)
               throw new LicenseException(LICENSE_ERROR_EXPIRED);
            else if (usedLicenses > keyInfo.Usage)
               throw new LicenseException(string.Format(LICENSE_ERROR_MAX_REACHED, keyInfo.Usage));

            return keyInfo;
         }
      }

      internal static void CheckOnlyInstance()
      {
         if (_onlyInstanceMutex != null)
         {
            _onlyInstanceMutex = new Mutex(true, "DotNetify.LoadTestRunner", out bool isMutexCreated);
            if (isMutexCreated == false)
               throw new LicenseException(LICENSE_ERROR_ONLY_INSTANCE);
         }
      }

      public static KeyInfo ReadLicense()
      {
         if (_licenseKeyInfo != null)
            return _licenseKeyInfo;

         KeyInfo licenseKeyInfo;

         var path = Directory.GetCurrentDirectory();
         while (path != null && !File.Exists(path + LICENSE_FILE))
            path = Directory.GetParent(path)?.ToString();

         if (path == null)
         {
            System.Diagnostics.Trace.WriteLine(LICENSE_ERROR_NOT_FOUND);
            return _trialKeyInfo;
         }

         try
         {
            string licenseText = File.ReadAllText(path + LICENSE_FILE).Trim();
            string licenseKey = Encoding.UTF8.GetString(Convert.FromBase64String(licenseText));
            licenseKeyInfo = JsonSerializer.Deserialize<KeyInfo>(licenseKey);
         }
         catch (IOException)
         {
            throw new LicenseException(LICENSE_ERROR_BAD_FILE);
         }
         catch
         {
            throw new LicenseException(LICENSE_ERROR_INVALID_KEY);
         }

         var rsa = new RSACryptoServiceProvider();
         rsa.ImportParameters(PublicPemToRsa(PUBLIC_KEY));

         var hash = licenseKeyInfo.Hash;
         licenseKeyInfo.Hash = null;
         if (rsa.VerifyData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(licenseKeyInfo)), new SHA512Managed(), hash))
         {
            _licenseKeyInfo = licenseKeyInfo;
            return licenseKeyInfo;
         }

         throw new LicenseException(LICENSE_ERROR_INVALID_KEY);
      }

      #region Cryptography

      internal static RSAParameters PublicPemToRsa(string publicKey)
      {
         try

         {
            using (TextReader reader = new StringReader($"-----BEGIN PUBLIC KEY-----\r\n{publicKey}\r\n-----END PUBLIC KEY-----"))
            {
               PemReader pemReader = new PemReader(reader);
               RsaKeyParameters param = (RsaKeyParameters) pemReader.ReadObject();
               return ToRSAParameters(param);
            }
         }
         catch (Exception ex)
         {
            throw new CryptographicException("Failed to convert PEM key to RSA", ex);
         }
      }

      internal static RSAParameters ToRSAParameters(RsaKeyParameters rsaKey)
      {
         var rsaParameters = new RSAParameters
         {
            Modulus = rsaKey.Modulus.ToByteArrayUnsigned()
         };

         if (rsaKey.IsPrivate)
            rsaParameters.D = ConvertRSAParametersField(rsaKey.Exponent, rsaParameters.Modulus.Length);
         else
            rsaParameters.Exponent = rsaKey.Exponent.ToByteArrayUnsigned();
         return rsaParameters;
      }

      private static byte[] ConvertRSAParametersField(BigInteger n, int size)
      {
         byte[] byteArrayUnsigned = n.ToByteArrayUnsigned();
         if (byteArrayUnsigned.Length == size)
            return byteArrayUnsigned;

         if (byteArrayUnsigned.Length > size)
            throw new ArgumentException("Specified size too small", nameof(size));

         byte[] padded = new byte[size];
         Array.Copy(byteArrayUnsigned, 0, padded, size - byteArrayUnsigned.Length, byteArrayUnsigned.Length);

         return padded;
      }

      #endregion Cryptography
   }
}