﻿using BackEcommerceAngNet.DataAccess;
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
    }
}