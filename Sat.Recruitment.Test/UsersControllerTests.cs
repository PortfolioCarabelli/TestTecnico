using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Controllers;
using Sat.Recruitment.Api.Entities;
using Xunit;

namespace Sat.Recruitment.Test
{
    [CollectionDefinition("Tests", DisableParallelization = true)]
    public class UsersControllerTests
    {
        private readonly UsersController _userController;

        public UsersControllerTests()
        {
            var logger = new LoggerFactory().CreateLogger<UsersController>();
            _userController = new UsersController(logger);
        }

        [Fact]
        public async Task CreateUser_NormalUser_Success()
        {
            var result = await _userController.CreateUser("Mike", "mike@gmail.com", "Av. Juan G", "+349 1122354215", "Normal", "124");
            Assert.True(result.IsSuccess);
            Assert.Equal("User Created", result.Errors);
        }

        [Fact]
        public async Task CreateUser_DuplicateUser_Failure()
        {
            var result = await _userController.CreateUser("Agustina", "Agustina@gmail.com", "Av. Juan G", "+349 1122354215", "Normal", "124");
            Assert.False(result.IsSuccess);
            Assert.Equal("The user is duplicated", result.Errors);
        }
    }
}
