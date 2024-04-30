using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Api.Controllers;
using Xunit;

namespace StrDss.Test
{
    public class ControllerActionsShouldBeAuthorized
    {
        [Fact]
        public void AllControllersActionsShouldBeAuthorized()
        {
            // Arrange
            var assembly = typeof(BaseApiController).Assembly;
            var controllerTypes = assembly.GetTypes().Where(t =>
                typeof(ControllerBase).IsAssignableFrom(t) && t.Name.EndsWith("Controller") && t.Name != "NetworkCheckerController");

            var unauthorizedMethods = controllerTypes 
                .SelectMany(controllerType =>
                {
                    var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    return methods
                        .Where(m => !m.GetCustomAttributes(typeof(ApiAuthorizeAttribute), false).Any())
                        .Select(m => $"{controllerType.Name}.{m.Name}");
                })
                .ToList();

            // Assert
            if (unauthorizedMethods.Any())
            {
                var message = $"The following controller actions do not have the [ApiAuthorize] attribute:\n{string.Join("\n", unauthorizedMethods)}";
                throw new Exception(message);
            }
        }
    }
}
