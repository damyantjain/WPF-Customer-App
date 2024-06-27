﻿using CustomerTestApp.Service;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace CustomerTestApp.WPF.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerManagement.CustomerManagementClient _client;

        public CustomerService(IConfiguration configuration)
        {
            var address = configuration["GrpcServer:Address"];
            var channel = GrpcChannel.ForAddress(address);
            _client = new CustomerManagement.CustomerManagementClient(channel);
        }

        public async IAsyncEnumerable<Customer> GetAllCustomers()
        {
            using var streamCall = _client.GetAllCustomers(new Empty());
            await foreach (var customer in streamCall.ResponseStream.ReadAllAsync())
            {
                yield return customer;
            }
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _client.AddCustomerAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _client.UpdateCustomerAsync(customer);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            await _client.DeleteCustomerAsync(new CustomerId { Id = customerId });
        }
    }
}
