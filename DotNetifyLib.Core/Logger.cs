/*
Copyright 2021 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using Microsoft.Extensions.Logging;

namespace DotNetify
{
   /// <summary>
   /// Global logger.
   /// </summary>
   public static class Logger
   {
      private static ILogger _logger;

      public static void Init(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger(nameof(DotNetify));

      public static void LogInformation(string message, params object[] args) => _logger?.LogInformation(message, args);

      public static void LogDebug(string message, params object[] args) => _logger?.LogDebug(message, args);

      public static void LogError(string message, params object[] args) => _logger?.LogError(message, args);

      public static void LogWarning(string message, params object[] args) => _logger?.LogWarning(message, args);
   }
}