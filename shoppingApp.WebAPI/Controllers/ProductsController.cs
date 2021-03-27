using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using shoppingApp.Business.Abstract;
using shoppingApp.Entity;
using shoppingApp.WebAPI.DTO;


namespace shoppingApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // localhost/api/products/2
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {   /*
            var products = await _productService.GetAll();  

            var productsDTO = new List<ProductDTO>();

            foreach (var p in products)
            {
                productsDTO.Add(ProductToDTO(p));
            }

            return Ok(productsDTO);

            */
            var res = await _productService.GetAll();
            if (res.Success)
            {
                return Ok(res.Data);
            }
            return BadRequest(res.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var p = await _productService.GetById(id);
            if(p==null)
            {
                return NotFound(); // 404
            }
            return Ok(ProductToDTO(p)); // 200
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            await _productService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId},ProductToDTO(entity));
        }

        // localhost:5000/api/products/2
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if (id!=entity.ProductId)
            {
                return BadRequest();
            }

            var product = await _productService.GetById(id);

            if(product==null)
            {
                return NotFound();
            }

            await _productService.UpdateAsync(product,entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetById(id);

            if(product==null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(product);
            return NoContent();
        }

        private static ProductDTO ProductToDTO(Product p)
        {
            return new ProductDTO{
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Url = p.Url,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                };
        }
    }
}