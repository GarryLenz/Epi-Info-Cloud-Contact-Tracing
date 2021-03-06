﻿using System;
using System.Web.Mvc;
using Epi.Cloud.Common.Constants;

namespace Epi.Cloud.MVC.Controllers
{
    public class ResponseController : BaseSurveyController
    {
        [HttpGet]
        public ActionResult Index(string surveyId, string responseId, int pageNumber = 1)
        {
            try
            {
                TempData[TempDataKeys.ResponseId] = responseId;

                return RedirectToRoute(new { Controller = "Survey", Action = "Index", surveyId = surveyId, PageNumber = pageNumber });
            }
            catch (Exception ex)
            {
                Epi.Web.Utility.ExceptionMessage.SendLogMessage(ex, this.HttpContext);

                return RedirectToAction(ViewActions.Exception);
            }
        }
    }
}
