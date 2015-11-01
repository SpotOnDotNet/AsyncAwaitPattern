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
            // shows that context won't be available in current method when resuming after awaitable call with ConfigureAwait(false)

            var httpContext = HttpContext.Current; // fix: remove .ConfigureAwait(false) above

            return new string[] { "value1", "value2" };
        }
    }
}