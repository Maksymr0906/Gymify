using Gymify.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;

namespace Gymify.Tests
{
    // Ми створюємо цей клас лише для того, щоб задовольнити конструктор SignInManager
    public class FakeSignInManager : SignInManager<ApplicationUser>
    {
        public FakeSignInManager()
            : base(
                // 1. UserManager (теж створюємо фейковий)
                new Mock<UserManager<ApplicationUser>>(
                    new Mock<IUserStore<ApplicationUser>>().Object,
                    null, null, null, null, null, null, null, null
                ).Object,

                // 2. IHttpContextAccessor
                new Mock<IHttpContextAccessor>().Object,

                // 3. IUserClaimsPrincipalFactory
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,

                // 4. IOptions
                new Mock<IOptions<IdentityOptions>>().Object,

                // 5. ILogger
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,

                // 6. IAuthenticationSchemeProvider
                new Mock<IAuthenticationSchemeProvider>().Object
            )
        { }
    }
}