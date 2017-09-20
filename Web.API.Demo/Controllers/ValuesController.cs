using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.API.Demo.Controllers
{
    public class ComplexClass
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DataTable Data { get; set; }
    }

    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [Route("value/post")]
        public ComplexClass Post1([FromBody]ComplexClass entiy)
        {
            return new ComplexClass();
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
