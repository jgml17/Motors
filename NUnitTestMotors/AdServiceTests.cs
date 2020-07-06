using NUnit.Framework;
using RestApiClientAdsServices.Api;
using System;
using System.Threading.Tasks;

namespace NUnitTestMotors
{
    public class AdServiceTests
    {
        private IAdsApi _adsApi;
        //private const string DEBUGBASEPATH = "http://172.16.199.128:5500";
        private const string DEBUGBASEPATH = "http://localhost:5500";

        [SetUp]
        public void Setup()
        {
            _adsApi = new AdsApi(DEBUGBASEPATH);
        }

        /// <summary>
        /// Consultar todos anuncios
        /// </summary>
        [Test]
        public async Task GetAdsAll()
        {
            try
            {
                var ret = await _adsApi.ApiAdsGetAsync();

                if (ret?.Count >= 0)
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail("ERROR: ", "objeto nulo");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("ERROR: ", ex.Message);
            }
        }
    }
}