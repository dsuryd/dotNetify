using DotNetify;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
               return Task.FromResult((Exception)new ApplicationException());
            return Task.FromResult(exception);
         }
      }

      private class ArgumentNullExceptionMiddleware : IExceptionMiddleware
      {
         public Task<Exception> OnException(HubCallerContext context, Exception exception)
         {
            if (exception is ArgumentNullException)
               return Task.FromResult((Exception)new InvalidOperationException());
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
         VMController.Register<ExceptionOnRequestVM>();
         var hub = new MockDotNetifyHub().Create();

         string exceptionType = null;
         hub.Response += (sender, e) =>
         {
            dynamic exception = JsonConvert.DeserializeObject<dynamic>(e.Item2);
            exceptionType = exception.ExceptionType;
         };

         hub.RequestVM(nameof(ExceptionOnRequestVM), null);
         Assert.AreEqual(nameof(JsonSerializationException), exceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnRequest_ExceptionIntercepted()
      {
         VMController.Register<ExceptionOnRequestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<JsonSerializationExceptionMiddleware>()
            .UseMiddleware<ArgumentNullExceptionMiddleware>()
            .Create();

         string exceptionType = null;
         hub.Response += (sender, e) =>
         {
            dynamic exception = JsonConvert.DeserializeObject<dynamic>(e.Item2);
            exceptionType = exception.ExceptionType;
         };

         hub.RequestVM(nameof(ExceptionOnRequestVM));
         Assert.AreEqual(nameof(ApplicationException), exceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnUpdate_NotIntercepted()
      {
         VMController.Register<ExceptionOnUpdateVM>();
         var hub = new MockDotNetifyHub().Create();

         string exceptionType = null;
         hub.Response += (sender, e) =>
         {
            dynamic exception = JsonConvert.DeserializeObject<dynamic>(e.Item2);
            exceptionType = exception.ExceptionType;
         };

         hub.RequestVM(nameof(ExceptionOnUpdateVM));
         hub.UpdateVM(nameof(ExceptionOnUpdateVM), new Dictionary<string, object> { { "Property", "" } });
         Assert.AreEqual(nameof(ArgumentNullException), exceptionType);
      }

      [TestMethod]
      public void ExceptionMiddleware_ExceptionOnUpdate_ExceptionIntercepted()
      {
         VMController.Register<ExceptionOnUpdateVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<JsonSerializationExceptionMiddleware>()
            .UseMiddleware<ArgumentNullExceptionMiddleware>()
            .Create();

         string exceptionType = null;
         hub.Response += (sender, e) =>
         {
            dynamic exception = JsonConvert.DeserializeObject<dynamic>(e.Item2);
            exceptionType = exception.ExceptionType;
         };

         hub.RequestVM(nameof(ExceptionOnUpdateVM));
         hub.UpdateVM(nameof(ExceptionOnUpdateVM), new Dictionary<string, object> { { "Property", "" } });
         Assert.AreEqual(nameof(InvalidOperationException), exceptionType);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void ExceptionMiddleware_ExceptionOnDispose_ExceptionIntercepted()
      {
         VMController.Register<ExceptionOnDisposeVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExceptionOnDisposeMiddleware>()
            .Create();

         hub.RequestVM(nameof(ExceptionOnDisposeVM));
         hub.DisposeVM(nameof(ExceptionOnDisposeVM));

         throw ExceptionOnDisposeMiddleware.Exception;
      }

      [TestMethod]
      public void ExceptionMiddleware_OperationCancelled_NoResponse()
      {
         VMController.Register<ExceptionOnDisposeVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<OperationCancelledMiddleware>()
            .Create();

         bool responseSent = false;
         hub.Response += (sender, e) => responseSent = true;

         hub.RequestVM(nameof(ExceptionOnDisposeVM));
         Assert.IsFalse(responseSent);
      }
   }
}
