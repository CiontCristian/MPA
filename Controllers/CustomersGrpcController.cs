using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcCustomersService;
using LibraryModel.Models;
using System.Net.Http;

namespace Ciont_Cristian_Lab2.Controllers
{
    public class CustomersGrpcController : Controller
    {

        private readonly GrpcChannel channel;
        public CustomersGrpcController()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);
            channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions() { HttpClient = httpClient });
            //channel = GrpcChannel.ForAddress("https://localhost:5001");
        }
        [HttpGet]
        public IActionResult Index()
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            CustomerList cust = client.GetAll(new Empty());
            return View(cust);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GrpcCustomersService.Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Insert(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Get(new CustomerId() { Id = id });
                return View(createdCustomer);
            }
            return new NotFoundResult();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteGrpc([Bind("CustomerID")] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = new CustomerService.CustomerServiceClient(channel);
                   // GrpcCustomersService.CustomerId 
                    var createdCustomer = client.Delete(new CustomerId() {Id = id });
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record:{ ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Get(new CustomerId() { Id = id.Value });
                return View(createdCustomer);
            }
            return new NotFoundResult();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Get(new CustomerId() { Id = id.Value });
                return View(createdCustomer);
            }
            return new NotFoundResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Adress,Birthdate")] GrpcCustomersService.Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Update(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);

        }
    }
}
