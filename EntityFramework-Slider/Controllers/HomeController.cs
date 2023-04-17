using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        public HomeController(AppDbContext context,
               IBasketService basketService,
               IProductService productService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.SetString("name", "Pervin");

            //Response.Cookies.Append("surname", "Rehimli", new CookieOptions { MaxAge = TimeSpan.FromMinutes(30) });

            //Book book = new Book
            //{
            //    Id = 1,
            //    Name = "Xosrov ve Shirin"
            //};

            //Response.Cookies.Append("book", JsonConvert.SerializeObject(book));
            
           

            List<Slider> sliders = await _context.Sliders.ToListAsync();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Blog> blogs = await _context.Blogs.Where(m=>!m.SoftDelete).ToListAsync();

            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Product> products = await _productService.GetAll();

            About abouts = await _context.Abouts.Include(m=>m.Advantages).FirstOrDefaultAsync();

            IEnumerable<Expert> experts = await _context.Experts.Where(m => !m.SoftDelete).ToListAsync();

            Subscribe subscribes = await _context.Subscribes.FirstOrDefaultAsync();

            IEnumerable<Say> says = await _context.Says.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => m.SoftDelete == false).ToListAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Blogs = blogs,
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Experts = experts,
                Subscribes = subscribes, 
                Says = says,
                Instagrams = instagrams
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            Product? dbProduct = await _productService.GetById((int)id);

            if (dbProduct == null) return NotFound();

            List<BasketVM> basket = _basketService.GetBasketDatas();

            BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbProduct.Id);

            _basketService.AddProductToBasket(existProduct, dbProduct, basket);

            int basketCount = basket.Sum(m=>m.Count);

            return Ok(basketCount);
        }


        



        //public IActionResult Test()
        //{
        //    var sessionData = HttpContext.Session.GetString("name");
        //    var cookieData = Request.Cookies["surname"];
        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);


        //    return Json(objectData);
        //}

        //class Book
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //}
    }
}