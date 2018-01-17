﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp_ebuilder.Authorizer
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                var authorizedUsers = ConfigurationManager.AppSettings[UsersConfigKey];
                var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];

                Users = String.IsNullOrEmpty(Users) ? authorizedUsers : Users;
                Roles = String.IsNullOrEmpty(Roles) ? "HR Admin,Managerial,Non-Managerial" : Roles;
               
                if (!String.IsNullOrEmpty(Roles))
                {
                   
                    if (!CurrentUser.IsInRole(Roles))
                    {
                       filterContext.Result = new RedirectToRouteResult(new
                       RouteValueDictionary(new { controller = "Home", action = "Index" }));

                       //base.OnAuthorization(filterContext); //returns to login url
                    }
                }

                /*if (!String.IsNullOrEmpty(Users))
                {
                    if (!Users.Contains(CurrentUser.email.ToString()))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Home", action = "Index" }));

                        // base.OnAuthorization(filterContext); //returns to login url
                    }
                }*/

            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                       RouteValueDictionary(new { controller = "Home", action = "Index" }));

            }
        }
    }
}