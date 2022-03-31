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
using System.Net.Mail;
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
                HttpContext.Session.SetInt32("CustomerId", myuser.UserId);
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
            if (model.serviceproid == 0)
            {
                ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.
                Cancelrequestid).FirstOrDefault();
                serviceRequest.ServiceStartDate = DateTime.Parse(model.date + " " + model.time);
                _helperlandDBContext.ServiceRequests.Update(serviceRequest);
                _helperlandDBContext.SaveChanges();
                return RedirectToAction("Welcome");
            }
            else
            {
                var x = from ServiceRequest in _helperlandDBContext.ServiceRequests
                        where ServiceRequest.ServiceProviderId == model.serviceproid
                        && ServiceRequest.Status == 1 && ServiceRequest.ServiceRequestId != model.Cancelrequestid
                        select ServiceRequest;
                var mstartdate = DateTime.Parse(model.date + " " + model.time);
                var menddate = mstartdate.AddHours(model.endtime);
                var j = 0;
                foreach (var servicedatecheck in x)
                {
                    var endtimefromdb = servicedatecheck.ServiceStartDate.AddHours(servicedatecheck.ServiceHours);
                    if (endtimefromdb >= mstartdate && endtimefromdb < menddate)
                    {
                        j = 1;
                    }

                    else if (menddate >= servicedatecheck.ServiceStartDate && mstartdate < endtimefromdb)
                    {
                        j = 1;
                    }
                    else if (mstartdate >= servicedatecheck.ServiceStartDate && menddate <= endtimefromdb)
                    {
                        j = 1;
                    }
                    else if (servicedatecheck.ServiceStartDate >= mstartdate && endtimefromdb <= menddate)
                    {
                        j = 1;
                    }
                    if (j == 1)
                    {
                        TempData["date"] = servicedatecheck.ServiceStartDate.ToShortDateString();
                        TempData["s"] = servicedatecheck.ServiceStartDate.ToString("HH:mm");
                        TempData["e"] = servicedatecheck.ServiceStartDate.AddHours(servicedatecheck.ServiceHours).ToString("HH:mm");
                        break;
                    }
                }
                if (j == 0)
                {
                    ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.
            Cancelrequestid).FirstOrDefault();
                    serviceRequest.ServiceStartDate = DateTime.Parse(model.date + " " + model.time);
                    _helperlandDBContext.ServiceRequests.Update(serviceRequest);
                    _helperlandDBContext.SaveChanges();
                    User u = _helperlandDBContext.Users.Where(x => x.UserId == serviceRequest.ServiceProviderId).FirstOrDefault();
                    MailMessage mail = new MailMessage();
                    mail.To.Add(u.Email);
                    mail.From = new MailAddress("helperland12@gmail.com");
                    mail.Subject = "Reschedule Service Request by Customer";
                    mail.Body = "Service Request " + serviceRequest.ServiceId + " has been rescheduled by customer. New date and time are " + DateTime.Parse(model.date + " " + model.time) + ".";
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                    smtp.Send(mail);
                    smtp.Dispose();
                }
                else if (j == 1)
                {
                    TempData["conflict11"] = "conflict";


                }
                return RedirectToAction("Welcome");
            }
        }
        [HttpPost]
        public IActionResult cancelrequest(CustomerSideModel model)
        {
            ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.
            Cancelrequestid).FirstOrDefault();
            
            serviceRequest.Status = 2;
            _helperlandDBContext.ServiceRequests.Update(serviceRequest);
            _helperlandDBContext.SaveChanges();
            if (serviceRequest.ServiceProviderId > 0)
            {
                User u = _helperlandDBContext.Users.Where(x => x.UserId == serviceRequest.ServiceProviderId).FirstOrDefault();
                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Cancelled Service Request by Customer";
                mail.Body = "Service Request " + serviceRequest.ServiceId + " has been cancelled by customer.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
            }
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
                csmodel.user = from u in _helperlandDBContext.Users select u;
                
                csmodel.myrate = from r in _helperlandDBContext.Ratings select r;

                return View(csmodel);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        public IActionResult spservicehistory()
        {
            if (HttpContext.Session.GetInt32("SPId") != null)
            {

                ServiceProviderSideModel sp = new ServiceProviderSideModel();
                sp.serviceRequests = from serviceRequests in _helperlandDBContext.ServiceRequests
                                     where serviceRequests.Status == 3 && serviceRequests.ServiceProviderId == HttpContext.Session.GetInt32("SPId")
                                     select serviceRequests;
                sp.serviceRequestAddresses = from serviceRequestAddresses in _helperlandDBContext.ServiceRequestAddresses
                                             select serviceRequestAddresses;
                sp.serviceRequestExtras = from serviceRequestExtras in _helperlandDBContext.ServiceRequestExtras
                                          select serviceRequestExtras;

                sp.users = from User in _helperlandDBContext.Users
                           where User.UserTypeId == 1
                           select User;
                return View(sp);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public IActionResult UpcomingService()
        {
            if (HttpContext.Session.GetInt32("SPId") != null)
            {

                ServiceProviderSideModel sp = new ServiceProviderSideModel();
                sp.serviceRequests = from serviceRequests in _helperlandDBContext.ServiceRequests
                                     where serviceRequests.Status == 1 && serviceRequests.ServiceProviderId == HttpContext.Session.GetInt32("SPId")
                                     select serviceRequests;
                sp.serviceRequestAddresses = from serviceRequestAddresses in _helperlandDBContext.ServiceRequestAddresses
                                             select serviceRequestAddresses;
                sp.serviceRequestExtras = from serviceRequestExtras in _helperlandDBContext.ServiceRequestExtras
                                          select serviceRequestExtras;

                sp.users = from User in _helperlandDBContext.Users
                           where User.UserTypeId == 1
                           select User;
                return View(sp);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult CompleteSr(ServiceProviderSideModel model)
        {
            ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.srid).FirstOrDefault();
            serviceRequest.Status = 3;
            _helperlandDBContext.ServiceRequests.Update(serviceRequest);
            _helperlandDBContext.SaveChanges();
            return RedirectToAction("UpcomingService");
        }
        [HttpPost]
        public IActionResult CancellSr(ServiceProviderSideModel model)
        {
            ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.srid).FirstOrDefault();
            serviceRequest.Status = 1;
            serviceRequest.ServiceProviderId = null;
            serviceRequest.SpacceptedDate = null;
            _helperlandDBContext.ServiceRequests.Update(serviceRequest);
            _helperlandDBContext.SaveChanges();
            return RedirectToAction("UpcomingService");
        }
        [HttpPost]
        public IActionResult AcceptSr(ServiceProviderSideModel model)
        {
            var x = from ServiceRequest in _helperlandDBContext.ServiceRequests
                    where ServiceRequest.ServiceProviderId == HttpContext.Session.GetInt32("SPId")
                    && ServiceRequest.Status == 1
                    select ServiceRequest;

            if (x.ToList() != null)
            {
                var j = 0;
                foreach (var servicedatecheck in x)
                {
                    var endtimefromdb = servicedatecheck.ServiceStartDate.AddHours(servicedatecheck.ServiceHours + 1);
                    if (endtimefromdb >= model.starttime && endtimefromdb < model.endtime)
                    {
                        j = 1;
                    }
                    else if (model.endtime >= servicedatecheck.ServiceStartDate && model.starttime < endtimefromdb)
                    {
                        j = 1;
                    }
                    else if (model.starttime >= servicedatecheck.ServiceStartDate && model.endtime <= endtimefromdb)
                    {
                        j = 1;
                    }
                    else if (servicedatecheck.ServiceStartDate >= model.starttime && endtimefromdb <= model.endtime)
                    {
                        j = 1;
                    }

                }
                if (j == 0)
                {
                    ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.srid).FirstOrDefault();
                    serviceRequest.ServiceProviderId = HttpContext.Session.GetInt32("SPId");
                    serviceRequest.SpacceptedDate = DateTime.Now;
                    serviceRequest.Status = 1;
                    _helperlandDBContext.ServiceRequests.Update(serviceRequest);
                    _helperlandDBContext.SaveChanges();
                    var spid1 = serviceRequest.ServiceProviderId;
                    var spinfo = from u in _helperlandDBContext.Users
                                 where u.UserId != spid1 && u.ZipCode == serviceRequest.ZipCode && u.UserTypeId == 2
                                 select u;
                    if (spinfo != null)
                    {
                        foreach (var email in spinfo)
                        {
                            MailMessage mail = new MailMessage();
                            mail.To.Add(email.Email);
                            mail.From = new MailAddress("helperland12@gmail.com");
                            mail.Subject = "Service request is not available";
                            mail.Body = "Service Request " + serviceRequest.ServiceId + " is no more available now!";
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                            smtp.Send(mail);
                            smtp.Dispose();
                        }
                    }
                    
                    return RedirectToAction("WelcomeForSp");
                }
                else
                {
                    TempData["conflict"] = "conflict";
                    return RedirectToAction("WelcomeForSp");

                }

            }
            else
            {
                ServiceRequest serviceRequest = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == model.srid).FirstOrDefault();
                serviceRequest.ServiceProviderId = HttpContext.Session.GetInt32("SPId");
                serviceRequest.SpacceptedDate = DateTime.Now;
                serviceRequest.Status = 1;
                _helperlandDBContext.ServiceRequests.Update(serviceRequest);
                _helperlandDBContext.SaveChanges();

                return RedirectToAction("WelcomeForSp");
            }


            
        }
        public IActionResult WelcomeForSp()
        {
           
            if (HttpContext.Session.GetInt32("SPId") != null)
            {
                User abc = _helperlandDBContext.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("SPId")).FirstOrDefault();
                var x = from User in _helperlandDBContext.Users
                        where User.UserId == HttpContext.Session.GetInt32("SPId")
                        select User;
                ServiceProviderSideModel sp = new ServiceProviderSideModel();
                sp.serviceRequests = from serviceRequests in _helperlandDBContext.ServiceRequests
                                     where serviceRequests.ZipCode == abc.ZipCode && serviceRequests.Status == 1 && serviceRequests.ServiceProviderId == null
                                     select serviceRequests;
                sp.serviceRequestswithoutpets = from serviceRequests in _helperlandDBContext.ServiceRequests
                                                where serviceRequests.ZipCode == abc.ZipCode && serviceRequests.Status == 1 && serviceRequests.HasPets == false && serviceRequests.ServiceProviderId == null
                                                select serviceRequests;
                sp.serviceRequestAddresses = from serviceRequestAddresses in _helperlandDBContext.ServiceRequestAddresses
                                             select serviceRequestAddresses;
                sp.serviceRequestExtras = from serviceRequestExtras in _helperlandDBContext.ServiceRequestExtras
                                          select serviceRequestExtras;
                sp.favoriteAndBlockeds = from FavoriteAndBlocked in _helperlandDBContext.FavoriteAndBlockeds
                                         where FavoriteAndBlocked.TargetUserId == HttpContext.Session.GetInt32("SPId")
                                         select FavoriteAndBlocked;
                sp.favoriteAndBlockeds1 = from FavoriteAndBlocked in _helperlandDBContext.FavoriteAndBlockeds
                                         where FavoriteAndBlocked.UserId == HttpContext.Session.GetInt32("SPId")
                                         select FavoriteAndBlocked;
                sp.users = from User in _helperlandDBContext.Users
                           where User.UserTypeId == 1
                           select User;


                return View(sp);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        public IActionResult WelcomeForAdmin()
        {
            AdminSideModel model = new AdminSideModel();
            model.myuser = from u in _helperlandDBContext.Users
                           where u.UserTypeId != 3
                           select u;
            return View(model);
        }
        [HttpPost]
        public IActionResult activateuser(AdminSideModel model)
        {
            User u = _helperlandDBContext.Users.Where(x => x.UserId == model.iduser).FirstOrDefault();
            u.IsApproved = true;
            u.ModifiedBy = 3;
            u.ModifiedDate = DateTime.Now;
            _helperlandDBContext.Users.Update(u);
            _helperlandDBContext.SaveChanges();
            return RedirectToAction("WelcomeForAdmin");
        }
        [HttpPost]
        public IActionResult deactivateuser(AdminSideModel model)
        {
            User u = _helperlandDBContext.Users.Where(x => x.UserId == model.iduser).FirstOrDefault();
            u.IsApproved = false;
            u.ModifiedBy = 3;
            u.ModifiedDate = DateTime.Now;
            _helperlandDBContext.Users.Update(u);
            _helperlandDBContext.SaveChanges();
            return RedirectToAction("WelcomeForAdmin");
        }
        public IActionResult ServiceRequestAdmin(AdminSideModel model)
        {
            model.sr = from sr in _helperlandDBContext.ServiceRequests select sr;
            model.sradd = from add in _helperlandDBContext.ServiceRequestAddresses select add;
            model.myuser = from user in _helperlandDBContext.Users select user;
            model.myrate = from rate in _helperlandDBContext.Ratings select rate;
            return View(model);
        }
        [HttpPost]
        public IActionResult editsrbyadmin(AdminSideModel m)
        {
            ServiceRequest sr=_helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == m.Srid).FirstOrDefault();
            sr.ServiceStartDate= DateTime.Parse(m.date1 + " " + m.time1);
            sr.ModifiedBy = 3;
            sr.ModifiedDate = DateTime.Now;
            _helperlandDBContext.ServiceRequests.Update(sr);
            _helperlandDBContext.SaveChanges();
            ServiceRequestAddress serviceRequestAddress = _helperlandDBContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == m.Srid).FirstOrDefault();
            serviceRequestAddress.AddressLine1 = m.Add2;
            serviceRequestAddress.AddressLine2 = m.Add1;
            serviceRequestAddress.City = m.City;
            
            serviceRequestAddress.PostalCode = m.zipcode;
            
           
            _helperlandDBContext.ServiceRequestAddresses.Update(serviceRequestAddress);
            _helperlandDBContext.SaveChanges();
            User u = _helperlandDBContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault();
            User u1 = _helperlandDBContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault();
            if (sr.ServiceProviderId > 0)
            {

                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Updated By Admin";
                mail.Body = "Service Request is " + sr.ServiceId + " is updated by admin.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
                MailMessage mail1 = new MailMessage();
                mail1.To.Add(u1.Email);
                mail1.From = new MailAddress("helperland12@gmail.com");
                mail1.Subject = "Updated By Admin";
                mail1.Body = "Service Request is " + sr.ServiceId + " is updated by admin.";
                mail1.IsBodyHtml = true;
                SmtpClient smtp1 = new SmtpClient("smtp.gmail.com", 587);
                smtp1.EnableSsl = true;
                smtp1.UseDefaultCredentials = false;
                smtp1.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp1.Send(mail1);
                smtp1.Dispose();

            }
            else
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Updated By Admin";
                mail.Body = "Service Request is "+sr.ServiceId+" is updated by admin.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
            }
            return RedirectToAction("ServiceRequestAdmin");


        }
        [HttpPost]
        public IActionResult CancelSRbyCust(AdminSideModel m)
        {
            ServiceRequest sr = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == m.Srid).FirstOrDefault();
            sr.Status = 2;
           
            sr.ModifiedBy = 3;
            sr.ModifiedDate = DateTime.Now;
            _helperlandDBContext.ServiceRequests.Update(sr);
            _helperlandDBContext.SaveChanges();
            User u = _helperlandDBContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault();
            User u1 = _helperlandDBContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault();
            if (sr.ServiceProviderId > 0)
            {

                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Cancelled By Admin";
                mail.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from customer.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
                MailMessage mail1 = new MailMessage();
                mail1.To.Add(u1.Email);
                mail1.From = new MailAddress("helperland12@gmail.com");
                mail1.Subject = "Cancelled By Admin";
                mail1.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from customer.";
                mail1.IsBodyHtml = true;
                SmtpClient smtp1 = new SmtpClient("smtp.gmail.com", 587);
                smtp1.EnableSsl = true;
                smtp1.UseDefaultCredentials = false;
                smtp1.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp1.Send(mail1);
                smtp1.Dispose();

            }
            else
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Cancelled By Admin";
                mail.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from customer.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
            }

            return RedirectToAction("ServiceRequestAdmin");
        }
        public IActionResult CancelSRbySP(AdminSideModel m)
        {
            ServiceRequest sr = _helperlandDBContext.ServiceRequests.Where(x => x.ServiceRequestId == m.Srid).FirstOrDefault();
            sr.Status = 1;
            sr.ServiceProviderId = null;
            sr.SpacceptedDate = null;
            sr.ModifiedBy = 3;
            sr.ModifiedDate = DateTime.Now;
            _helperlandDBContext.ServiceRequests.Update(sr);
            _helperlandDBContext.SaveChanges();
            User u = _helperlandDBContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault();
            User u1 = _helperlandDBContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault();
            if (sr.ServiceProviderId > 0)
            {

                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Cancelled By Admin";
                mail.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from service provider.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
                MailMessage mail1 = new MailMessage();
                mail1.To.Add(u1.Email);
                mail1.From = new MailAddress("helperland12@gmail.com");
                mail1.Subject = "Cancelled By Admin";
                mail1.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from service provider.";
                mail1.IsBodyHtml = true;
                SmtpClient smtp1 = new SmtpClient("smtp.gmail.com", 587);
                smtp1.EnableSsl = true;
                smtp1.UseDefaultCredentials = false;
                smtp1.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp1.Send(mail1);
                smtp1.Dispose();

            }
            else
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(u.Email);
                mail.From = new MailAddress("helperland12@gmail.com");
                mail.Subject = "Cancelled By Admin";
                mail.Body = "Service Request is " + sr.ServiceId + " is cancelled by admin from service provider.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                smtp.Send(mail);
                smtp.Dispose();
            }
            return RedirectToAction("ServiceRequestAdmin");
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
                csmodel.favoriteAndBlockeds1 = from fb in _helperlandDBContext.FavoriteAndBlockeds 
                                              where fb.TargetUserId == HttpContext.Session.GetInt32("CustomerId") select fb;

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
        public IActionResult SPsettings()
        {
            if (HttpContext.Session.GetInt32("SPId") != null)
            {
                var userinfo = (from User in _helperlandDBContext.Users
                                where User.UserId == HttpContext.Session.GetInt32("SPId")
                                select new
                                {
                                    User.FirstName,
                                    User.LastName,
                                    User.Mobile,
                                    User.DateOfBirth,
                                    User.Email,
                                    User.NationalityId,
                                    User.Gender,
                                    User.UserProfilePicture,
                                   
                                }).ToList();
                var useradd = (from UserAddress in _helperlandDBContext.UserAddresses
                               where UserAddress.UserId == HttpContext.Session.GetInt32("SPId")
                               select new
                               {
                                   UserAddress.AddressLine1,
                                   UserAddress.AddressLine2,
                                   UserAddress.City,
                                   UserAddress.PostalCode,
                               }).ToList();
                if (userinfo.FirstOrDefault() != null)
                {
                    ViewBag.sfName = userinfo.FirstOrDefault().FirstName;
                    ViewBag.slName = userinfo.FirstOrDefault().LastName;
                    ViewBag.sMobile = userinfo.FirstOrDefault().Mobile;
                    
                   
                    ViewBag.sDOB = userinfo.FirstOrDefault().DateOfBirth;
                    ViewBag.sEmail = userinfo.FirstOrDefault().Email;
                    ViewBag.sNationalityId = userinfo.FirstOrDefault().NationalityId;
                    ViewBag.sGender = userinfo.FirstOrDefault().Gender;
                    ViewBag.sUserProfilePicture = userinfo.FirstOrDefault().UserProfilePicture;
                    ViewBag.sUserProfilePicture1 = userinfo.FirstOrDefault().UserProfilePicture;
                    if (useradd.FirstOrDefault() != null)
                    {
                        ViewBag.sAddressLine1 = useradd.FirstOrDefault().AddressLine1;
                        ViewBag.sAddressLine2 = useradd.FirstOrDefault().AddressLine2;
                        ViewBag.sCity = useradd.FirstOrDefault().City;
                        ViewBag.SPostalCode = useradd.FirstOrDefault().PostalCode;
                        return View();
                    }
                    return View();
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult changespPassword(ServiceProviderSideModel model)
        {
            User u = _helperlandDBContext.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("SPId")).FirstOrDefault();
            if (u.Password == model.oldpassword)
            {
                u.Password = model.Password.ToString();
                _helperlandDBContext.Users.Update(u);
                _helperlandDBContext.SaveChanges();
                TempData["changepass"] = "Bill";
                return RedirectToAction("SPsettings");
            }
            else
            {
                TempData["errorchangepass"] = "Bill";
                return RedirectToAction("SPsettings");
            }
        }
        [HttpPost]
        public IActionResult spPersonalInfo(ServiceProviderSideModel model)
        {
            User u = _helperlandDBContext.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("SPId")).FirstOrDefault();
            u.FirstName = model.fname;
            u.LastName = model.lname;
            u.Mobile = model.phone;
            u.ModifiedDate = DateTime.Now;
            if (model.day != null && model.month != null && model.year != null)
            {
                u.DateOfBirth = DateTime.Parse(model.day + "-" + model.month + "-" + model.year);
            }
            if (model.natid != null)
            {
                u.NationalityId = int.Parse(model.natid);
            }
            
            u.Gender = model.gender;
            u.UserProfilePicture = model.profiledp;
            u.ZipCode = model.postal;

            _helperlandDBContext.Users.Update(u);
            _helperlandDBContext.SaveChanges();
            HttpContext.Session.SetString("SPfname", model.fname);
            UserAddress add = _helperlandDBContext.UserAddresses.Where(x => x.UserId == HttpContext.Session.GetInt32("SPId")).FirstOrDefault();
            if (add != null)
            {
                add.AddressLine1 = model.add2;
                add.AddressLine2 = model.add1;
                add.PostalCode = model.postal;
                add.City = model.city;
                _helperlandDBContext.UserAddresses.Update(add);
                _helperlandDBContext.SaveChanges();
            }
            else
            {
                UserAddress add1 = new UserAddress();
                add1.UserId = (int)HttpContext.Session.GetInt32("SPId");
                add1.AddressLine1 = model.add2;
                add1.AddressLine2 = model.add1;
                add1.PostalCode = model.postal;
                add1.City = model.city;
                add1.IsDefault = false;
                add1.IsDeleted = false;
                _helperlandDBContext.UserAddresses.Add(add1);
                _helperlandDBContext.SaveChanges();
            }
            TempData["personaldetails"] = "Bill";
            return RedirectToAction("SPsettings");
            
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
                Rating r = _helperlandDBContext.Ratings.Where(x => x.ServiceRequestId == model.rate.ServiceRequestId).FirstOrDefault();
                if (r != null)
                {
                    var rateavg = (decimal.Parse(model.friendly) + decimal.Parse(model.ontime) + decimal.Parse(model.quality)) / 3;
                    r.RatingFrom = model.rate.RatingFrom;
                    r.RatingTo = model.rate.RatingTo;
                    r.Ratings = rateavg;
                    r.Comments = model.rate.Comments;
                    r.RatingDate = DateTime.Now;
                    r.OnTimeArrival = decimal.Parse(model.ontime);
                    r.Friendly = decimal.Parse(model.friendly);
                    r.QualityOfService = decimal.Parse(model.quality);
                    _helperlandDBContext.Ratings.Update(r);
                    _helperlandDBContext.SaveChanges();
                    return RedirectToAction("ServiceHistory");
                }
                else
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
               
            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }
        public IActionResult RatingForSp()
        {
            if (HttpContext.Session.GetInt32("SPId") != null)
            {

                ServiceProviderSideModel sp = new ServiceProviderSideModel();
                sp.serviceRequests = from serviceRequests in _helperlandDBContext.ServiceRequests
                                     where serviceRequests.Status == 3 && serviceRequests.ServiceProviderId == HttpContext.Session.GetInt32("SPId")
                                     select serviceRequests;
                sp.ratings = from Rating in _helperlandDBContext.Ratings
                             where Rating.RatingTo == HttpContext.Session.GetInt32("SPId")
                             select Rating;
                sp.users = from User in _helperlandDBContext.Users
                           where User.UserTypeId == 1
                           select User;
                return View(sp);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        public IActionResult blockcustomer()
        {
            if (HttpContext.Session.GetInt32("SPId") != null)
            {

                ServiceProviderSideModel sp = new ServiceProviderSideModel();
                sp.serviceRequests = from serviceRequests in _helperlandDBContext.ServiceRequests
                                     where serviceRequests.Status == 3 && serviceRequests.ServiceProviderId == HttpContext.Session.GetInt32("SPId")
                                     select serviceRequests;

                sp.users = from User in _helperlandDBContext.Users
                           where User.UserTypeId == 1
                           select User;
                sp.favoriteAndBlockeds = from favoriteAndBlockeds in _helperlandDBContext.FavoriteAndBlockeds
                                         where favoriteAndBlockeds.UserId == HttpContext.Session.GetInt32("SPId")
                                         select favoriteAndBlockeds;
                return View(sp);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult blockcust(ServiceProviderSideModel model)
        {
            FavoriteAndBlocked fb = _helperlandDBContext.FavoriteAndBlockeds.Where(x => x.UserId == model.spid && x.TargetUserId == model.userid).FirstOrDefault();
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
                fb1.UserId = model.spid;
                fb1.TargetUserId = model.userid;
                fb1.IsFavorite = false;
                fb1.IsBlocked = true;
                _helperlandDBContext.FavoriteAndBlockeds.Add(fb1);
                _helperlandDBContext.SaveChanges();
            }

            return RedirectToAction("blockcustomer");
        }

        [HttpPost]
        public IActionResult unblockcust(ServiceProviderSideModel model)
        {
            FavoriteAndBlocked fb = _helperlandDBContext.FavoriteAndBlockeds.Where(x => x.UserId == model.spid && x.TargetUserId == model.userid).FirstOrDefault();
            if (fb != null)
            {

                fb.IsFavorite = false;
                fb.IsBlocked = false;
                _helperlandDBContext.FavoriteAndBlockeds.Update(fb);
                _helperlandDBContext.SaveChanges();
            }
            return RedirectToAction("blockcustomer");

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
            
            var spinfo = from u in _helperlandDBContext.Users
                         where u.ZipCode == sr.ZipCode && u.UserTypeId == 2
                         select u;
            if (spinfo != null)
            {
                foreach (var email in spinfo)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(email.Email);
                    mail.From = new MailAddress("helperland12@gmail.com");
                    mail.Subject = "New Service request is available";
                    mail.Body = "New Service Request " + sr.ServiceId + " is available now!";
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("helperland12@gmail.com", "helperlandlogin12");
                    smtp.Send(mail);
                    smtp.Dispose();
                }
            }
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
