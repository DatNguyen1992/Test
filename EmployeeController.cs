using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//add
using LoginWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace LoginWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private string uri = "http://localhost:53325/api/Employees/";
        private HttpClient httpClient = new HttpClient();

        public IActionResult Index(double? min, double? max)
        {
            if (HttpContext.Session.GetString("ename") == null)
            {
                return RedirectToAction("Login");
            }//Step 1 Use Session)
            else 
            { 
                var eList = JsonConvert.DeserializeObject<IEnumerable<Employee>>(httpClient.GetStringAsync(uri).Result);//Get All
                if (min==null||max==null)
                {
                    return View(eList);
                }
                else
                {
                    var newList = eList.Where(e=>e.Salary >= min && e.Salary <= max);
                    return View(newList);
                }
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string name,string pass)
        {
            try
            {
                var eList = JsonConvert.DeserializeObject<IEnumerable<Employee>>(httpClient.GetStringAsync(uri).Result);//Get All
                Employee employee = eList.SingleOrDefault(e=>e.EmployeeName.Equals(name));
                if (employee!=null)
                {
                    HttpContext.Session.SetString("ename",name);//Step 1 Use Session
                    if (employee.Password.Equals(pass))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Msg = "Invalid Password!!";
                    }
                }
                else
                {
                    ViewBag.Msg = "Invalid EmployeeName";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        public IActionResult Edit(string ename)
        {
            var eList = JsonConvert.DeserializeObject<IEnumerable<Employee>>(httpClient.GetStringAsync(uri).Result);//Get All
            var emp = eList.SingleOrDefault(e=>e.EmployeeName.Equals(ename));
            return View(emp);
        }
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            try
            {
                var model = httpClient.PutAsJsonAsync<Employee>(uri,employee).Result;
                if (model.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Msg = "Fail";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            try
            {
                var model = httpClient.PostAsJsonAsync<Employee>(uri, employee).Result;
                if (model.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                else
                    {
                        ViewBag.Msg = "Fail";
                    }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }
        public IActionResult Delete(string employeeName)
        {
            try
            {
                var model = httpClient.DeleteAsync(uri+employeeName).Result;
                if (model.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Msg = "Fail";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }
    }
}
