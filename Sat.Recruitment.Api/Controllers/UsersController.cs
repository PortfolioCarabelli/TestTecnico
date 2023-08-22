using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly List<User> _users = new List<User>();
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
            LoadUsersFromFile();
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var errors = ValidateErrors(name, email, address, phone);
            if (!string.IsNullOrEmpty(errors))
                return new Result() { IsSuccess = false, Errors = errors };

            var newUser = new User
            {
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = decimal.Parse(money)
            };

            MoneyCalculos(newUser);

            if (IsUserDuplicate(newUser))
            {
                _logger.LogInformation("The user is duplicated");
                return new Result() { IsSuccess = false, Errors = "The user is duplicated" };
            }

            _users.Add(newUser);

            _logger.LogInformation("User Created");
            return new Result() { IsSuccess = true, Errors = "User Created" };
        }

        private string ValidateErrors(string name, string email, string address, string phone)
        {
            var errors = "";
            if (string.IsNullOrEmpty(name))
                errors += "The name is required. ";
            if (string.IsNullOrEmpty(email))
                errors += "The email is required. ";
            if (string.IsNullOrEmpty(address))
                errors += "The address is required. ";
            if (string.IsNullOrEmpty(phone))
                errors += "The phone is required. ";
            return errors;
        }


        private void MoneyCalculos(User user)
        {
            if (user.UserType == "Normal")
            {
                if (user.Money > 100)
                {
                    user.Money += user.Money * Convert.ToDecimal(0.12);
                }
                else if (user.Money > 10)
                {
                    user.Money += user.Money * Convert.ToDecimal(0.8);
                }
            }
            else if (user.UserType == "SuperUser")
            {
                if (user.Money > 100)
                {
                    user.Money += user.Money * Convert.ToDecimal(0.20);
                }
            }
            else if (user.UserType == "Premium")
            {
                if (user.Money > 100)
                {
                    user.Money += user.Money * 2;
                }
            }
        }

        private bool IsUserDuplicate(User newUser)
        {
            return _users.Any(user =>
                user.Email == newUser.Email ||
                user.Phone == newUser.Phone ||
                (user.Name == newUser.Name && user.Address == newUser.Address));
        }

        private void LoadUsersFromFile()
        {
            var reader = new StreamReader(Directory.GetCurrentDirectory() +  "/Files/Users.txt");
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                var userData = line.Split(',').Select(data => data.Trim()).ToArray();
                var user = new User
                {
                    Name = userData[0],
                    Email = userData[1],
                    Phone = userData[2],
                    Address = userData[3],
                    UserType = userData[4],
                    Money = decimal.Parse(userData[5])
                };
                _users.Add(user);
            }
            reader.Close();
        }
    }
}
