using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Basic.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Produces("application/json")]
    public class TypesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<MyComplexThing> Get() {
            return new MyComplexThing();
        }
    }

    public class MyComplexThing {
        public byte[] Bytes { get; set; }
        public IDictionary<long, MyComplexThing> OtherThings { get; set; }
        public ICollection<MyComplexThing> MyComplexThings { get; set; }
        public Guid Guid { get; set; }
        public Uri Uri { get; set; }
    }
}
