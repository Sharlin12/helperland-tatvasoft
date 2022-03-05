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
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public bool IsEmailExists(string eMail)
        {
            var IsCheck = _helperlandDBContext.Users.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }
        [HttpPost]
        public IActionResult RescheduleSR(CustomerSideModel model)
        {
            ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.
            Cancelrequestid).FirstOrDefault();
            serviceRequest.ServiceStartDate = DateTime.Parse(model.date + " " + model.time);
            _helperlandDBContext.ServiceRequests.Update(serviceRequest);
            _helperlandDBContext.SaveChanges();
            return RedirectToAction("Welcome");
        }
        [HttpPost]
        public IActionResult cancelrequest(CustomerSideModel model)
        {
            ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.
            Cancelrequestid).FirstOrDefault();
            
            serviceRequest.Status = 2;
            _helperlandDBContext.ServiceRequests.Update(serviceRequest);
            _helperlandDBContext.SaveChanges();

            return RedirectToAction("Welcome");

        }
        public IActionResult Welcome()
        {
            ViewBag.data = HttpContext.Session.GetString("Customerfname");
          
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                CustomerSideModel csmodel = new CustomerSideModel();
                csmodel.serviceRequests = from ServiceRequest in _helperlandDBContext.ServiceRequests
                                     where ServiceRequest.UserId == HttpContext.Session.GetInt32("CustomerId") && ServiceRequest.Status == 1  
                                     select ServiceRequest;
                csmodel.serviceRequestAddresses = from serviceRequestAddresses in _helperlandDBContext.ServiceRequestAddresses
                                             select serviceRequestAddresses;
                csmodel.serviceRequestExtras = from serviceRequestExtras in _helperlandDBContext.ServiceRequestExtras
                                          select serviceRequestExtras;


                return View(csmodel);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
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
        public IActionResult ServiceHistory()
        {
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                CustomerSideModel csmodel = new CustomerSideModel();
                csmodel.serviceRequests = from ServiceRequest in _helperlandDBContext.ServiceRequests
                                          where ServiceRequest.UserId == HttpContext.Session.GetInt32("CustomerId") && ServiceRequest.Status != 1
                                          select ServiceRequest;
                csmodel.serviceRequestAddresses = from serviceRequestAddresses in _helperlandDBContext.ServiceRequestAddresses
                                                  select serviceRequestAddresses;
                csmodel.serviceRequestExtras = from serviceRequestExtras in _helperlandDBContext.ServiceRequestExtras
                                               select serviceRequestExtras;

                csmodel.user = from u in _helperlandDBContext.Users select u;
                csmodel.myrate = from r in _helperlandDBContext.Ratings select r;


                return View(csmodel);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        [HttpPost]
        public IActionResult AddNewAddress(CustomerSettingsModel model)
        {
            UserAddress userAddress = new UserAddress();
            userAddress.UserId = (int)HttpContext.Session.GetInt32("CustomerId");
            userAddress.AddressLine1 = model.myaddress.AddressLine2.ToString();
            userAddress.AddressLine2 = model.myaddress.AddressLine1.ToString();
            userAddress.City = model.myaddress.City.ToString();
            userAddress.PostalCode = model.myaddress.PostalCode.ToString();
            userAddress.IsDefault = false;
            userAddress.IsDeleted = false;
            if (model.myaddress.Mobile != null)
            {
                userAddress.Mobile = model.myaddress.Mobile.ToString();
            }
            _helperlandDBContext.UserAddresses.Add(userAddress);
            _helperlandDBContext.SaveChanges();
            TempData["newadd"] = "Bill";
            return RedirectToAction("CustomerSettings");
        }
        [HttpPost]
        public IActionResult changeUserPassword(CustomerSettingsModel model)
        {
            User u = _helperlandDBContext.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).FirstOrDefault();
            if (u.Password == model.oldpassword)
            {
                u.Password = model.Password.ToString();
                _helperlandDBContext.Users.Update(u);
                _helperlandDBContext.SaveChanges();
                TempData["changepass"] = "Bill";
                return RedirectToAction("CustomerSettings");
            }
            else
            {
                TempData["errorchangepass"] = "Bill";
                return RedirectToAction("CustomerSettings");
            }
            
        }
      
        public IActionResult FavoritePros()
        {
            CustomerSideModel csmodel = new CustomerSideModel();
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                csmodel.serviceRequests = from sr in _helperlandDBContext.ServiceRequests
                                          where sr.UserId == HttpContext.Session.GetInt32("CustomerId") && sr.Status == 3
                                          select sr;
                csmodel.user= from sr in _helperlandDBContext.Users
                              where sr.UserTypeId==2
                              select sr;
                csmodel.sr = from sr in _helperlandDBContext.ServiceRequests
                                          where sr.Status == 3
                                          select sr;
                csmodel.myrate = from r in _helperlandDBContext.Ratings select r;

                csmodel.favoriteAndBlockeds = from fb in _helperlandDBContext.FavoriteAndBlockeds where fb.UserId == HttpContext.Session.GetInt32("CustomerId") select fb;

                return View(csmodel);
            }
            else
            {
                return RedirectToAction("Index");
            }
                
        }
        [HttpPost]
        public IActionResult unfavorite(CustomerSideModel m)
        {
        FavoriteAndBlocked fb = _helperlandDBContext.FavoriteAndBlockeds.Where(x => x.UserId == m.userid && x.TargetUserId == m.spid).FirstOrDefault();
            if (fb != null)
            {

                fb.IsFavorite = false;
                fb.IsBlocked = false;
                _helperlandDBContext.FavoriteAndBlockeds.Update(fb);
                _helperlandDBContext.SaveChanges();
            }
            return RedirectToAction("FavoritePros");
        }
            [HttpPost]
        public IActionResult makeFavorite(CustomerSideModel m)
        {


            FavoriteAndBlocked fb = _helperlandDBContext.FavoriteAndBlockeds.Where(x => x.UserId == m.userid && x.TargetUserId == m.spid).FirstOrDefault();
            if (fb != null)
            {
                
                fb.IsFavorite = true;
                fb.IsBlocked = false;
                _helperlandDBContext.FavoriteAndBlockeds.Update(fb);
                _helperlandDBContext.SaveChanges();
            }
            else
            {
                FavoriteAndBlocked fb1 = new FavoriteAndBlocked();
                fb1.UserId = m.userid;
                fb1.TargetUserId = m.spid;
                fb1.IsFavorite = true;
                fb1.IsBlocked = false;
                _helperlandDBContext.FavoriteAndBlockeds.Add(fb1);
                _helperlandDBContext.SaveChanges();
            }
           
            return RedirectToAction("FavoritePros");
        }
        [HttpPost]
        public IActionResult makeblock(CustomerSideModel m)
        {
            FavoriteAndBlocked fb = _helperlandDBContext.FavoriteAndBlockeds.Where(x => x.UserId == m.userid && x.TargetUserId == m.spid).FirstOrDefault();
            if (fb != null)
            {

                fb.IsFavorite = false;
                fb.IsBlocked = true;
                _helperlandDBContext.FavoriteAndBlockeds.Update(fb);
                _helperlandDBContext.SaveChanges();
            }
            else
            {
                FavoriteAndBlocked fb1 = new FavoriteAndBlocked();
                fb1.UserId = m.userid;
                fb1.TargetUserId = m.spid;
                fb1.IsFavorite = false;
                fb1.IsBlocked = true;
                _helperlandDBContext.FavoriteAndBlockeds.Add(fb1);
                _helperlandDBContext.SaveChanges();
            }

            return RedirectToAction("FavoritePros");
        }
        [HttpPost]
        public IActionResult deleteaddress(CustomerSettingsModel m)
        {
            UserAddress ua = _helperlandDBContext.UserAddresses.Where(x => x.AddressId == m.deleteaddid).FirstOrDefault();
            ua.IsDeleted = true;
            _helperlandDBContext.UserAddresses.Update(ua);
            _helperlandDBContext.SaveChanges();
            TempData["deleteadd"] = "Bill";
            return RedirectToAction("CustomerSettings");
        }
        [HttpPost]
        public IActionResult editaddress(CustomerSettingsModel m)
        {
            UserAddress ua1 = _helperlandDBContext.UserAddresses.Where(x => x.AddressId == m.deleteaddid).FirstOrDefault();
            ua1.AddressLine1 = m.myaddress.AddressLine2.ToString() ;
            ua1.AddressLine2 = m.myaddress.AddressLine1.ToString();
            ua1.PostalCode = m.myaddress.PostalCode.ToString();
           
            ua1.City = m.myaddress.City.ToString();
            if (m.myaddress.Mobile != null)
            {
                ua1.Mobile = m.myaddress.Mobile.ToString();
            }
            _helperlandDBContext.UserAddresses.Update(ua1);
            _helperlandDBContext.SaveChanges();
            TempData["editadd"] = "Bill";
            return RedirectToAction("CustomerSettings");
        }
        public IActionResult CustomerSettings()
        {
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                
                var x = from u in _helperlandDBContext.Users
                               where u.UserId == HttpContext.Session.GetInt32("CustomerId")
                                      select u;
                ViewBag.personal = x.FirstOrDefault();
                CustomerSettingsModel csmodel = new CustomerSettingsModel();
                csmodel.userAddresses = from ua in _helperlandDBContext.UserAddresses
                                          where ua.UserId == HttpContext.Session.GetInt32("CustomerId") && ua.IsDeleted!=true
                                          select ua;
                return View(csmodel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult customersettingstab1(CustomerSettingsModel model)
        {
            
            User u = _helperlandDBContext.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).FirstOrDefault();
            u.FirstName = model.fname;
            u.LastName = model.lname;
            u.Mobile = model.phone;
            u.ModifiedDate = DateTime.Now;
            if(model.day!=null && model.month!=null && model.year != null)
            {
                u.DateOfBirth = DateTime.Parse(model.day + "-" + model.month + "-" + model.year);
            }
            
            u.LanguageId =int.Parse( model.lang);
            _helperlandDBContext.Users.Update(u);
            _helperlandDBContext.SaveChanges();
            HttpContext.Session.SetString("Customerfname", model.fname);
            TempData["personald"] = "Bill";
            return RedirectToAction("CustomerSettings");
        }
        public IActionResult BookService()
        {
            if ( HttpContext.Session.GetInt32("CustomerId") != null)
            {
                return View();
            }
            else
            {
                return View("Index");
            }
            
        }
        [HttpPost]
        public IActionResult CheckPostalCode(BookServiceModel model)
        {

            var details = (from userlist in _helperlandDBContext.Users
                           where userlist.UserTypeId == 2 && userlist.ZipCode == model.ZipCodeMatching.PostalCode
                           select new
                           {
                               userlist.UserId,
                               userlist.FirstName,

                           }).ToList();
            if (details.FirstOrDefault() != null)
            {
                BookServiceModel model1 = new BookServiceModel();
                model1.ua = from UserAddress in _helperlandDBContext.UserAddresses
                            where UserAddress.UserId== HttpContext.Session.GetInt32("CustomerId") && UserAddress.IsDeleted != true && UserAddress.PostalCode==model.ZipCodeMatching.PostalCode
                            select UserAddress;
                ViewBag.MatchedPC = "Matched";
                ViewBag.zip = model.ZipCodeMatching.PostalCode;
                ViewBag.add = "address";
                var cities = (from userlist in _helperlandDBContext.UserAddresses
                               where userlist.PostalCode == model.ZipCodeMatching.PostalCode
                               select new
                               {
                                   userlist.UserId,
                                   userlist.City,

                               }).ToList();
                ViewBag.cityy = cities.FirstOrDefault();
                if (ViewBag.cityy != null)
                {
                    ViewBag.city = cities.FirstOrDefault().City;
                }
               
                return View("BookService",model1);
            }
            else
            {
                ViewBag.UnmatchedPc = "Not Matched";
                return View("BookService");
            }
        }
        [HttpPost]
        public IActionResult RateService(CustomerSideModel model)
        {
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                Rating rating = new Rating();
                var rateavg = (decimal.Parse(model.friendly) + decimal.Parse(model.ontime) + decimal.Parse(model.quality)) / 3;
                rating.ServiceRequestId = model.rate.ServiceRequestId;
                rating.RatingFrom = model.rate.RatingFrom;
                rating.RatingTo = model.rate.RatingTo;
                rating.Ratings = rateavg;
                rating.Comments = model.rate.Comments;
                rating.RatingDate = DateTime.Now;
                rating.OnTimeArrival = decimal.Parse(model.ontime);
                rating.Friendly = decimal.Parse(model.friendly);
                rating.QualityOfService = decimal.Parse(model.quality);
                _helperlandDBContext.Ratings.Add(rating);
                _helperlandDBContext.SaveChanges();
                return RedirectToAction("ServiceHistory");
            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }
        [HttpPost]
        public IActionResult ServiceBooking(BookServiceModel model)
        {
            var rand = new Random();
            var serviceid = rand.Next(1000, 9999);
            ServiceRequest sr = new ServiceRequest();
            sr.UserId = (int)HttpContext.Session.GetInt32("CustomerId");
            sr.ServiceId = serviceid;
            sr.ServiceStartDate= DateTime.Parse(model.sap.YourDate + " " + model.sap.yourtime);
            sr.ZipCode = model.ZipCodeMatching.PostalCode.ToString();
            sr.ServiceHourlyRate = 18;
            sr.Status = 1;
            sr.ServiceHours = float.Parse(model.sap.serviceHrs);
            if ((model.sap.extraHrs) != null)
            {
                sr.ExtraHours = float.Parse(model.sap.extraHrs);
            }
            
            sr.SubTotal = int.Parse(model.sap.subtotal);
            sr.Discount = 20;
            sr.TotalCost = int.Parse(model.sap.totalcost);
            if (model.sap.comments != null)
            {
                sr.Comments = model.sap.comments.ToString();
            }
            
            sr.PaymentDue = false;
            sr.HasPets = model.sap.haspets;
            sr.CreatedDate = DateTime.Now;
            sr.ModifiedDate = DateTime.Now;
            sr.Distance = 0;
            _helperlandDBContext.ServiceRequests.Add(sr);
            _helperlandDBContext.SaveChanges();

            /*add address*/
            var oldadd = (from userlist in _helperlandDBContext.UserAddresses
                          where userlist.AddressId == int.Parse(model.selectedaddress) && userlist.IsDeleted!=true
                          select new
                          {
                              userlist.AddressLine1,
                              userlist.AddressLine2,
                              userlist.City,
                              userlist.State,
                              userlist.PostalCode,
                              userlist.Mobile,
                              userlist.Email
                          }).ToList();
            var serviceID = (from userlist in _helperlandDBContext.ServiceRequests
                            where userlist.ServiceId == serviceid 
                             select new
                            {
                                userlist.ServiceRequestId
                            }).ToList();
            if (oldadd.FirstOrDefault() != null)
            {

                ServiceRequestAddress serviceRequestAddress = new ServiceRequestAddress();
                serviceRequestAddress.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                serviceRequestAddress.AddressLine1 = oldadd.FirstOrDefault().AddressLine2;
                serviceRequestAddress.AddressLine2 = oldadd.FirstOrDefault().AddressLine1;
                serviceRequestAddress.City = oldadd.FirstOrDefault().City;
                serviceRequestAddress.State = oldadd.FirstOrDefault().State;
                serviceRequestAddress.PostalCode = oldadd.FirstOrDefault().PostalCode;
                serviceRequestAddress.Mobile = oldadd.FirstOrDefault().Mobile;
                serviceRequestAddress.Email = oldadd.FirstOrDefault().Email;
                _helperlandDBContext.ServiceRequestAddresses.Add(serviceRequestAddress);
                _helperlandDBContext.SaveChanges();


            }
            else
            {
                ServiceRequestAddress serviceRequestAddress = new ServiceRequestAddress();
                serviceRequestAddress.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                serviceRequestAddress.AddressLine1 = model.UserAddressModel.AddressLine2.ToString();
                serviceRequestAddress.AddressLine2 = model.UserAddressModel.AddressLine1.ToString();
                serviceRequestAddress.City = model.UserAddressModel.City.ToString();
                serviceRequestAddress.PostalCode = model.UserAddressModel.PostalCode.ToString();
                if (model.UserAddressModel.Mobile != null)
                {
                    serviceRequestAddress.Mobile = model.UserAddressModel.Mobile.ToString();
                }
                _helperlandDBContext.ServiceRequestAddresses.Add(serviceRequestAddress);
                _helperlandDBContext.SaveChanges();
                UserAddress userAddress = new UserAddress();
                userAddress.UserId = (int)HttpContext.Session.GetInt32("CustomerId");
                userAddress.AddressLine1 = model.UserAddressModel.AddressLine2.ToString();
                userAddress.AddressLine2 = model.UserAddressModel.AddressLine1.ToString();
                userAddress.City = model.UserAddressModel.City.ToString();
                userAddress.PostalCode = model.UserAddressModel.PostalCode.ToString();
                userAddress.IsDefault = false;
                userAddress.IsDeleted = false;
                if (model.UserAddressModel.Mobile != null)
                {
                    userAddress.Mobile = model.UserAddressModel.Mobile.ToString();
                }
                _helperlandDBContext.UserAddresses.Add(userAddress);
                _helperlandDBContext.SaveChanges();

            }
            if (model.sap.extra1 == true)
            {
                ServiceRequestExtra serviceRequestExtra = new ServiceRequestExtra();
                serviceRequestExtra.ServiceExtraId = 1;
                serviceRequestExtra.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                _helperlandDBContext.ServiceRequestExtras.Add(serviceRequestExtra);
                _helperlandDBContext.SaveChanges();
            }
            if (model.sap.extra2 == true)
            {
                ServiceRequestExtra serviceRequestExtra = new ServiceRequestExtra();
                serviceRequestExtra.ServiceExtraId = 2;
                serviceRequestExtra.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                _helperlandDBContext.ServiceRequestExtras.Add(serviceRequestExtra);
                _helperlandDBContext.SaveChanges();
            }
            if (model.sap.extra3 == true)
            {
                ServiceRequestExtra serviceRequestExtra = new ServiceRequestExtra();
                serviceRequestExtra.ServiceExtraId = 3;
                serviceRequestExtra.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                _helperlandDBContext.ServiceRequestExtras.Add(serviceRequestExtra);
                _helperlandDBContext.SaveChanges();
            }
            if (model.sap.extra4== true)
            {
                ServiceRequestExtra serviceRequestExtra = new ServiceRequestExtra();
                serviceRequestExtra.ServiceExtraId = 4;
                serviceRequestExtra.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                _helperlandDBContext.ServiceRequestExtras.Add(serviceRequestExtra);
                _helperlandDBContext.SaveChanges();
            }
            if (model.sap.extra5== true)
            {
                ServiceRequestExtra serviceRequestExtra = new ServiceRequestExtra();
                serviceRequestExtra.ServiceExtraId = 5;
                serviceRequestExtra.ServiceRequestId = serviceID.FirstOrDefault().ServiceRequestId;
                _helperlandDBContext.ServiceRequestExtras.Add(serviceRequestExtra);
                _helperlandDBContext.SaveChanges();
            }
            ViewBag.serviceid = serviceid;
            return View("bookservice");
        
            
        }
        /*[HttpPost]
        public IActionResult AddNewAddress(BookServiceModel u)

        {
            
               UserAddress myuser=new UserAddress();
                myuser.UserId = (int)HttpContext.Session.GetInt32("CustomerId");
                myuser.AddressLine1 = u.UserAddressModel.AddressLine1.ToString();
                myuser.AddressLine2 = u.UserAddressModel.AddressLine2.ToString();
                myuser.PostalCode = u.UserAddressModel.PostalCode.ToString();
                myuser.City = u.UserAddressModel.City.ToString();
                if (u.UserAddressModel.Mobile != null)
                {
                    myuser.Mobile = u.UserAddressModel.Mobile.ToString();
                }
                    
                    myuser.IsDefault = false;
                    myuser.IsDeleted = false;

                _helperlandDBContext.UserAddresses.Add(myuser);
                _helperlandDBContext.SaveChanges();
                    
                    ViewBag.thirdscreen = "thirdtab";
                    return View("BookService");



        }*/
        /* [HttpPost]
         public IActionResult schedule(BookServiceModel model)
         {
             ViewBag.extra = DateTime.Parse(model.sap.YourDate+" "+model.sap.yourtime);
             ViewBag.zip = model.ZipCodeMatching.PostalCode;
             ViewBag.value = model.sap.serviceHrs + " " + model.sap.totalcost + " " + model.sap.subtotal;
             ViewBag.has = model.sap.haspets;
             ViewBag.comments = model.sap.comments;
             ViewBag.extrahr = model.sap.extraHrs;
             return View();
         }*/
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
