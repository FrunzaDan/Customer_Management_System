using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService
{
    private readonly IBLLConfig _configuration;
    private readonly IDBUtils _dbUtils;

    public CustomerService()
    {
        _configuration = ServiceLocator.GetService<IBLLConfig>();
        _dbUtils = ServiceLocator.GetService<IDBUtils>();
    }

}
