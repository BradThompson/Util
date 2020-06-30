using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLinePlay
{
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        public void ThrowBadCodeException()
        {
            try { throw new BadCodeException(); }
            catch (BadCodeException e)
            {
                Assert.AreEqual("Exception of type 'CommandLinePlay.BadCodeException' was thrown.", e.Message);
            }
        }
        [TestMethod]
        public void ThrowBadCodeExceptionWithMessage()
        {
            try { throw new BadCodeException("My Bad"); }
            catch (BadCodeException e)
            {
                Assert.AreEqual("My Bad", e.Message);
            }
        }
        [TestMethod]
        public void ThrowUserInputException()
        {
            try { throw new UserInputException(); }
            catch (UserInputException e)
            {
                Assert.AreEqual("Exception of type 'CommandLinePlay.UserInputException' was thrown.", e.Message);
            }
        }
        [TestMethod]
        public void ThrowUserInputExceptionWithMessage()
        {
            try { throw new UserInputException("Not nice"); }
            catch (UserInputException e)
            {
                Assert.AreEqual("Not nice", e.Message);
            }
        }

    }
}
