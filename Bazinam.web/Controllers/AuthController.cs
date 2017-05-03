using Bazinam.DomainClasses;
using Bazinam.ServiceLayer.Contracts;
using Bazinam.ViewModel;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Bazinam.web.Controllers
{
    public partial class AuthController : Controller
    {
        public const string authCookieName = "ApplicationCookie";
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;

        public AuthController(IApplicationUserManager userManager, IApplicationSignInManager applicationSignInManager,
            IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _signInManager = applicationSignInManager;
            _authenticationManager = authenticationManager;
        }
        // GET: Auth
        public virtual ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult LogIn(string returnUrl)
        {
            var model = new LoginVM()
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = SignInStatus.Failure;

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: true);
            }

            switch (result)
            {
                case SignInStatus.Success:
                    return redirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                default:
                    ModelState.AddModelError("", "ورود به سیستم موفقیت آمیز نبود");
                    return View(model);
            }
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }
        public virtual ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut(authCookieName);
            return RedirectToAction("index", "home");
        }
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Home");
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private ActionResult redirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("");// MVC.AdminPanel.Dashboard.ActionNames.Index, MVC.AdminPanel.Dashboard.Name, new { area = MVC.Admin.Name });
        }
    }
}