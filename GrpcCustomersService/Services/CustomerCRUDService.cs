using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess = LibraryModel.Data;
using ModelAccess = LibraryModel.Models;
using Newtonsoft.Json;

namespace GrpcCustomersService
{
    public class CustomerCRUDService : CustomerService.CustomerServiceBase
    {
        private DataAccess.LibraryContext db = null;
        public CustomerCRUDService(DataAccess.LibraryContext db)
        {
            this.db = db;
        }

        public override Task<CustomerList> GetAll(Empty empty, ServerCallContext context)
        {
            CustomerList pl = new CustomerList();
            var query = from cust in db.Customers
                        select new Customer()
                        {
                            CustomerId = cust.CustomerID,
                            Name = cust.Name,
                            Adress = cust.Adress,
                            Birthdate = cust.BirthDate.ToString()
                        };
            pl.Item.AddRange(query.ToArray());
            return Task.FromResult(pl);
        }

        public override Task<Customer> Get(CustomerId requestData, ServerCallContext context)
        {
            Console.WriteLine(requestData.Id);
            var query = from cust in db.Customers
                        where cust.CustomerID == requestData.Id
                        select new Customer()
                        {
                            CustomerId = cust.CustomerID,
                            Name = cust.Name,
                            Adress = cust.Adress,
                            Birthdate = cust.BirthDate.ToString()
                        }
                        ;
    
            return Task.FromResult(query.ToArray()[0]);
        }

        public override Task<Empty> Insert(Customer requestData, ServerCallContext context)
        {
            db.Customers.Add(new ModelAccess.Customer
            {
                CustomerID = requestData.CustomerId,
                Name = requestData.Name,
                Adress = requestData.Adress,
                BirthDate = DateTime.Parse(requestData.Birthdate)
            });
            db.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Delete(CustomerId requestData, ServerCallContext context)
        {
            var query = from cust in db.Customers
                        where cust.CustomerID == requestData.Id
                        select new Customer()
                        {
                            CustomerId = cust.CustomerID,
                            Name = cust.Name,
                            Adress = cust.Adress,
                            Birthdate = cust.BirthDate.ToString()
                        }
                       ;
            Customer customerGrpc = query.ToArray()[0];
            
            db.Customers.Remove(new ModelAccess.Customer
            {
                CustomerID = customerGrpc.CustomerId,
                Name = customerGrpc.Name,
                Adress = customerGrpc.Adress,
                BirthDate = DateTime.Parse(customerGrpc.Birthdate)
            });
               

            db.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Customer> Update(Customer requestData, ServerCallContext context)
        {
            var customer = db.Customers.Update(new ModelAccess.Customer
            {
                CustomerID = requestData.CustomerId,
                Name = requestData.Name,
                Adress = requestData.Adress,
                BirthDate = DateTime.Parse(requestData.Birthdate)
            }).Entity;

            db.SaveChanges();
            
            return Task.FromResult(new Customer()
            {
                CustomerId = customer.CustomerID,
                Name = customer.Name,
                Adress = customer.Adress,
                Birthdate = customer.BirthDate.ToString()
            });
        }
    }
}
