using BackEcommerceAngNet.DataAccess;
using BackEcommerceAngNet.Models;
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
        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = dataAccess.GetProduct(id);
            return Ok(result);
        }
        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody]User user)
        {
            user.CreatedAt=DateTime.Now.ToString(formatodate);
            user.ModifiedAt=DateTime.Now.ToString(formatodate);
            var result=dataAccess.InsertarUsuario(user);
            string? message;
            if (result)
            {
                message = "Insertado correctamente";
            }
            else
            {
                message = "No se pudo insertar";
            }
            return Ok(message);
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            string token = dataAccess.UserExist(user.Email, user.Password);
            if (token == "")
            {
                token = "invalid";
            }
            return Ok(token);
        }

        [HttpPost("InsertReview")]
        public IActionResult InsertReview([FromBody] Review review)
        {
            review.cretedAt = DateTime.Now.ToString(formatodate);
            dataAccess.InsertReview(review);
            
            return Ok("review insertado");
        }

        [HttpGet("GetReviews/{productid}")]
        public IActionResult GetReviews(int productid)
        {
            var result=dataAccess.GetReviews(productid);
            return Ok(result);
        }

        [HttpPost("InsertCartItem/{useid}/{productid}")]
        //aqui no debe ir el [FromBody] porque son 2 parametros y al useid lo convierte en string
        public IActionResult InsertCartItem(int useid, int productid)
        {
            var result=dataAccess.InsertItemCart(useid, productid);
            
            return Ok(result ? "insertado" : "no insertado");
        }

        [HttpGet("GetCartActivoPorUser/{userid}")]
        public IActionResult InsertCartItem(int userid)
        {
            var result = dataAccess.GetCartActivePorUser(userid);

            return Ok(result);
        }
    }
}
