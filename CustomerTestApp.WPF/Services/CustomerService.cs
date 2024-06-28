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
        public async Task<CustomerResponse> AddCustomerAsync(Customer customer)
        {
            try
            {
                return await _client.AddCustomerAsync(customer);
            }
            catch (RpcException ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"gRPC error: {ex.Status.Detail}" };
            }
            catch (Exception ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"An error occurred: {ex.Message}" };
            }
        }
        public async Task<CustomerResponse> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                return await _client.UpdateCustomerAsync(customer);
            }
            catch (RpcException ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"gRPC error: {ex.Status.Detail}" };
            }
            catch (Exception ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<CustomerResponse> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var request = new CustomerId { Id = customerId };
                return await _client.DeleteCustomerAsync(request);
            }
            catch (RpcException ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"gRPC error: {ex.Status.Detail}" };
            }
            catch (Exception ex)
            {
                return new CustomerResponse { Status = Service.Status.Error, Message = $"An error occurred: {ex.Message}" };
            }
        }

    }
}