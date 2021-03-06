using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReaLocate.Services.Web;

namespace ReaLocate.Services.Web.Tests
{
    [TestClass]
    [PexClass(typeof(IdentifierProvider))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class IdentifierProviderTest
    {

        [PexMethod]
        public string EncodeId([PexAssumeUnderTest]IdentifierProvider target, int id)
        {
            string result = target.EncodeId(id);
            return result;
            // TODO: add assertions to method IdentifierProviderTest.EncodeId(IdentifierProvider, Int32)
        }
    }
}
