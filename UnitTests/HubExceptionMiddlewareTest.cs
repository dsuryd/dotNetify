using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
   [TestClass]
   public class HubExceptionMiddlewareTest
   {
      private class ExceptionOnRequestVM : BaseVM
      {
         public string Property => throw new ArgumentNullException();
      }

      private class ExceptionOnUpdateVM : BaseVM
      {
         public string Property
         {
            get => "hello";
            set => throw new ArgumentNullException();
         }
      }

      private class ExceptionOnDisposeVM : BaseVM
      {
         public string Property { get; set; }

         public override void Dispose() => throw new ArgumentOutOfRangeException();
      }

      private class JsonSerializationExceptionMiddleware : IExceptionMiddleware
      {
         public Task<Exception> OnException(HubCallerContext context, Exception exception)
         {
            if (exception is JsonSerializationException)
               return Task.FromResult((Exception) new ApplicationException());
            return Task.FromResult(exception);
         }
      }

      private class ArgumentNullExceptionMiddleware : IExceptionMiddleware
      {
         public Task<Exception> OnException(HubCallerContext context, Exception exception)
         {
            if (exception is ArgumentNullException)
               return Task.FromResult((Exception) new InvalidOperationException());
            return Task.FromResult(exception);
         }
      }

      private class ExceptionOnDisposeMiddleware : IExceptionMiddleware
      {
         public static Exception Exception { get; set; }

         public Task<Exception> OnException(HubCallerContext context, Exception exception)
         {
            Exception = exception;
            return Task.FromResult(exception);
         }
      }

      private class OperationCancelledMiddleware : IMiddleware
      {
         public Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            throw new OperationCanceledException();
         }
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnRequest_NotIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnRequestVM>()
            .Build();

         var client = hubEmulator.CreateClient();
         var response = client.Connect(nameof(ExceptionOnRequestVM)).As<dynamic>();
         Assert.AreEqual(nameof(JsonSerializationException), (string) response.ExceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnRequest_ExceptionIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnRequestVM>()
            .UseMiddleware<JsonSerializationExceptionMiddleware>()
            .UseMiddleware<ArgumentNullExceptionMiddleware>()
            .Build();

         var client = hubEmulator.CreateClient();
         var response = client.Connect(nameof(ExceptionOnRequestVM)).As<dynamic>();
         Assert.AreEqual(nameof(ApplicationException), (string) response.ExceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnUpdate_NotIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnUpdateVM>()
            .Build();

         var client = hubEmulator.CreateClient();
         client.Connect(nameof(ExceptionOnUpdateVM));
         var response = client.Dispatch(new Dictionary<string, object> { { "Property", "" } }).As<dynamic>();
         Assert.AreEqual(nameof(ArgumentNullException), (string) response.ExceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnUpdate_ExceptionIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnUpdateVM>()
            .UseMiddleware<JsonSerializationExceptionMiddleware>()
            .UseMiddleware<ArgumentNullExceptionMiddleware>()
            .Build();

         var client = hubEmulator.CreateClient();
         client.Connect(nameof(ExceptionOnUpdateVM));
         var response = client.Dispatch(new Dictionary<string, object> { { "Property", "" } }).As<dynamic>();

         Assert.AreEqual(nameof(InvalidOperationException), (string) response.ExceptionType);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void ExceptionMiddleware_ExceptionOnDispose_ExceptionIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnDisposeVM>()
            .UseMiddleware<ExceptionOnDisposeMiddleware>()
            .Build();

         var client = hubEmulator.CreateClient();
         client.Connect(nameof(ExceptionOnDisposeVM));
         client.Destroy();

         throw ExceptionOnDisposeMiddleware.Exception;
      }

      [TestMethod]
      public void ExceptionMiddleware_OperationCancelled_NoResponse()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<ExceptionOnDisposeVM>()
            .UseMiddleware<OperationCancelledMiddleware>()
            .Build();

         var client = hubEmulator.CreateClient();
         var request = client.Connect(nameof(ExceptionOnDisposeVM));

         Assert.AreEqual(0, request.Count);
      }
   }
}