using helperland.Models;
using helperland.Models.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace helperland.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelperLandDBContext _helperlandDBContext;
        private readonly IWebHostEnvironment w;

        public HomeController(ILogger<HomeController> logger,HelperLandDBContext obj,IWebHostEnvironment web)
        {
            _logger = logger;
            _helperlandDBContext = obj;
            w = web;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Prices()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Faqs()
        {
            return View();
        }
        public IActionResult CustomerRegister()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CustomerRegister(RegisterModel u)

        {
            var details = IsEmailExists(u.Email);

            if (details)
            {

                ModelState.AddModelError("Email", "EmailAddress already in Use");
                return View();
            }
            else
            {
                User myuser = new User();
                myuser.FirstName = u.FirstName.ToString();
                myuser.LastName = u.LastName.ToString();
                myuser.Password = u.Password.ToString();
                myuser.Email = u.Email.ToString();
                myuser.Mobile = u.Mobile.ToString();
                myuser.UserTypeId = 1;
                myuser.WorksWithPets = false;
                myuser.CreatedDate = DateTime.Now;
                myuser.ModifiedDate = DateTime.Now;
                myuser.ModifiedBy = 1;
                myuser.IsApproved = true;


                _helperlandDBContext.Users.Add(myuser);
                _helperlandDBContext.SaveChanges();
                HttpContext.Session.SetString("Customerfname", myuser.FirstName);
                return RedirectToAction("Welcome", "Home");


            }
        }
        public bool IsEmailExists(string eMail)
        {
            var IsCheck = _helperlandDBContext.Users.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }
        public IActionResult Welcome()
        {
            ViewBag.data = HttpContext.Session.GetString("Customerfname");
            return View();
        }
        public IActionResult WelcomeForSp()
        {
            ViewBag.data = HttpContext.Session.GetString("SPfname");
            return View();
        }
        public IActionResult WelcomeForAdmin()
        {
            
            return View();
        }
        public IActionResult SPregisteration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SPregisteration(RegisterModel u)

        {
            var details = IsEmailExists(u.Email);

            if (details)
            {

                ModelState.AddModelError("Email", "EmailAddress already in Use");
                return View();
            }
            else
            {
                User myuser = new User();
                myuser.FirstName = u.FirstName.ToString();
                myuser.LastName = u.LastName.ToString();
                myuser.Password = u.Password.ToString();
                myuser.Email = u.Email.ToString();
                myuser.Mobile = u.Mobile.ToString();
                myuser.UserTypeId = 2;
                myuser.WorksWithPets = false;
                myuser.CreatedDate = DateTime.Now;
                myuser.ModifiedDate = DateTime.Now;
                myuser.ModifiedBy = 2;
                myuser.IsApproved =false;


                _helperlandDBContext.Users.Add(myuser);
                _helperlandDBContext.SaveChanges();
                HttpContext.Session.SetString("SPfname", myuser.FirstName);
                return RedirectToAction("Index");


            }
        }
        [HttpPost]
        public IActionResult contact(ContactUsModel u)

        {
            string uniqueFileName = null;

            if (u.upload != null)
            {
                string uploadsfolder = Path.Combine(w.WebRootPath, "ContactUsUploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + u.upload.FileName;

                String filepath = Path.Combine(uploadsfolder, uniqueFileName);
                u.upload.CopyTo(new FileStream(filepath, FileMode.Create));
                u.FileName = filepath;
                u.UploadFileName = uniqueFileName;
            }
            

            ContactU myuser = new ContactU();

            myuser.Name = u.FirstName.ToString() + " " + u.LastName.ToString();

            myuser.Email = u.Email.ToString();
            myuser.PhoneNumber = u.Mobile.ToString();
            myuser.Subject = u.Subject.ToString();
            myuser.Message = u.Message.ToString();
            myuser.UploadFileName = u.UploadFileName;
           
            if (u.upload != null)
            {
                myuser.FileName = u.upload.FileName;
            }
            else
            {
                myuser.FileName = null;
            }


            _helperlandDBContext.ContactUs.Add(myuser);
            _helperlandDBContext.SaveChanges();
            ViewBag.Msg = String.Format("Successfully sent");
            return View("~/Views/Home/Contact.cshtml");


        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
