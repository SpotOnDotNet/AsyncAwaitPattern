using System.Threading;
using System.Web.Http;

namespace ExternalService.Controllers
{
    public class SlowController : ApiController
    {
        public string Get(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return "ok";
        }
    }
}