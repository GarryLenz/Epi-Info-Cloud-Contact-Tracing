﻿using System;
using Epi.Cloud.Common.EntityObjects;

namespace Epi.Web.MVC.Models
{

    /// <summary>
    /// The Survey Model that will be pumped to view
    /// </summary>
    public class SurveyAnswerModel
    {
        public string ResponseId { get; set; }
        public string SurveyId { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int Status { get; set; }
        public string XML { get; set; }
        public string ParentRecordId { get; set; }
        public string RelateParentId { get; set; }
        public FormResponseDetail ResponseDetail { get; set; }

    }
}