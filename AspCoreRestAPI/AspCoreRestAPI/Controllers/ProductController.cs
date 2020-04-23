using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AspCoreRestAPI.Dtos;
using AspCoreRestAPI.Extentions;
using AspCoreRestAPI.Fillters;
using AspCoreRestAPI.Models;
using AspCoreRestAPI.Resources;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AspCoreRestAPI.Controllers
{
    [Route("api/{culture}[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionstring;
        private readonly ILogger<ProductController> _logger;
        private readonly IStringLocalizer<ProductController> _localizer;
        private readonly LocalService _localService;

        public ProductController(IConfiguration configuration, ILogger<ProductController> logger, IStringLocalizer<ProductController> localizer, LocalService localService)
        {
            _connectionstring = configuration.GetConnectionString("DbConnectionString");
            _logger = logger;
            _localizer = localizer;
            _localService = localService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            //_logger.LogError("tran hien loi roi");

            //test 
            var culture = CultureInfo.CurrentCulture.Name;
            string text = _localizer["test"];
            string textforgotpassword = _localService.GetLocalizedHtmlString("ForgotPassword");
            using (var con = new SqlConnection(_connectionstring))
            {
                if(con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                var result = await 
                    con.QueryAsync<Product>(
                        "GetAllData",
                        null, null, null, System.Data.CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> GetById(int id)
        {
            using (var con = new SqlConnection(_connectionstring))
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                var parammeters = new DynamicParameters();
                parammeters.Add("@id", id);
                var result = await con.QueryAsync<Product>("Get_Product_By_Id", parammeters, null, null,
                    System.Data.CommandType.StoredProcedure);

                return result.Single();
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post( [FromBody] Product product)
        {
            var newid = 0;
            try
            {
                using (var con = new SqlConnection(_connectionstring))
                {
                    if(con.State == ConnectionState.Closed)
                        con.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@sku", product.Sku);
                    parameters.Add("@price", float.Parse(product.Price.ToString()));
                    parameters.Add("@discountprice", float.Parse(product.DiscountPrice.ToString()));
                    parameters.Add("@imageurl", product.ImageUrl);
                    parameters.Add("@viewcount", product.ViewCount);
                    parameters.Add("@imagelist", product.ImageList);
                    parameters.Add("@isactive", product.IsActive);
                    parameters.Add("@id", dbType:DbType.Int32, direction:ParameterDirection.Output);
                    await con.ExecuteAsync("Create_Product", parameters, null, null,
                        CommandType.StoredProcedure);

                    newid = parameters.Get<int>("@id");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Ok(newid);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            using (var con = new SqlConnection(_connectionstring))
            {
                if(con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@discountprice", product.DiscountPrice);
                parameters.Add("@imageurl", product.ImageUrl);
                parameters.Add("@viewcount", product.ViewCount);
                parameters.Add("@imagelist", product.ImageList);
                parameters.Add("@isactive", product.IsActive);
                await con.ExecuteAsync("Update_Product", parameters, null, null,
                    System.Data.CommandType.StoredProcedure);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var con = new SqlConnection(_connectionstring))
            {
                if(con.State == System.Data.ConnectionState.Closed)
                    con.Open();

                var paramesters = new DynamicParameters();
                paramesters.Add("@id", id);
                await con.ExecuteAsync("Delete_Product", paramesters, null, null,
                    System.Data.CommandType.StoredProcedure);
            }
        }

        [HttpGet("paging", Name = "GetPagind")]
        public async Task<PagedResult<Product>> GetPaging(string keyword, int categoryId, int pageIndex, int pageSize)
        {
            using (var con = new SqlConnection(_connectionstring))
            {
                if(con.State == System.Data.ConnectionState.Closed) {con.Open(); }
                var paramester = new DynamicParameters();
                paramester.Add("@keyword", keyword);
                paramester.Add("@categoryId", categoryId);
                paramester.Add("@pageIndex", pageIndex);
                paramester.Add("@pageSize", pageSize);
                paramester.Add("@totalRow", dbType:System.Data.DbType.Int32, direction:System.Data.ParameterDirection.Output);

                var resutl = await con.QueryAsync<Product>("Get_Product_Paging", paramester, null, null,
                    System.Data.CommandType.StoredProcedure);
                int totalRow = paramester.Get<int>("@totalRow");

                var pageResult = new PagedResult<Product>()
                {
                    ItemList = resutl.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRow = totalRow
                };
                return pageResult;
            }
        }
    }
}