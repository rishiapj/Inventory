using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        IConfiguration _iconfiguration;
        public HomeController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }
        private IMongoDatabase mongoDatabase;

        //Generic method to get the mongodb database details  
        public IMongoDatabase GetMongoDatabase()
        {
            try
            {
                string value1 = _iconfiguration.GetSection("Data").GetSection("ConnectionString").Value;
                // Second way  
                string value2 = _iconfiguration.GetValue<string>("Data:Database");

                var mongoClient = new MongoClient(value1);
                return mongoClient.GetDatabase(value2);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Index()
        {
            try
            {
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //fetch the details from CustomerDB and pass into view  
                var result = mongoDatabase.GetCollection<Customer>("Customers").Find(FilterDefinition<Customer>.Empty).ToList();
                return View(result);
            }
            catch (Exception)
            {
                List<Customer> lst = new List<Customer>();
                return View(lst);
            }
          
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            try
            {
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //var tst = mongoDatabase.GetCollection<Customer>("Customers").SortByDescending(m => m.Id).Limit(1);
                mongoDatabase.GetCollection<Customer>("Customers").InsertOne(customer);
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //fetch the details from CustomerDB based on id and pass into view  
                var customer = mongoDatabase.GetCollection<Customer>("Customers").Find<Customer>(k => k.CustomerId == id).FirstOrDefault();
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //fetch the details from CustomerDB and pass into view  
                Customer customer = mongoDatabase.GetCollection<Customer>("Customers").Find<Customer>(k => k.CustomerId == id).FirstOrDefault();
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            try
            {
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //Build the where condition  
                var filter = Builders<Customer>.Filter.Eq("CustomerId", customer.CustomerId);
                //Build the update statement   
                var updatestatement = Builders<Customer>.Update.Set("CustomerId", customer.CustomerId);
                updatestatement = updatestatement.Set("CustomerName", customer.CustomerName);
                updatestatement = updatestatement.Set("Address", customer.Address);
                updatestatement = updatestatement.Set("Mobile", customer.Mobile);
                //fetch the details from CustomerDB based on id and pass into view  
                var result = mongoDatabase.GetCollection<Customer>("Customers").UpdateOne(filter, updatestatement);
                if (result.IsAcknowledged == false)
                {
                    return BadRequest("Unable to update Customer  " + customer.CustomerName);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Get the database connection  
            mongoDatabase = GetMongoDatabase();
            //fetch the details from CustomerDB and pass into view  
            Customer customer = mongoDatabase.GetCollection<Customer>("Customers").Find<Customer>(k => k.CustomerId == id).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            try
            {
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();
                //Delete the customer record  
                var result = mongoDatabase.GetCollection<Customer>("Customers").DeleteOne<Customer>(k => k.CustomerId == customer.CustomerId);
                if (result.IsAcknowledged == false)
                {
                    return BadRequest("Unable to Delete Customer " + customer.CustomerId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
