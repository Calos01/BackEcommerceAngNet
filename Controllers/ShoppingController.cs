using BackEcommerceAngNet.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace BackEcommerceAngNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : Controller
    {
        private readonly IDataAccess dataAccess;
        private readonly string formatodate;
        public ShoppingController(IDataAccess dataAccess, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            formatodate = configuration["Constants: FormatoDate"];
        }

        [HttpGet("GetListCategories")]
        public IActionResult GetListCategories()
        {
            var result=dataAccess.GetProductCategories();
            return Ok(result); 
        } 
        [HttpGet("GetOffer")]
        public IActionResult GetOffer(int id)
        {
            var result=dataAccess.GetOffer(id);
            return Ok(result);
        }
        [HttpGet("GetProdCategory")]
        public IActionResult GetProdCategory(int id)
        {
            var result = dataAccess.GetProductCategory(id);
            return Ok(result);
        }
        [HttpGet("GetProductos")]
        public IActionResult GetProductos(string category, string subcategory, int count)
        {
            var result = dataAccess.GetProductos(category, subcategory, count);
            return Ok(result);
        }
    }
}
