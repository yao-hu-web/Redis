using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NT40Redis1.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using ShopDAL.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace NT40Redis1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IConfiguration Configuration;
        private readonly RedisCache redis;
        public HomeController(ILogger<HomeController> logger,RedisCache redisCache, IConfiguration config)
        {
            _logger = logger;
            redis = redisCache;
            Configuration = config;
        }

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("mysession", $"你好！张三{DateTime.Now}");

            string prodJson = redis.GetString("prods");
            if (string.IsNullOrEmpty(prodJson))
            {
                HttpClient httpClient = new HttpClient();
                prodJson = await httpClient.GetStringAsync(Configuration.GetSection("pathUrl").Value);
                redis.SetString("prods", prodJson);
            }
            IEnumerable<Products> prodList = JsonConvert.DeserializeObject<IEnumerable<Products>>(prodJson);
            return View(prodList);
            ////创建redis客户端对象

            ////写入数据,//设置绝对过期时间
            //redis.SetString("name", "张三1!"+ DateTime.Now,new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(2)));
            ////设置相对过期时间
            //redis.SetString("name2", $"张三2！{DateTime.Now}",
            //    new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));
            //ViewBag.Msg = "数据返回成功！";
            //return View();
        }

        public IActionResult Privacy()
        {
            var result = HttpContext.Session.GetString("mysession");
            ViewBag.Msg = "数据读取成功！" + result;
            ////创建客户端访问
            //RedisCacheOptions options = new RedisCacheOptions();
            //options.Configuration = "127.0.0.1:6379";
            //options.InstanceName = "myredis";
            //var redisClient = new RedisCache(options);
            //写入数据
            //var result = redis.GetString("name");
            // var result2 = redis.GetString("name2");
            // ViewBag.Msg = "数据读取成功！"+result;
            // ViewBag.Msg2 = "数据读取成功！" + result2;
            //string prodJson = redis.GetString("prods");
            //if (string.IsNullOrEmpty(prodJson))
            //{
            //    HttpClient httpClient = new HttpClient();
            //    prodJson = await httpClient.GetStringAsync(Configuration.GetSection("APIUrl").Value);
            //    redis.SetString("prods", prodJson);
            //}
            //IEnumerable<Products> prodList = JsonConvert.DeserializeObject<IEnumerable<Products>>(prodJson);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
