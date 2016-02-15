using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;

namespace UnitTest
{
    public class UnitTestVM : BaseVM
    {
        public string Name
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public int Age
        {
            get { return Get<int>(); }
            set { Set(value); }
        }
    }

    [TestClass]
    public class VMControllerTest
    {
        [TestMethod]
        public void OnRequest()
        {
            var vmController = new VMController();

            //vmController.OnRequestVM("conn1", typeof(UnitTestVM).Name);
        }
    }
}
