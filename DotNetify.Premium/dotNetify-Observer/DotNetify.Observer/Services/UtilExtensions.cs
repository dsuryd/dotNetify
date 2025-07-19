using System;
using System.Linq;
using System.Text.Json;
using DotNetify.Forwarding;

namespace DotNetify.Observer
{
   public static class UtilExtensions
   {
      public static string ToLocalIpPortString(this HttpConnection httpConnection) => $"{httpConnection.LocalIpAddressString}:{httpConnection.LocalPort}";

      public static string GetHostName(this ConnectionContext context)
      {
         var headers = context.HttpRequestHeaders?.AllHeaders;
         if (headers?.ContainsKey("Host") == true)
            return headers["Host"].FirstOrDefault();

         return null;
      }

      public static ConnectionContext GetOriginContext(this ConnectionContext context)
      {
         return DotNetifyHubForwarder.GetOriginConnectionContext(context.Items.ToDictionary(x => (object) x.Key, x => x.Value));
      }

      public static string RelativeTo(this DateTimeOffset dateTime, DateTimeOffset currentDateTime)
      {
         if (dateTime > currentDateTime)
            return dateTime.ToString("s");

         TimeSpan timeSpan = currentDateTime - dateTime;
         double totalSeconds = timeSpan.TotalSeconds;

         if (totalSeconds < 1)
            return $"< 1 second ago";

         if (totalSeconds < 60)
            return $"{timeSpan.Seconds} second{(timeSpan.Seconds == 1 ? "" : "s")} ago";

         if (totalSeconds < 3600) // 60 mins * 60 sec
            return $"{timeSpan.Minutes} minute{(timeSpan.Minutes == 1 ? "" : "s")} ago";

         if (totalSeconds < 86400)  // 24 hrs * 60 mins * 60 sec
            return $"{timeSpan.Hours} minute{(timeSpan.Hours == 1 ? "" : "s")} ago";

         return dateTime.ToString("s");
      }

      public static string SerializeToText(this object data)
      {
         if (data == null || string.IsNullOrWhiteSpace(data?.ToString()))
            return string.Empty;

         if (data is string)
            return data.ToString();

         return JsonSerializer.Serialize(data)
            .Replace("\\u0022", "")
            .Replace("\\u002B", "+");
      }
   }
}