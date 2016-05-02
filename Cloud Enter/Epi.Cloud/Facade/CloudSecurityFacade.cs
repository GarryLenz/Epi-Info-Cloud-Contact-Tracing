﻿using Epi.Web.Enter.Common.DTO;
using Epi.Web.Enter.Common.Message;
using Epi.Web.MVC.Models;
using Epi.Web.MVC.Repositories.Core;
using Epi.Web.MVC.Utility;

namespace Epi.Web.MVC.Facade
{
    public class CloudSecurityFacade : ISecurityFacade
    {
        //declare UserAuthenticationRequest
        private Epi.Web.Enter.Common.Message.UserAuthenticationRequest _surveyAuthenticationRequest;
        //declare PassCodeDTO
        private Epi.Web.Enter.Common.DTO.PassCodeDTO _PassCodeDTO;
        // declare ISurveyResponseRepository which inherits IRepository of SurveyResponseResponse object
        private ISurveyAnswerRepository _iSurveyAnswerRepository;

        public CloudSecurityFacade(UserAuthenticationRequest surveyAuthenticationRequest
                            , ISurveyAnswerRepository iSurveyResponseRepository
                            , Epi.Web.Enter.Common.DTO.PassCodeDTO PassCodeDTO)
        {
            _surveyAuthenticationRequest = surveyAuthenticationRequest;
            _iSurveyAnswerRepository = iSurveyResponseRepository;
            _PassCodeDTO = PassCodeDTO;
        }

        public UserAuthenticationResponse ValidateUser(string userName, string password)
        {
            //_surveyAuthenticationRequest.PassCode = passcode;
            //_surveyAuthenticationRequest.SurveyResponseId = responseId;
            UserDTO User = new UserDTO();
            User.UserName = userName;
            User.PasswordHash = password;
            _surveyAuthenticationRequest.User = User;

            UserAuthenticationResponse AuthenticationResponse = _iSurveyAnswerRepository.ValidateUser(_surveyAuthenticationRequest);
            return AuthenticationResponse;
        }
        public void UpdatePassCode(string ResponseId, string Passcode)
        {

            // convert DTO to  UserAuthenticationRquest
            _PassCodeDTO.ResponseId = ResponseId;
            _PassCodeDTO.PassCode = Passcode;
            UserAuthenticationRequest AuthenticationRequestObj = Mapper.ToUserAuthenticationObj(_PassCodeDTO);
            SurveyHelper.UpdatePassCode(AuthenticationRequestObj, _iSurveyAnswerRepository);

        }
        public UserAuthenticationResponse GetAuthenticationResponse(string responseId)
        {

            _surveyAuthenticationRequest.SurveyResponseId = responseId;
            UserAuthenticationResponse AuthenticationResponse = _iSurveyAnswerRepository.GetAuthenticationResponse(_surveyAuthenticationRequest);
            return AuthenticationResponse;
        }
        public UserAuthenticationResponse GetUserInfo(int UserId)
        {
            UserDTO User = new UserDTO();
            User.UserId = UserId;
            _surveyAuthenticationRequest.User = User;

            UserAuthenticationResponse AuthenticationResponse = _iSurveyAnswerRepository.GetUserInfo(_surveyAuthenticationRequest);
            return AuthenticationResponse;

        }


        public bool UpdateUser(UserDTO User)
        {
            UserAuthenticationRequest request = new UserAuthenticationRequest();
            request.User = User;
            return _iSurveyAnswerRepository.UpdateUser(request);
        }
    }
}