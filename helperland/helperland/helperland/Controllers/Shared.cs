using helperland.Models;
using helperland.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;


namespace helperland.Controllers
{
    public class Shared : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelperLandDBContext _obj;
        public Shared(ILogger<HomeController> logger, HelperLandDBContext db)
        {
            _logger = logger;
            _obj = db;
        }

        [HttpPost]
        public ActionResult Login(LoginForgotPassModel reg)
        {
            var details = (from userlist in _obj.Users
                           where userlist.Email == reg.LoginModel.Email && userlist.Password == reg.LoginModel.Password
                           select new
                           {
                               userlist.UserId,
                               userlist.FirstName,
                               userlist.Email,
                               userlist.Password,
                               userlist.UserTypeId,
                               userlist.IsApproved

                           }).ToList();
            if (details.FirstOrDefault() != null)
            {
                if (details.FirstOrDefault().UserTypeId == 1)
                {

                    HttpContext.Session.SetInt32("CustomerId", details.FirstOrDefault().UserId);
                    HttpContext.Session.SetString("Customerfname", details.FirstOrDefault().FirstName);
                    if (reg.LoginModel.hiddenfield!=null)
                    {
                        return RedirectToAction("BookService", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Welcome", "Home");
                    }
                   
                }
                else if(details.FirstOrDefault().UserTypeId==2)
                {
                    if (reg.LoginModel.hiddenfield != null)
                    {
                        ModelState.AddModelError("Invalid","Only Customer can apply for service");
                        ViewBag.Message = String.Format("Invalid Login");
                        return View("~/Views/Home/Index.cshtml");
                    }
                    if (details.FirstOrDefault().IsApproved)
                    {
                        HttpContext.Session.SetInt32("SPId", details.FirstOrDefault().UserId);
                        HttpContext.Session.SetString("SPfname", details.FirstOrDefault().FirstName);
                        return RedirectToAction("WelcomeForSp", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Invalid", "Login after your status is approved");
                        ViewBag.Message = String.Format("Invalid Login");
                        return View("~/Views/Home/Index.cshtml");
                    }
                }
                else if (details.FirstOrDefault().UserTypeId == 3)
                {
                    if (reg.LoginModel.hiddenfield != null)
                    {
                        ModelState.AddModelError("Invalid", "Only Customer can apply for service");
                        ViewBag.Message = String.Format("Invalid Login");
                        return View("~/Views/Home/Index.cshtml");
                    }
                    return RedirectToAction("WelcomeForAdmin", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                ModelState.AddModelError("Invalid", "Invalid login");
                ViewBag.Message = String.Format("Invalid Login");
                return View("~/Views/Home/Index.cshtml");


            }



        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(UpdatePassword u)
        {
          
            User myuser = _obj.Users.Where(x => x.Email == HttpContext.Session.GetString("EmailForForgotPassword")).FirstOrDefault();
            myuser.Password = u.Password;
            _obj.Users.Update(myuser) ;
            _obj.SaveChanges();
            HttpContext.Session.Remove("EmailForForgotPassword");
            ViewBag.Message2 = String.Format("Successfull");
            return View("~/Views/Home/Index.cshtml");

        }

        public ActionResult checkEmailExist(LoginForgotPassModel reg)
        {
            var details = IsEmailExists(reg.ForgotPasswordModel.Email1);

            if (details)
            {
                HttpContext.Session.SetString("EmailForForgotPassword", reg.ForgotPasswordModel.Email1);
                return RedirectToAction("ForgotPassword");
            }
            else
            {
                ModelState.AddModelError("Email1", "EmailAddress does not exist");
                ViewBag.Message1 = String.Format("Invalid Login");
                return View("~/Views/Home/Index.cshtml");
            }
        }
        public bool IsEmailExists(string eMail)
        {
            var IsCheck = _obj.Users.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }

    }
}
