﻿namespace Epi.Web.Enter.Common.BusinessObject
{
    public class UserAuthenticationRequestBO
    {
        private string _ResponseId;
        private string _PassCode;

        public string ResponseId
        {
            get { return _ResponseId; }
            set { _ResponseId = value; }
        }

        public string PassCode
        {
            get { return _PassCode; }
            set { _PassCode = value; }
        }
    }
}