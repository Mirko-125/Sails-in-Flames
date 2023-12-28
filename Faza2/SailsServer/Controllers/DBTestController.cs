using Microsoft.AspNetCore.Mvc;
using SailsServer.Database;

namespace SailsServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DBTestController : ControllerBase
    {
        private readonly IIgraciProvider igraciProvider;
        private readonly IIgraciRepository igraciRepository;

        public DBTestController(IIgraciProvider productProvider,
            IIgraciRepository productRepository)
        {
            this.igraciProvider = productProvider;
            this.igraciRepository = productRepository;
        }

        [HttpPost]
        public async Task Post([FromBody] Igraci igrac)
        {
            await igraciRepository.Create(igrac);
        }

        [HttpGet]
        public async Task<IEnumerable<Igraci>> Get(string UserID)
        {
            return await igraciProvider.Get(UserID);
        }
    }
}