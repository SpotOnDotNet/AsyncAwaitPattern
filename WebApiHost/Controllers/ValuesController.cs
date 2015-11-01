using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CurrentExamples;

namespace WebApiHost.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public async Task<IEnumerable<string>> Get()
        {
            var dotNet45Task = new DotNet45TaskExamples();

            string result = await dotNet45Task.ASyncCallWithContinuation().ConfigureAwait(false);

            var httpContext = HttpContext.Current;

            return new string[] { "value1", "value2" };
        }
    }
}