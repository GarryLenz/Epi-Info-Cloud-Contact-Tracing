﻿using System.Collections.Generic;
using Epi.Cloud.DataEntryServices.Model;
using Epi.Web.Enter.Common.DTO;

namespace Epi.Web.MVC.Models
{
    /// <summary>
    /// Maps DTO object to Model object or Model object to DTO object
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Maps FormInfo DTO to FormInfo Model
        /// </summary>
        /// <param name="FormInfoDTO"></param>
        /// <returns></returns>
        public static FormInfoModel ToFormInfoModel(this FormInfoDTO FormInfoDTO)
        {
            return new FormInfoModel
            {
                IsSQLProject = FormInfoDTO.IsSQLProject,
                FormId = FormInfoDTO.FormId,
                FormName = FormInfoDTO.FormName,
                FormNumber = FormInfoDTO.FormNumber,
                IsDraftMode = FormInfoDTO.IsDraftMode,
                OrganizationId = FormInfoDTO.OrganizationId,
                OrganizationName = FormInfoDTO.OrganizationName,
                UserId = FormInfoDTO.UserId,
                IsOwner = FormInfoDTO.IsOwner,
                OwnerFName = FormInfoDTO.OwnerFName,
                OwnerLName = FormInfoDTO.OwnerLName,
                IsShareable = FormInfoDTO.IsShareable,
                IsShared = FormInfoDTO.IsShared,
                DataAccessRuleId = FormInfoDTO.DataAccessRuleId,
                EwavLiteToggleSwitch = FormInfoDTO.EwavLiteToggleSwitch
            };
        }

        /// <summary>
        /// Maps SurveyInfo DTO to SurveyInfo Model.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static SurveyInfoModel ToSurveyInfoModel(this SurveyInfoDTO SurveyInfoDTO)
        {
            return new SurveyInfoModel
            {
                SurveyId = SurveyInfoDTO.SurveyId,
                SurveyNumber = SurveyInfoDTO.SurveyNumber,
                SurveyName = SurveyInfoDTO.SurveyName,
                OrganizationName = SurveyInfoDTO.OrganizationName,
                DepartmentName = SurveyInfoDTO.DepartmentName,
                IntroductionText = SurveyInfoDTO.IntroductionText,
                ExitText = SurveyInfoDTO.ExitText,
                IsSuccess = SurveyInfoDTO.IsSuccess,
                SurveyType = SurveyInfoDTO.SurveyType,
                ClosingDate = SurveyInfoDTO.ClosingDate,
                UserPublishKey = SurveyInfoDTO.UserPublishKey,
                IsDraftMode = SurveyInfoDTO.IsDraftMode,
                StartDate = SurveyInfoDTO.StartDate,
                IsSqlProject = SurveyInfoDTO.IsSqlProject,
                FormOwnerId = SurveyInfoDTO.OwnerId,
            };
        }

        /// <summary>
        /// Maps SurveyInfo Model to SurveyInfo DTO.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>

        public static SurveyInfoDTO ToSurveyInfoDTO(SurveyInfoModel SurveyInfoModel)
        {
            return new SurveyInfoDTO
            {
                SurveyId = SurveyInfoModel.SurveyId,
                SurveyNumber = SurveyInfoModel.SurveyNumber,
                SurveyName = SurveyInfoModel.SurveyName,
                OrganizationName = SurveyInfoModel.OrganizationName,
                DepartmentName = SurveyInfoModel.DepartmentName,
                IntroductionText = SurveyInfoModel.IntroductionText,
                ExitText = SurveyInfoModel.ExitText,
                IsSuccess = SurveyInfoModel.IsSuccess,
                ClosingDate = SurveyInfoModel.ClosingDate,
                UserPublishKey = SurveyInfoModel.UserPublishKey,
                IsDraftMode = SurveyInfoModel.IsDraftMode,
                StartDate = SurveyInfoModel.StartDate,
            };
        }


