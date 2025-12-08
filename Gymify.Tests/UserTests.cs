using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Tests
{
    public class UserTests : BaseTests
    {
        [Test]
        public async Task UserCanRegisterLoginAndViewDashboard()
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var testEmail = $"user_{uniqueId}@test.com";
            var testUser = $"GymUser_{uniqueId}";
            var testPassword = "Password123!";

            await Page.GotoAsync($"{BaseUrl}/Register");

            await Page.FillAsync("#Email", testEmail);
            await Page.FillAsync("#UserName", testUser);
            await Page.FillAsync("#Password", testPassword);
            await Page.FillAsync("#ConfirmPassword", testPassword);

            await Page.ClickAsync("form[action='/register'] button[type='submit']");

            await Page.GotoAsync($"{BaseUrl}/Login");

            await Page.FillAsync("#Email", testEmail);
            await Page.FillAsync("#Password", testPassword);

            await Page.ClickAsync("form[action='/login'] button[type='submit']");

            var dashboardTitle = Page.Locator(".dashboard-title");
            await Expect(dashboardTitle).ToBeVisibleAsync();

            await Expect(Page.Locator(".level-badge")).ToBeVisibleAsync();
        }
    }
}
