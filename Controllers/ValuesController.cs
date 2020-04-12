using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PostmanAssignment.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}