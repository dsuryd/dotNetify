using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;

[assembly: InternalsVisibleTo("DotNetify.Observer.UnitTests")]
[assembly: InternalsVisibleTo("DotNetify.LicenseKeyGenerator")]

namespace DotNetify.Observer
{
   internal class LicenseException : Exception
   {
      public LicenseException(string message) : base(message)
      {
      }
   }

   internal static class License
   {
      private static readonly string LICENSE_ERROR_INVALID_KEY = "invalid license key";
      private static readonly string LICENSE_ERROR_BAD_FILE = "could not open license file";

      private static readonly string LICENSE_ERROR_NOT_FOUND = "license file not found";
      private static readonly string LICENSE_ERROR_EXPIRED = "license has expired";
      private static readonly string LICENSE_ERROR_FEATURE_NOT_INCLUDED = "license doesn't include this feature";

      private static readonly object _sync = new object();
      private static KeyInfo _licenseKeyInfo;

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

      internal static void Check()
      {
         lock (_sync)
         {
            var keyInfo = ReadLicense();

            if (keyInfo == null)
               throw new LicenseException(LICENSE_ERROR_NOT_FOUND);
            else if (DateTime.Now > keyInfo?.Expires)
               throw new LicenseException(LICENSE_ERROR_EXPIRED);
            else if (keyInfo.Level != "pro" && keyInfo.Level != "team" && keyInfo.Level != "enterprise")
               throw new LicenseException(LICENSE_ERROR_FEATURE_NOT_INCLUDED);
         }
      }

      internal static void CheckAtLeastTeam()
      {
         lock (_sync)
         {
            var keyInfo = ReadLicense();

            if (keyInfo == null)
               throw new LicenseException(LICENSE_ERROR_NOT_FOUND);
            else if (DateTime.Now > keyInfo?.Expires)
               throw new LicenseException(LICENSE_ERROR_EXPIRED);
            else if (keyInfo.Level != "team" && keyInfo.Level != "enterprise")
               throw new LicenseException(LICENSE_ERROR_FEATURE_NOT_INCLUDED);
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
            return null;
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