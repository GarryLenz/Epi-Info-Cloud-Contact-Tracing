﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Epi.Cloud.MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //----------------------------------------------------------------------------------------------------------------------

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //----------------------------------------------------------------------------------------------------------------------

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //----------------------------------------------------------------------------------------------------------------------

            routes.IgnoreRoute
            (
                "{*staticfile}",
                new { staticfile = @".*\.(jpg|gif|jpeg|png|js|css|htm|html|htc|php)$" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            //routes.MapRoute
            //(
            //   "ListForms", // Route name
            //    "Home/ListForms", // URL with parameters
            //    new { controller = "Home", action = "ListForms" }
            //); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/ReadSortedResponseInfo", ///{formid}/{page}/{sort}/{sortfield} URL with parameters
                new { controller = "Home", action = "ReadSortedResponseInfo", formid = UrlParameter.Optional, page = UrlParameter.Optional, sort = UrlParameter.Optional, sortfield = UrlParameter.Optional, orgid = UrlParameter.Optional, reset = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/ReadResponseInfo", // URL with parameters
                new { controller = "Home", action = "ReadResponseInfo" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/GetSettings", // URL with parameters
                new { controller = "Home", action = "GetSettings", formid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/SaveSettings", // URL with parameters
                new { controller = "Home", action = "SaveSettings", formid = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Edit", // URL with parameters
                new { controller = "Home", action = "Edit", ResId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
              (
                  null, // Route name
                  "Home/LogOut", // URL with parameters
                  new { controller = "Home", action = "LogOut", ResId = UrlParameter.Optional }
              );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Delete/{ResponseId}", // URL with parameters
                new { controller = "Home", action = "Delete", ResponseId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/CheckForConcurrency", // URL with parameters
                new { controller = "Home", action = "CheckForConcurrency", responseid = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/GotoMetaadmin", // URL with parameters
                new { controller = "Home", action = "GotoMetaadmin" }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "MetadataAdmin/Index", ///{formid}/{page}/{sort}/{sortfield} URL with parameters
                new { controller = "MetadataAdmin", action = "Index", Id = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "MetadataAdmin/ClearMetadaCache", // URL with parameters
                new { controller = "MetadataAdmin", action = "ClearMetadaCache" }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "MetadataAdmin/DeleteBlob", // URL with parameters
                new { controller = "MetadataAdmin", action = "DeleteBlob", ProjectId = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "MetadataAdmin/ViewBlob", // URL with parameters
                new { controller = "MetadataAdmin", action = "ViewBlob", ProjectId = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "MetadataAdmin/UploadBlob", // URL with parameters
                new { controller = "MetadataAdmin", action = "UploadBlob", ProjectId = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Notify", // URL with parameters
                new { controller = "Home", action = "Notify", responseid = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Unlock", // URL with parameters
                new { controller = "Home", action = "Unlock", responseid = UrlParameter.Optional, RecoverLastRecordVersion = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/Unlock/{ResponseId}/{RecoverLastRecordVersion}", // URL with parameters
                new { controller = "FormResponse", action = "Unlock", ResponseId = UrlParameter.Optional, RecoverLastRecordVersion = UrlParameter.Optional }
            ); // Param

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/CheckForConcurrency", // URL with parameters
                new { controller = "FormResponse", action = "CheckForConcurrency" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/Delete/{ResponseId}", // URL with parameters
                new { controller = "FormResponse", action = "Delete", ResponseId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            /*routes.MapRoute
            (
                null, // Route name
                "FormResponse/CheckForConcurrency", // URL with parameters
                new { controller = "FormResponse", action = "CheckForConcurrency", ResponseId = UrlParameter.Optional }
            ); // Param*/

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/DeleteBranch/{ResponseId}", // URL with parameters
                new { controller = "FormResponse", action = "DeleteBranch", ResponseId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/{surveyid}/{orgid}", // URL with parameters
                new { controller = "Home", action = "Index", surveyid = UrlParameter.Optional, orgid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Index/{surveyid}/{AddNewFormId}", // URL with parameters
                new { controller = "Home", action = "Index", surveyid = UrlParameter.Optional, AddNewFormId = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Home/Index/{surveyid}/{EditForm}", // URL with parameters
                new { controller = "Home", action = "Index", surveyid = UrlParameter.Optional, EditForm = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/LogOut", // URL with parameters
                new { controller = "FormResponse", action = "LogOut", ResId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/ReadResponseInfo", // URL with parameters
                new { controller = "FormResponse", action = "ReadResponseInfo", ResId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/{formid}/{responseid}", // URL with parameters
                new { controller = "FormResponse", action = "Index", formid = UrlParameter.Optional, responseid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "FormResponse/{formid}", // URL with parameters
                new { controller = "FormResponse", action = "Index", formid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null,                                              // Route name
                "EIWST/Notify/{id}",                           // URL with parameters
                new { controller = "EIWST", action = "Notify", id = "" }
            );  // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "EIWST/DataService/{surveyid}", // URL with parameters
                new { controller = "EIWST", action = "Index", surveyid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            //     routes.MapRoute
            //(
            //    null, // Route name
            //    "EIWST/ManagerService", // URL with parameters
            //    new { controller = "EIWST", action = "TestManagerService" }
            //); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null,                                              // Route name
                "Survey/UpdateResponse/{id}",                           // URL with parameters
                new { controller = "Survey", action = "UpdateResponse", id = "" }
            );  // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null,                                              // Route name
                "Survey/SaveSurvey/{id}",                           // URL with parameters
                new { controller = "Survey", action = "SaveSurvey", id = "" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
               (
                   null, // Route name
                   "Survey/Delete/{responseid}", // URL with parameters
                   new { controller = "Survey", action = "Delete", responseid = UrlParameter.Optional, PageNumber = UrlParameter.Optional }
               );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/DeleteBranch/{responseid}", // URL with parameters
                new { controller = "Survey", action = "DeleteBranch", responseid = UrlParameter.Optional, PageNumber = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/AddChild", // URL with parameters
                new { controller = "Survey", action = "AddChild" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/HasResponse", // URL with parameters
                new { controller = "Survey", action = "HasResponse" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/UpdateGrid", // URL with parameters
                new { controller = "Survey", action = "UpdateGrid" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/ReadResponseInfo", // URL with parameters
                new { controller = "Survey", action = "ReadResponseInfo" }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/LogOut", // URL with parameters
                new { controller = "Survey", action = "LogOut", ResId = UrlParameter.Optional }
            );

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Survey/{responseid}/{PageNumber}", // URL with parameters
                new { controller = "Survey", action = "Index", responseid = UrlParameter.Optional, PageNumber = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Login/ForgotPassword", // URL with parameters
                new { controller = "Login", action = "ForgotPassword" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Login/ResetPassword", // URL with parameters
                new { controller = "Login", action = "ResetPassword" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Login/Index", // URL with parameters
                new { controller = "Login", action = "Index" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Login", // URL with parameters
                new { controller = "Login", action = "Index" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "Final/{surveyid}", // URL with parameters
                new { controller = "Final", action = "Index", surveyid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null,                                              // Route name
                "Post/Notify/{id}",                           // URL with parameters
                new { controller = "Post", action = "Notify", id = "" }
            );  // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null,                                              // Route name
                "Post/SignOut/{id}",                           // URL with parameters
                new { controller = "Post", action = "SignOut", id = "" }
            );  // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminOrganization/AutoComplete", // URL with parameters
                new { controller = "AdminOrganization", action = "AutoComplete" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminOrganization/GetUserInfoAD", // URL with parameters
                new { controller = "AdminOrganization", action = "GetUserInfoAD", email = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminOrganization/OrgList", // URL with parameters
                new { controller = "AdminOrganization", action = "OrgList" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminOrganization/{orgkey}/{iseditmode}", // URL with parameters
                new { controller = "AdminOrganization", action = "OrgInfo", orgkey = UrlParameter.Optional, iseditmode = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminOrganization/Cancel", // URL with parameters
                new { controller = "AdminOrganization", action = "Cancel" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminUser/GetUserInfoAD", // URL with parameters
                new { controller = "AdminUser", action = "GetUserInfoAD", email = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminUser/GetUserList", // URL with parameters
                new { controller = "AdminUser", action = "GetUserList", orgid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminUser/UserList", // URL with parameters
                new { controller = "AdminUser", action = "UserList" }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
                null, // Route name
                "AdminUser/{userid}/{iseditmode}/{orgid}", // URL with parameters
                new { controller = "AdminUser", action = "UserInfo", userid = UrlParameter.Optional, iseditmode = UrlParameter.Optional, orgid = UrlParameter.Optional }
            ); // Parameter defaults

            //----------------------------------------------------------------------------------------------------------------------

            routes.MapRoute
            (
               "Default", // Route name
               "{*url}", // URL with parameters
               new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
