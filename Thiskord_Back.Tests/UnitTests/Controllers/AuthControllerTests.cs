using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Thiskord_Back.Controllers;
using Thiskord_Back.Models.Account;
using Thiskord_Back.Models.Auth;
using Thiskord_Back.Services;

namespace Thiskord_Back.Tests.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IAuthService> _mockAuthService;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public void AuthController_Authentification_ReturnsOk()
        {
            var request = new AuthRequest { user_auth = "EMRE", password = "EMRE" };
            var testUser = new UserAccount(1, "EMRE", "EMRE@EMRE.EMRE", "https://picture.com/pic.png");
            var authenticatedUser = new AuthenticatedUser(testUser, "fake-jwt-token");
            
            _mockAuthService.Setup(s => s.AuthLogin(request.user_auth, request.password)).Returns(authenticatedUser);
            
            var actionResult = _authController.authentification(request);
            
            actionResult.Should().BeOfType<OkObjectResult>();
            // Vérifie que l'utilisateur authentifié est correct
            var okResult = actionResult as OkObjectResult;
            var newlyAuthenticatedUser = okResult!.Value as AuthenticatedUser;
            authenticatedUser.Should().NotBeNull();
            authenticatedUser!.user.user_name.Should().Be("EMRE");
            authenticatedUser.token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public void AuthController_Authentification_ReturnsUnauthorized()
        {
            var request = new AuthRequest { user_auth = "usernonexistant", password = "fauxmdp" };
            
            _mockAuthService.Setup(s => s.AuthLogin(request.user_auth, request.password)).Returns((AuthenticatedUser)null);
            
            var actionResult = _authController.authentification(request);
            actionResult.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}