        public static SurveyAnswerModel ToSurveyAnswerModel(this SurveyAnswerDTO surveyAnswerDTO)
        {
            return new SurveyAnswerModel
            {
                ResponseId = surveyAnswerDTO.ResponseId,
                SurveyId = surveyAnswerDTO.SurveyId,
                DateUpdated = surveyAnswerDTO.DateUpdated,
                DateCompleted = surveyAnswerDTO.DateCompleted,
                Status = surveyAnswerDTO.Status,
				
                ResponseDetail = surveyAnswerDTO.ResponseDetail,
                XML = surveyAnswerDTO.XML
            };
        }

        /// <summary>
        /// Maps SurveyInfo Model to SurveyInfo DTO.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>

        public static SurveyAnswerDTO ToSurveyAnswerDTO(SurveyAnswerModel SurveyAnswerModel)
        {
			return new SurveyAnswerDTO
			{
				ResponseId = SurveyAnswerModel.ResponseId,
				SurveyId = SurveyAnswerModel.SurveyId,
				DateUpdated = SurveyAnswerModel.DateUpdated,
				DateCompleted = SurveyAnswerModel.DateCompleted,
				Status = SurveyAnswerModel.Status,
				RelateParentId = SurveyAnswerModel.RelateParentId,
                XML = SurveyAnswerModel.XML
            };
        }

        public static Epi.Web.Enter.Common.Message.UserAuthenticationRequest ToUserAuthenticationObj(Epi.Cloud.Common.DTO.PassCodeDTO pDTO)
        {
            return new Epi.Web.Enter.Common.Message.UserAuthenticationRequest
            {
                SurveyResponseId = pDTO.ResponseId,
                PassCode = pDTO.PassCode

            };
        }


        internal static List<RelateModel> ToRelateModel(List<Enter.Common.DTO.FormsHierarchyDTO> FormsHierarchy, string FormId)
        {
            List<RelateModel> List = new List<RelateModel>();

            // Common.DTO.FormsHierarchyDTO FormsHierarchyDTO = FormsHierarchy.Single(X => X.FormId == FormId);
            foreach (var Obj in FormsHierarchy)
            {
                RelateModel RelateModel = new RelateModel();
                RelateModel.RootFormId = Obj.RootFormId;
                RelateModel.FormId = Obj.FormId;
                RelateModel.ViewId = Obj.ViewId;
                RelateModel.IsSqlProject = Obj.IsSqlProject;
                RelateModel.IsRoot = Obj.IsRoot;
                RelateModel.ResponseIds = Mapper.ToSurveyAnswerModel(Obj.ResponseIds);
                List.Add(RelateModel);
            }
            return List;
        }

        internal static List<SurveyAnswerModel> ToSurveyAnswerModel(List<Enter.Common.DTO.SurveyAnswerDTO> list)
        {
            List<SurveyAnswerModel> ModelList = new List<SurveyAnswerModel>();
            foreach (var Obj in list)
            {
                SurveyAnswerModel SurveyAnswerModel = new Models.SurveyAnswerModel();
                SurveyAnswerModel.ResponseId = Obj.ResponseId;
                SurveyAnswerModel.SurveyId = Obj.SurveyId;
                SurveyAnswerModel.DateUpdated = Obj.DateUpdated;
                SurveyAnswerModel.DateCompleted = Obj.DateCompleted;
                SurveyAnswerModel.Status = Obj.Status;
				SurveyAnswerModel.ResponseDetail = Obj.ResponseDetail;
                SurveyAnswerModel.XML = Obj.XML;
                SurveyAnswerModel.ParentRecordId = Obj.ParentRecordId;
                SurveyAnswerModel.RelateParentId = Obj.RelateParentId;
                ModelList.Add(SurveyAnswerModel);
            }
            return ModelList;
        }

        internal static OrganizationModel ToOrganizationModel(Enter.Common.DTO.OrganizationDTO DTO)
        {
            OrganizationModel ModelList = new OrganizationModel();

            ModelList.IsEnabled = DTO.IsEnabled;
            ModelList.Organization = DTO.Organization;
            ModelList.OrganizationId = DTO.OrganizationId;
            ModelList.OrganizationKey = DTO.OrganizationKey;
            return ModelList;
        }

        internal static List<OrganizationModel> ToOrganizationModelList(List<Enter.Common.DTO.OrganizationDTO> list)
        {
            List<OrganizationModel> ModelList = new List<OrganizationModel>();

            foreach (var item in list)
            {
                ModelList.Add(Mapper.ToOrganizationModel(item));
            }

            return ModelList;
        }

