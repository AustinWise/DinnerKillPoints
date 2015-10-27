using Microsoft.Owin;
using Owin;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

[assembly: OwinStartupAttribute(typeof(DkpWeb.Startup))]
namespace DkpWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        //TODO: find a better place to stash this method
        public static bool IsUserInRole(string roleName)
        {
            var ctx = HttpContext.Current;
            if (!ctx.Request.IsAuthenticated)
                return false;
            var user = HttpContext.Current.User;
            if (!user.Identity.IsAuthenticated)
                return false;
            var userMan = ctx.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userMan.IsInRole(user.Identity.GetUserId(), roleName);
        }
    }
}
