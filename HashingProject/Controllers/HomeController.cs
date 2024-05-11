using HashingProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
namespace HashingProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        private const int SaltSize = 16;
        public HomeController(ILogger<HomeController> logger , AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();     
        }
        [HttpPost]
        public IActionResult Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                   
                byte[] salt = GenerateSalt();

                byte[] hashedPassword = HashPassword(model.Password, salt);

                string saltString = Convert.ToBase64String(salt);
                string hashedPasswordString = Convert.ToBase64String(hashedPassword);
                var userinfo = new User() {Name = model.Name, Password = hashedPasswordString ,Salt = saltString};
                _appDbContext.Users.Add(userinfo);
                _appDbContext.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Username or Password is not correct!";
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
               var user =  _appDbContext.Users.FirstOrDefault(u=>u.Name == model.Name);
                byte[] storedSalt = Convert.FromBase64String(user.Salt);
                byte[] storedHashedPassword = Convert.FromBase64String(user.Password);

                byte[] enteredPasswordHash = HashPassword(model.Password, storedSalt);

                bool passwordsMatch = CompareByteArrays(enteredPasswordHash, storedHashedPassword);

                if (passwordsMatch)
                {
                    return RedirectToAction("Index", "Home");                 }
                else
                {
                    ViewBag.Message = "Username or Password is not correct!";
                    return View();
                }
            }
            ViewBag.Message = "Username or Password is not correct!";
            return View(); 
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                return pbkdf2.GetBytes(32); 
            }
        }

        private bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