        internal static OrgAdminInfoModel ToOrgAdminInfoModel(Enter.Common.Message.OrganizationResponse Organizations)
        {
            OrgAdminInfoModel OrgAdminInfoModel = new OrgAdminInfoModel();
            OrgAdminInfoModel.OrgName = Organizations.OrganizationList[0].Organization;
            OrgAdminInfoModel.IsOrgEnabled = Organizations.OrganizationList[0].IsEnabled;

            return OrgAdminInfoModel;
        }

        internal static List<UserModel> ToUserModelList(List<Enter.Common.DTO.UserDTO> UserList)
        {
            List<UserModel> UserModel = new List<UserModel>();
            foreach (var user in UserList)
            {
                UserModel.Add(Mapper.ToUserModel(user));

            }
            return UserModel;
        }

        public static UserModel ToUserModel(Enter.Common.DTO.UserDTO user)
        {
            UserModel UserModel = new UserModel();
            UserModel.Email = user.EmailAddress;
            UserModel.FirstName = user.FirstName;
            UserModel.LastName = user.LastName;
            UserModel.Role = GetUserRole(user.Role);
            UserModel.IsActive = user.IsActive;
            UserModel.UserId = user.UserId;
            return UserModel;
        }
        public static UserModel ToUserModelR(Enter.Common.DTO.UserDTO user)
        {
            UserModel UserModel = new UserModel();
            UserModel.Email = user.EmailAddress;
            UserModel.FirstName = user.FirstName;
            UserModel.LastName = user.LastName;
            UserModel.Role = user.Role.ToString();
            UserModel.IsActive = user.IsActive;
            UserModel.UserId = user.UserId;
            return UserModel;
        }
        private static string GetUserRole(int p)
        {
            string Role = "";
            switch (p)
            {
                case 1:
                    Role = "User";
                    break;

                case 2:
                    Role = "Organization Admin";
                    break;

                case 3:
                    Role = "Admin";
                    break;

                default:
                    break;
            }
            return Role;
        }



        internal static Enter.Common.DTO.UserDTO ToUserDTO(UserModel UserModel)
        {
            Enter.Common.DTO.UserDTO UserDTO = new Enter.Common.DTO.UserDTO();

            UserDTO.EmailAddress = UserModel.Email;
            UserDTO.UserId = UserModel.UserId;
            UserDTO.FirstName = UserModel.FirstName;
            UserDTO.LastName = UserModel.LastName;
            UserDTO.Role = int.Parse(UserModel.Role);
            UserDTO.IsActive = UserModel.IsActive;
            if (!string.IsNullOrEmpty(UserModel.PhoneNumber))
            {
                UserDTO.PhoneNumber = UserModel.PhoneNumber;
            }
            else
            {
                UserDTO.PhoneNumber = "123456789";
            }
            return UserDTO;
        }

        internal static SurveyInfoModel ToFormInfoModel(Enter.Common.DTO.SurveyInfoDTO SurveyInfoDTO)
        {
            return new SurveyInfoModel
            {
                SurveyId = SurveyInfoDTO.SurveyId,
                SurveyNumber = SurveyInfoDTO.SurveyNumber,
                SurveyName = SurveyInfoDTO.SurveyName,
                OrganizationName = SurveyInfoDTO.OrganizationName,
                DepartmentName = SurveyInfoDTO.DepartmentName,
                IntroductionText = SurveyInfoDTO.IntroductionText,
                ExitText = SurveyInfoDTO.ExitText,
                IsSuccess = SurveyInfoDTO.IsSuccess,
                SurveyType = SurveyInfoDTO.SurveyType,
                ClosingDate = SurveyInfoDTO.ClosingDate,
                UserPublishKey = SurveyInfoDTO.UserPublishKey,
                IsDraftMode = SurveyInfoDTO.IsDraftMode,
                StartDate = SurveyInfoDTO.StartDate,
                IsSqlProject = SurveyInfoDTO.IsSqlProject,
                FormOwnerId = SurveyInfoDTO.OwnerId,
            };
        }
    }
}