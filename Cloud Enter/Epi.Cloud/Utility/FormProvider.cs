﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Epi.Cloud.FormMetadataServices;
using Epi.Core.EnterInterpreter;
using Epi.Web.Enter.Common.DTO;
using MvcDynamicForms;
using MvcDynamicForms.Fields;
using Epi.Cloud.SqlServer;
using System.Data;
using Epi.Web.MVC.Facade;

namespace Epi.Web.MVC.Utility
{
    public static class FormProvider
    {
        private static ISurveyStoreDocumentDBFacade _isurveyDocumentDBStoreFacade;

        [ThreadStatic]
        public static List<SurveyAnswerDTO> SurveyAnswerList;

        [ThreadStatic]
        public static List<SurveyInfoDTO> SurveyInfoList;
        public static Form GetForm(object surveyMetaData, int pageNumber, Epi.Web.Enter.Common.DTO.SurveyAnswerDTO surveyAnswer)
        {
            return GetForm(surveyMetaData, pageNumber, surveyAnswer, SurveyAnswerList, SurveyInfoList);
        }

        public static Form GetForm(object surveyMetaData, int pageNumber, Epi.Web.Enter.Common.DTO.SurveyAnswerDTO surveyAnswer, List<SurveyAnswerDTO> surveyAnswerList, List<SurveyInfoDTO> surveyInfoList)
        {
            // Save last values for subsequent calls from ValidateAll in SurveyController
            SurveyAnswerList = surveyAnswerList;
            SurveyInfoList = surveyInfoList;

            var metadataProvider = new MetadataProvider();
            var metadata = metadataProvider.GetMeta(pageNumber);

            string SurveyAnswer;

            if (surveyAnswer != null)
            {
                SurveyAnswer = surveyAnswer.XML;
            }
            else { SurveyAnswer = ""; }

            var form = new Form();

            form.ResponseId = surveyAnswer.ResponseId;

            form.SurveyInfo = (Epi.Web.Enter.Common.DTO.SurveyInfoDTO)surveyMetaData;
            //Watermark 
            if (form.SurveyInfo.IsDraftMode)
            {
                form.IsDraftModeStyleClass = "draft";
            }

            string XML = form.SurveyInfo.XML;

            form.CurrentPage = pageNumber;
            if (string.IsNullOrEmpty(XML))
            {
                form.NumberOfPages = 1;
            }
            else
            {
                form.NumberOfPages = GetNumberOfPages(XDocument.Parse(XML));
            }
            if (string.IsNullOrEmpty(XML))
            {
                // no XML what to do?
            }
            else
            {
                XDocument xdoc = XDocument.Parse(XML);

                var _FieldsTypeIDs = from _FieldTypeID in
                                         xdoc.Descendants("Field")
                                         //where _FieldTypeID.Attribute("Position").Value == (PageNumber - 1).ToString()
                                     select _FieldTypeID;

                double _Width, _Height;
                _Width = GetWidth(xdoc);
                _Height = GetHeight(xdoc);
                form.PageId = GetPageId(xdoc, pageNumber);
                form.Width = _Width;
                form.Height = _Height;
                //Add checkcode to Form
                XElement ViewElement = xdoc.XPathSelectElement("Template/Project/View");
                string checkcode = ViewElement.Attribute("CheckCode").Value.ToString();
                StringBuilder JavaScript = new StringBuilder();
                StringBuilder VariableDefinitions = new StringBuilder();
                string defineFormat = "cce_Context.define(\"{0}\", \"{1}\", \"{2}\", \"{3}\");";
                string defineNumberFormat = "cce_Context.define(\"{0}\", \"{1}\", \"{2}\", new Number({3}));";

                XDocument xdocResponse = XDocument.Parse(surveyAnswer.XML);

                form.HiddenFieldsList = xdocResponse.Root.Attribute("HiddenFieldsList").Value;
                form.HighlightedFieldsList = xdocResponse.Root.Attribute("HighlightedFieldsList").Value;
                form.DisabledFieldsList = xdocResponse.Root.Attribute("DisabledFieldsList").Value;
                form.RequiredFieldsList = xdocResponse.Root.Attribute("RequiredFieldsList").Value;
                if (surveyAnswerList != null)
                {
                    form.FormCheckCodeObj = form.GetRelateCheckCodeObj(GetRelateFormObj(surveyAnswerList, surveyInfoList), checkcode);
                }
                else
                {
                    form.FormCheckCodeObj = form.GetCheckCodeObj(xdoc, xdocResponse, checkcode);
                }

                form.FormCheckCodeObj.GetVariableJavaScript(VariableDefinitions);
                form.FormCheckCodeObj.GetSubroutineJavaScript(VariableDefinitions);

                string PageName = GetPageName(xdoc, pageNumber);


                //Generate page level Java script (Before)
                JavaScript.Append(GetPageLevelJS(pageNumber, form, PageName, "Before"));
                //Generate page level Java script (After)
                JavaScript.Append(GetPageLevelJS(pageNumber, form, PageName, "After"));

                Dictionary<string, string> _SurveyAnswerFromDocumentDB = null;
                if (form.ResponseId != null)
                {
                  //  _SurveyAnswerFromDocumentDB = GetSurveyDataFromDocumentDB(form.SurveyInfo.SurveyName, form.ResponseId, "surveyid", Convert.ToString(pageNumber));
                }

                foreach (var fieldAttributes in metadata)
                {
                    var FieldValue = GetControlValue(xdocResponse, fieldAttributes.Name);

                    ////StartNewcode
                    //string FieldValue = string.Empty;
                    //if (_SurveyAnswerFromDocumentDB != null)
                    //{
                    //    FieldValue = (from element in _SurveyAnswerFromDocumentDB
                    //                  where element.Key == fieldAttributes.Name.ToLower()
                    //                  select element.Value).FirstOrDefault();
                    //}

                    //EndNewcode 
                    JavaScript.Append(GetFormJavaScript(checkcode, form, fieldAttributes.Name));

                    // TODO: Temporary until all field types are supported
                    var uniqueId = fieldAttributes.UniqueId;
                    var _FieldTypeID = _FieldsTypeIDs.Where(f => f.AttributeValue("UniqueId") == fieldAttributes.UniqueId).SingleOrDefault();

                    switch (fieldAttributes.FieldTypeId)
                    {
                        case 1: // textbox
                            var _TextBoxValue = FieldValue;
                            form.AddFields(GetTextBox(fieldAttributes, _Width, _Height, _TextBoxValue));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value,"textbox","datasource",Value)); 
                            break;

                        case 2://Label/Title
                            form.AddFields(GetLabel(fieldAttributes, _Width, _Height));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "lable", "datasource",Value)); 
                            break;
                        case 3://Label

                            break;
                        case 4://MultiLineTextBox

                            var _TextAreaValue = FieldValue;
                            form.AddFields(GetTextArea(fieldAttributes, _Width, _Height, _TextAreaValue));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "multiline", "datasource",Value)); 
                            break;
                        case 5://NumericTextBox

                            var _NumericTextBoxValue = FieldValue;
                            form.AddFields(GetNumericTextBox(fieldAttributes, _Width, _Height, _NumericTextBoxValue));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineNumberFormat, _FieldTypeID.Attribute("Name").Value, "number", "datasource", Value)); 
                            break;

                        case 7://DatePicker

                            var _DatePickerValue = FieldValue;
                            form.AddFields(GetDatePicker(_FieldTypeID, _Width, _Height, xdocResponse, _DatePickerValue, form));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineNumberFormat, _FieldTypeID.Attribute("Name").Value, "number", "datasource", Value)); 
                            break;
                        case 8: //TimePicker
                            var _timePickerValue = FieldValue;
                            form.AddFields(GetTimePicker(_FieldTypeID, _Width, _Height, xdocResponse, _timePickerValue, form));

                            break;
                        case 10://CheckBox
                                //Renuka
                            var _CheckBoxValue = FieldValue;
                            var checkbox = GetCheckBox(fieldAttributes, _Width, _Height, _CheckBoxValue);
                            //var checkboxFromXml = GetCheckBoxFromXml(_FieldTypeID, _Width, _Height, xdocResponse, _CheckBoxValue);
                            form.AddFields(checkbox);
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "checkbox", "datasource",Value)); 
                            break;

                        case 11://DropDown Yes/No

                            var _DropDownSelectedValueYN = FieldValue;

                            if (_DropDownSelectedValueYN == "1" || _DropDownSelectedValueYN == "true")
                            {
                                _DropDownSelectedValueYN = "Yes";
                            }

                            if (_DropDownSelectedValueYN == "0" || _DropDownSelectedValueYN == "false")
                            {

                                _DropDownSelectedValueYN = "No";
                            }

                            var dropdownselectedvalueYN = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValueYN, "Yes&#;No", 11);

                            form.AddFields(dropdownselectedvalueYN);//, "Yes&#;No", 11);
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "yesno", "datasource",Value)); 

                            break;
                        case 12://RadioList
                            var _GroupBoxValue1 = string.Empty;
                            form.AddFields(GetGroupBox(fieldAttributes, _Width + 12, _Height, _GroupBoxValue1));
                            var _RadioListSelectedValue1 = FieldValue;
                            string RadioListValues1 = "";
                            RadioListValues1 = fieldAttributes.ChoicesList;
                            form.AddFields(GetRadioList(fieldAttributes, _Width, _Height, _RadioListSelectedValue1));

                            break;

                        case 17://DropDown LegalValues

                            string DropDownValues1 = "";
                            DropDownValues1 = string.Join("&#;", fieldAttributes.SourceTableValues).ToLower();
                            var DropDownValues1FromXml = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("TextColumnName").Value);
                            var _DropDownSelectedValue1 = FieldValue;
                            form.AddFields(GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue1, DropDownValues1, 17));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "legalvalue", "datasource",Value)); 

                            break;
                        case 18://DropDown Codes

                            string DropDownValues2 = "";
                            DropDownValues2 = string.Join("&#;", fieldAttributes.SourceTableValues).ToLower();
                            var DropDownValues2FromXml = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("TextColumnName").Value);
                            var _DropDownSelectedValue2 = FieldValue;
                            var dropdownselectedcodevalue = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue2, DropDownValues2, 18);
                            form.AddFields(dropdownselectedcodevalue);

                            // form.AddFields(GetDropDown(_FieldTypeID, _Width, _Height, xdocResponse, _DropDownSelectedValue2, DropDownValues2, 18, form));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "code", "datasource",Value)); 

                            break;
                        case 19://DropDown CommentLegal

                            string DropDownValues = "";
                            DropDownValues = string.Join("&#;", fieldAttributes.SourceTableValues).ToLower();
                            string DropDownValuesFromXml = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("TextColumnName").Value);
                            var _DropDownSelectedValue = FieldValue;
                            //form.AddFields(GetDropDown(_FieldTypeID, _Width, _Height, xdocResponse, _DropDownSelectedValue, DropDownValues, 19, form));
                            var dropdownselectedcommentlegalval = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue, DropDownValues, 19);
                            form.AddFields(dropdownselectedcommentlegalval);
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "commentlegal", "datasource",Value)); 

                            break;
                        case 20://RelateButton
                            form.AddFields(GetRelateButton(_FieldTypeID, _Width, _Height, xdocResponse, form));
                            break;
                        case 21://GroupBox
                            var _GroupBoxValue = FieldValue;
                            form.AddFields(GetGroupBox(fieldAttributes, _Width, _Height, _GroupBoxValue));
                            //                                             pName, pType, pSource
                            //VariableDefinitions.AppendLine(string.Format(defineFormat, _FieldTypeID.Attribute("Name").Value, "", "datasource",Value)); 
                            break;
                    }
                }

                //var gender = new RadioList
                //{
                //    DisplayOrder = 30,
                //    Title = "Gender",
                //    Prompt = "Select your gender:",
                //    Required = true,
                //    Orientation = Orientation.Vertical
                //};
                //gender.AddChoices("Male,Female", ",");


                //var sports = new CheckBoxList
                //{
                //    DisplayOrder = 40,
                //    Title = "Favorite Sports",
                //    Prompt = "What are your favorite sports?",
                //    Orientation = Orientation.Horizontal
                //};
                //sports.AddChoices("Baseball,Football,Soccer,Basketball,Tennis,Boxing,Golf", ",");







                form.FormJavaScript = VariableDefinitions.ToString() + "\n" + JavaScript.ToString();
            }

            return form;
        }

        #region Get Survey Response Data From DocumentDB
        /// <summary>
        /// GetSurveyResponseDataFrom DocumentDB
        /// </summary>
        /// <param name="ResponseId"></param>
        /// <param name="SurveyId"></param>
        /// <param name="PageNo"></param> 
        public static Dictionary<string, string> GetSurveyDataFromDocumentDB(string surveyName, string ResponseId, string SurveyId, string PageId)
        {
            _isurveyDocumentDBStoreFacade = new SurveyDocumentDBFacade();
            //ResponseId = "7daa7fb4-d3df-4fae-9ca6-fb2584a52184"; 
            var response = _isurveyDocumentDBStoreFacade.ReadSurveyAnswerByResponseID(surveyName, SurveyId, ResponseId, PageId);
            return response.SurveyQAList; 
        }
        #endregion


        public static void UpdateHiddenFields(int CurrentPage, Form form, XDocument xdoc, XDocument xdocResponse, System.Collections.Specialized.NameValueCollection pPostedForm)
        {
            double _Width, _Height;
            _Width = 1024;
            _Height = 768;

            var _FieldsTypeIDs = from _FieldTypeID in
                                     xdoc.Descendants("Field")
                                 where _FieldTypeID.Parent.Attribute("Position").Value != (CurrentPage - 1).ToString()
                                 select _FieldTypeID;

            foreach (var _FieldTypeID in _FieldsTypeIDs)
            {
                bool IsFound = false;
                string Value = null;

                foreach (var key in pPostedForm.AllKeys.Where(x => x.StartsWith(form.FieldPrefix)))
                {
                    string fieldKey = key.Remove(0, form.FieldPrefix.Length);

                    if (fieldKey.Equals(_FieldTypeID.Attribute("Name").Value, StringComparison.OrdinalIgnoreCase))
                    {
                        Value = pPostedForm[key];
                        IsFound = true;
                        break;
                    }
                }

                if (IsFound)
                {
                    MvcDynamicForms.Fields.Field field = null;
                    FieldAttributes fieldAttributes = new FieldAttributes(_FieldTypeID, xdocResponse, form);

                    switch (_FieldTypeID.Attribute("FieldTypeId").Value)
                    {
                        case "1": // textbox
                            var _TextBoxValue = Value;
                            field = GetTextBox(fieldAttributes, _Width, _Height, _TextBoxValue);
                            break;

                        case "2": //Label/Title
                            field = GetLabel(fieldAttributes, _Width, _Height);
                            break;

                        case "3": //Label
                            break;

                        case "4": //MultiLineTextBox
                            var _TextAreaValue = Value;
                            field = GetTextArea(fieldAttributes, _Width, _Height, _TextAreaValue);
                            break;

                        case "5": //NumericTextBox
                            var _NumericTextBoxValue = Value;
                            field = GetNumericTextBox(fieldAttributes, _Width, _Height, _NumericTextBoxValue);
                            break;

                        case "7": // 7 DatePicker
                            var _DatePickerValue = Value;
                            field = GetDatePicker(_FieldTypeID, _Width, _Height, xdocResponse, _DatePickerValue, form);
                            break;

                        case "8": //TimePicker
                            var _timePickerValue = Value;
                            field = GetTimePicker(_FieldTypeID, _Width, _Height, xdocResponse, _timePickerValue, form);
                            break;

                        case "10"://CheckBox
                            var _CheckBoxValue = Value;
                            field = GetCheckBox(fieldAttributes, _Width, _Height, _CheckBoxValue);
                            break;

                        case "11"://DropDown Yes/No
                            var _DropDownSelectedValueYN = Value;
                            if (_DropDownSelectedValueYN == "1")
                            {
                                _DropDownSelectedValueYN = "Yes";
                            }

                            if (_DropDownSelectedValueYN == "0")
                            {

                                _DropDownSelectedValueYN = "No";
                            }
                            var dropdownselectedvalueYN = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValueYN, "Yes&#;No", 11);

                            form.AddFields(dropdownselectedvalueYN);

                            //field = GetDropDown(_FieldTypeID, _Width, _Height, xdocResponse, _DropDownSelectedValueYN, "Yes&#;No", 11, form);
                            break;

                        case "12": //RadioList
                            var _RadioListSelectedValue1 = Value;
                            string RadioListValues1 = "";
                            field = GetRadioList(fieldAttributes, _Width, _Height, _RadioListSelectedValue1);
                            break;

                        case "17": //DropDown LegalValues
                            string DropDownValues1 = "";
                            DropDownValues1 = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("CodeColumnName").Value);
                            var _DropDownSelectedValue1 = Value;
                            field = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue1, DropDownValues1, 17);
                            break;

                        case "18": //DropDown Codes
                            string DropDownValues2 = "";
                            DropDownValues2 = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("CodeColumnName").Value);
                            var _DropDownSelectedValue2 = Value;
                            field = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue2, DropDownValues2, 18);
                            break;

                        case "19": //DropDown CommentLegal
                            string DropDownValues = "";
                            DropDownValues = GetDropDownValues(xdoc, _FieldTypeID.Attribute("Name").Value, _FieldTypeID.Attribute("SourceTableName").Value, _FieldTypeID.Attribute("CodeColumnName").Value);
                            var _DropDownSelectedValue = Value;
                            field = GetDropDown(fieldAttributes, _Width, _Height, _DropDownSelectedValue, DropDownValues, 19);
                            break;

                        case "21": //GroupBox
                            field = GetGroupBox(fieldAttributes, _Width, _Height, Value);
                            break;
                    }

                    if (field != null)
                    {
                        field.IsPlaceHolder = true;
                        form.AddFields(field);
                    }
                }
            }
        }

        private static double GetHeight(XDocument xdoc)

        {

            try
            {
                if (GetOrientation(xdoc) == "Portrait")
                {
                    var _top = from Node in
                                   xdoc.Descendants("View")
                               select Node.Attribute("Height").Value;

                    return double.Parse(_top.First());
                }
                else
                {

                    var _top = from Node in
                                   xdoc.Descendants("View")
                               select Node.Attribute("Width").Value;

                    return double.Parse(_top.First());

                }

            }
            catch (System.Exception ex)
            {
                return 768;

            }

        }
        private static double GetWidth(XDocument xdoc)
        {

            try
            {
                if (GetOrientation(xdoc) == "Portrait")
                {
                    var _left = (from Node in
                                     xdoc.Descendants("View")
                                 select Node.Attribute("Width").Value);
                    return double.Parse(_left.First());
                }
                else
                {

                    var _top = from Node in
                                   xdoc.Descendants("View")
                               select Node.Attribute("Height").Value;

                    return double.Parse(_top.First());

                }
            }
            catch (System.Exception ex)
            {

                return 1024;
            }
        }
        // Orientation="Landscape"
        private static string GetOrientation(XDocument xdoc)
        {

            try
            {

                var Orientation = (from Node in
                                 xdoc.Descendants("View")
                                   select Node.Attribute("Orientation").Value);
                return Orientation.First().ToString();
            }
            catch (System.Exception ex)
            {

                return null;
            }
        }


        private static string GetControlValue(XDocument xdoc, string ControlName)
        {

            string ControlValue = "";

            if (!string.IsNullOrEmpty(xdoc.ToString()))
            {

                //XDocument xdoc = XDocument.Parse(Xml);


                var _ControlValues = from _ControlValue in
                                         xdoc.Descendants("ResponseDetail")
                                     where _ControlValue.Attribute("QuestionName").Value == ControlName.ToString()
                                     select _ControlValue;

                foreach (var _ControlValue in _ControlValues)
                {
                    ControlValue = _ControlValue.Value;
                }
            }

            return ControlValue;
        }

        private static RadioList GetRadioList(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var radiolist = new RadioList(fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue
            };

            return radiolist;
        }
        private static RadioList GetRadioList(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, string RadioListValues, Form form)
        {

            var RadioList = new RadioList();
            string ListString = _FieldTypeID.Attribute("List").Value;
            ListString = ListString.Replace("||", "|");
            List<string> Lists = ListString.Split('|').ToList<string>();


            Dictionary<string, bool> Choices = new Dictionary<string, bool>();

            List<string> Pattern = new List<string>();
            Choices = GetChoices(Lists[0].Split(',').ToList<string>());
            Pattern = Lists[1].Split(',').ToList<string>();

            RadioList.Title = _FieldTypeID.Attribute("Name").Value;
            RadioList.Prompt = _FieldTypeID.Attribute("PromptText").Value;
            RadioList.DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value);
            RadioList.Required = _FieldTypeID.Attribute("IsRequired").Value == "True" ? true : false;

            RadioList.RequiredMessage = "This field is required";
            RadioList.Key = _FieldTypeID.Attribute("Name").Value;

            RadioList.Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value);
            RadioList.Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value);
            RadioList.PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            RadioList.ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            RadioList.fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value;
            RadioList.fontSize = double.Parse(_FieldTypeID.Attribute("PromptFontSize").Value);
            RadioList.fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value;
            // RadioList.IsRequired = bool.Parse(_FieldTypeID.Attribute("IsRequired").Value);
            RadioList.ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value);
            RadioList.InputFieldfontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value;
            RadioList.InputFieldfontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value);
            RadioList.InputFieldfontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value;
            RadioList.Value = _ControlValue;
            RadioList.IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList");
            RadioList.IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList");
            RadioList.IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList");
            RadioList.IsRequired = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList");

            RadioList.ShowTextOnRight = bool.Parse(_FieldTypeID.Attribute("ShowTextOnRight").Value);
            RadioList.Choices = Choices;
            RadioList.Width = _Width;
            RadioList.Height = _Height;
            RadioList.Pattern = Pattern;
            RadioList.ChoicesList = ListString;
            // if (RadioList.Pattern[0] == "Vertical")
            if (_Height > _Width)
            {
                RadioList.Orientation = (Orientation)0;
            }
            else
            {

                RadioList.Orientation = (Orientation)1;
            }

            return RadioList;

        }

        //private static NumericTextBox GetNumericTextBox(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, Form form)
        private static NumericTextBox GetNumericTextBox(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var numericTextBox = new NumericTextBox(fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue
            };

            return numericTextBox;
        }

        //private static Literal GetLabel(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer)
        private static Literal GetLabel(FieldAttributes fieldAttributes, double formWidth, double formHeight)
        {
            var label = new Literal(fieldAttributes, formWidth, formHeight);
            return label;
        }
        //private static TextArea GetTextArea(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, Form form)
        private static TextArea GetTextArea(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var textArea = new TextArea(fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue
            };
            return textArea;
        }
        private static Hidden GetHiddenField(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue)
        {
            var result = new Hidden
            {
                Title = _FieldTypeID.Attribute("Name").Value,
                Key = _FieldTypeID.Attribute("Name").Value,
                IsPlaceHolder = true,
                Value = _ControlValue
            };

            return result;
        }

        //private static TextBox GetTextBox(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, Form form)
        private static TextBox GetTextBox(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var textBox = new TextBox(fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue
            };

            return textBox;
        }
        //Renuka
        private static CheckBox GetCheckBox(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var checkBox = new CheckBox(fieldAttributes, formWidth, formHeight)// CheckBox (fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue,
                Response = controlValue
            };

            return checkBox;
        }

#if DEBUG
        //Renuka
        private static CheckBox GetCheckBoxFromXml(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue)
        {
            var CheckBox = new CheckBox
            {
                Title = _FieldTypeID.Attribute("Name").Value,
                Prompt = _FieldTypeID.Attribute("PromptText").Value,
                DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value),
                RequiredMessage = "This field is required",
                Key = _FieldTypeID.Attribute("Name").Value,
                PromptTop = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value) + 2,
                PromptLeft = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value) + 20,
                Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value),
                Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value),
                PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                //ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                ControlWidth = 10,
                Checked = _ControlValue == "Yes" ? true : _ControlValue == "true" ? true : false,
                fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value,
                fontSize = double.Parse(_FieldTypeID.Attribute("PromptFontSize").Value),
                fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value,
                IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList"),
                IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList"),
                IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList"),
                ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value)



            };


            return CheckBox;

        }

#endif
        private static DatePicker GetDatePicker(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, Form form)
        {

            var DatePicker = new DatePicker
            {
                Title = _FieldTypeID.Attribute("Name").Value,
                Prompt = _FieldTypeID.Attribute("PromptText").Value,
                DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value),
                //  Required = _FieldTypeID.Attribute("IsRequired").Value == "True" ? true : false,
                //RequiredMessage = _FieldTypeID.Attribute("PromptText").Value + " is required",
                RequiredMessage = "This field is required",
                Key = _FieldTypeID.Attribute("Name").Value,
                PromptTop = _Height * double.Parse(_FieldTypeID.Attribute("PromptTopPositionPercentage").Value),
                PromptLeft = _Width * double.Parse(_FieldTypeID.Attribute("PromptLeftPositionPercentage").Value),
                Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value),
                Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value),
                PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value,
                fontSize = double.Parse(_FieldTypeID.Attribute("PromptFontSize").Value),
                fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value,
                //IsRequired = bool.Parse(_FieldTypeID.Attribute("IsRequired").Value),
                IsRequired = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList"),
                Required = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList"),
                InputFieldfontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value,
                InputFieldfontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value),
                InputFieldfontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value,
                IsReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value),
                Lower = _FieldTypeID.Attribute("Lower").Value,
                Upper = _FieldTypeID.Attribute("Upper").Value,
                Value = _ControlValue,
                IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList"),
                IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList"),
                IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList"),
                ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value),
                Pattern = _FieldTypeID.Attribute("Pattern").Value

            };
            return DatePicker;

        }

        private static TimePicker GetTimePicker(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, Form form)
        {

            var TimePicker = new TimePicker
            {
                Title = _FieldTypeID.Attribute("Name").Value,
                Prompt = _FieldTypeID.Attribute("PromptText").Value,
                DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value),
                // Required = _FieldTypeID.Attribute("IsRequired").Value == "True" ? true : false,
                //RequiredMessage = _FieldTypeID.Attribute("PromptText").Value + " is required",
                RequiredMessage = "This field is required",
                Key = _FieldTypeID.Attribute("Name").Value,
                PromptTop = _Height * double.Parse(_FieldTypeID.Attribute("PromptTopPositionPercentage").Value),
                PromptLeft = _Width * double.Parse(_FieldTypeID.Attribute("PromptLeftPositionPercentage").Value),
                Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value),
                Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value),
                PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value),
                fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value,
                fontSize = double.Parse(_FieldTypeID.Attribute("PromptFontSize").Value),
                fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value,
                //IsRequired = bool.Parse(_FieldTypeID.Attribute("IsRequired").Value),
                IsRequired = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList"),
                Required = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList"),
                InputFieldfontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value,
                InputFieldfontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value),
                InputFieldfontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value,
                IsReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value),
                Lower = _FieldTypeID.Attribute("Lower").Value,
                Upper = _FieldTypeID.Attribute("Upper").Value,
                Value = _ControlValue,
                IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList"),
                IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList"),
                IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList"),
                ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value),
                Pattern = _FieldTypeID.Attribute("Pattern").Value

            };
            return TimePicker;

        }

        private static RelateButton GetRelateButton(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, Form form)
        {


            RelateButton RelateButton = new RelateButton();

            RelateButton.Title = _FieldTypeID.Attribute("Name").Value;
            RelateButton.Prompt = _FieldTypeID.Attribute("PromptText").Value;
            RelateButton.DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value);
            RelateButton.RequiredMessage = "This field is required";
            RelateButton.Key = _FieldTypeID.Attribute("Name").Value;
            RelateButton.Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value);
            RelateButton.Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value);
            RelateButton.ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            RelateButton.ControlHeight = _Height * double.Parse(_FieldTypeID.Attribute("ControlHeightPercentage").Value);
            RelateButton.fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value;
            RelateButton.fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value;
            RelateButton.InputFieldfontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value;
            RelateButton.InputFieldfontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value);
            RelateButton.InputFieldfontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value;
            // RelateButton.IsReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value);
            RelateButton.IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList");
            RelateButton.IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList");
            RelateButton.IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList");
            RelateButton.RelatedViewId = _FieldTypeID.Attribute("RelatedViewId").Value;
            return RelateButton;
        }


        //renuka
        private static Select GetDropDown(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue, string DropDownValues, int FieldTypeId)
        {
            var select = new Select(fieldAttributes, formWidth, formHeight)//, DropDownValues, FieldTypeId)
            {
                Value = controlValue
            };
            select.SelectType = FieldTypeId;
            select.SelectedValue = controlValue; 

            select.ShowEmptyOption = true;
            select.EmptyOption = "Select";
            select.AddChoices(DropDownValues, "&#;");
            select.SelectedValue = controlValue;
            if (!string.IsNullOrWhiteSpace(controlValue))
            {
                select.Choices[controlValue] = true;
            }

            return select;
        }
        private static Select GetDropDown(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue, string DropDownValues, int FieldTypeId, Form form)
        {



            Select DropDown = new Select();

            DropDown.Title = _FieldTypeID.Attribute("Name").Value;
            DropDown.Prompt = _FieldTypeID.Attribute("PromptText").Value;
            DropDown.DisplayOrder = int.Parse(_FieldTypeID.Attribute("TabIndex").Value);
            // DropDown.Required = _FieldTypeID.Attribute("IsRequired").Value == "True" ? true : false;
            DropDown.RequiredMessage = "This field is required";
            DropDown.Key = _FieldTypeID.Attribute("Name").Value;
            DropDown.PromptTop = _Height * double.Parse(_FieldTypeID.Attribute("PromptTopPositionPercentage").Value);
            DropDown.PromptLeft = _Width * double.Parse(_FieldTypeID.Attribute("PromptLeftPositionPercentage").Value);
            DropDown.Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value);
            DropDown.Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value);
            DropDown.PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            DropDown.ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            DropDown.fontstyle = _FieldTypeID.Attribute("PromptFontStyle").Value;
            DropDown.fontSize = double.Parse(_FieldTypeID.Attribute("PromptFontSize").Value);
            DropDown.fontfamily = _FieldTypeID.Attribute("PromptFontFamily").Value;
            //IsRequired = bool.Parse(_FieldTypeID.Attribute("IsRequired").Value),
            DropDown.IsRequired = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList");
            DropDown.Required = Helpers.GetRequiredControlState(form.RequiredFieldsList.ToString(), _FieldTypeID.Attribute("Name").Value, "RequiredFieldsList");
            DropDown.InputFieldfontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value;
            DropDown.InputFieldfontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value);
            DropDown.InputFieldfontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value;
            DropDown.ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value);
            DropDown.ShowEmptyOption = true;
            DropDown.SelectType = FieldTypeId;
            DropDown.SelectedValue = _ControlValue;
            DropDown.IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList");
            DropDown.IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList");
            DropDown.IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList");
            DropDown.ControlFontSize = float.Parse(_FieldTypeID.Attribute("ControlFontSize").Value);
            DropDown.ControlFontStyle = _FieldTypeID.Attribute("ControlFontStyle").Value;

            DropDown.EmptyOption = "Select";

            DropDown.AddChoices(DropDownValues, "&#;");

            if (!string.IsNullOrWhiteSpace(_ControlValue))
            {
                DropDown.Choices[_ControlValue] = true;
            }

            return DropDown;
        }
        public static string GetDropDownValuesDb(string ControlName, string TableName, string CodeColumnName)
        {
            StringBuilder DropDownValues = new StringBuilder();

            MetaData getDropdownVal = new MetaData();
            DataTable dtDropDownVal = getDropdownVal.GetDropdownDB(TableName, CodeColumnName);


            foreach (DataRow _SourceTableValue in dtDropDownVal.Rows)
            {
                DropDownValues.Append(_SourceTableValue[CodeColumnName]);

                DropDownValues.Append("&#;");
            }
            return DropDownValues.ToString();
        }

        public static string GetDropDownValues(XDocument xdoc, string ControlName, string TableName, string CodeColumnName)
        {
            StringBuilder DropDownValues = new StringBuilder();


            if (!string.IsNullOrEmpty(xdoc.ToString()))
            {

                // XDocument xdoc = XDocument.Parse(Xml.ToString());


                var _ControlValues = from _ControlValue in
                                         xdoc.Descendants("SourceTable")
                                     where _ControlValue.Attribute("TableName").Value == TableName.ToString()
                                     select _ControlValue;

                foreach (var _ControlValue in _ControlValues)
                {


                    var _SourceTableValues = from _SourceTableValue in _ControlValues.Descendants("Item")

                                             select _SourceTableValue;

                    foreach (var _SourceTableValue in _SourceTableValues)
                    {

                        // DropDownValues.Append(_SourceTableValue.LastAttribute.Value );
                        if (!string.IsNullOrEmpty(CodeColumnName))
                        {
                            string Xelement = _SourceTableValue.ToString().ToLower();
                            XElement NewXElement = XElement.Parse(Xelement);
                            DropDownValues.Append(NewXElement.Attribute(CodeColumnName.ToLower()).Value.Trim());
                        }
                        else
                        {
                            DropDownValues.Append(_SourceTableValue.Attributes().FirstOrDefault().Value.Trim());
                        }
                        DropDownValues.Append("&#;");
                    }
                }
            }

            return DropDownValues.ToString();
        }

        private static GroupBox GetGroupBox(FieldAttributes fieldAttributes, double formWidth, double formHeight, string controlValue)
        {
            var groupbox = new GroupBox(fieldAttributes, formWidth, formHeight)
            {
                Value = controlValue
            };

            return groupbox;
        }

        private static GroupBox GetGroupBox(XElement _FieldTypeID, double _Width, double _Height, XDocument SurveyAnswer, string _ControlValue)
        {


            var GroupBox = new GroupBox();

            GroupBox.Title = _FieldTypeID.Attribute("Name").Value;
            GroupBox.Prompt = _FieldTypeID.Attribute("PromptText").Value;
            GroupBox.RequiredMessage = "This field is required";
            GroupBox.Key = _FieldTypeID.Attribute("Name").Value + "_GroupBox";
            //GroupBox.Key = _FieldTypeID.Attribute("Name").Value;
            GroupBox.PromptTop = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value);
            GroupBox.PromptLeft = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value);
            GroupBox.Top = _Height * double.Parse(_FieldTypeID.Attribute("ControlTopPositionPercentage").Value);
            GroupBox.Left = _Width * double.Parse(_FieldTypeID.Attribute("ControlLeftPositionPercentage").Value);
            //GroupBox.PromptWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value);
            GroupBox.ControlHeight = _Height * double.Parse(_FieldTypeID.Attribute("ControlHeightPercentage").Value) - 12;
            GroupBox.ControlWidth = _Width * double.Parse(_FieldTypeID.Attribute("ControlWidthPercentage").Value) - 12;
            GroupBox.fontstyle = _FieldTypeID.Attribute("ControlFontStyle").Value;
            GroupBox.fontSize = double.Parse(_FieldTypeID.Attribute("ControlFontSize").Value);
            GroupBox.fontfamily = _FieldTypeID.Attribute("ControlFontFamily").Value;
            // GroupBox.ReadOnly = bool.Parse(_FieldTypeID.Attribute("IsReadOnly").Value);
            GroupBox.IsHidden = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HiddenFieldsList");
            GroupBox.IsHighlighted = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "HighlightedFieldsList");
            GroupBox.IsDisabled = Helpers.GetControlState(SurveyAnswer, _FieldTypeID.Attribute("Name").Value, "DisabledFieldsList");
            if (_FieldTypeID.Attribute("BackgroundColor") != null)
            {
                var color = Color.FromArgb((int)(int.Parse(_FieldTypeID.Attribute("BackgroundColor").Value)) + unchecked((int)0xFF000000));
                string HexValue = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
                GroupBox.BackgroundColor = HexValue;
            }


            return GroupBox;





        }
        private static int GetNumberOfPages(XDocument Xml)
        {
            var _FieldsTypeIDs = from _FieldTypeID in
                                     Xml.Descendants("View")

                                 select _FieldTypeID;

            return _FieldsTypeIDs.Elements().Count();
        }

        //get pegeid for xml
        private static string GetPageId(XDocument xdoc, int PageNumber)
        {
            //  XDocument xdoc = XDocument.Parse(Xml);

            XElement XElement = xdoc.XPathSelectElement("Template/Project/View/Page[@Position = '" + (PageNumber - 1).ToString() + "']");



            return XElement.Attribute("PageId").Value.ToString();
        }

        private static string GetPageName(XDocument xdoc, int PageNumber)
        {
            //XDocument xdoc = XDocument.Parse(Xml);

            XElement XElement = xdoc.XPathSelectElement("Template/Project/View/Page[@Position = '" + (PageNumber - 1).ToString() + "']");



            return XElement.Attribute("Name").Value.ToString();
        }

        private static string GetFormJavaScript(string CheckCode, Form form, string controlName)
        {// controlName

            StringBuilder B_JavaScript = new StringBuilder();
            EnterRule FunctionObject_B = (EnterRule)form.FormCheckCodeObj.GetCommand("level=field&event=before&identifier=" + controlName);
            if (FunctionObject_B != null && !FunctionObject_B.IsNull())
            {
                B_JavaScript.Append("function " + controlName.ToLower());
                FunctionObject_B.ToJavaScript(B_JavaScript);
            }

            StringBuilder A_JavaScript = new StringBuilder();
            EnterRule FunctionObject_A = (EnterRule)form.FormCheckCodeObj.GetCommand("level=field&event=after&identifier=" + controlName);
            if (FunctionObject_A != null && !FunctionObject_A.IsNull())
            {
                A_JavaScript.Append("function " + controlName.ToLower());
                FunctionObject_A.ToJavaScript(A_JavaScript);
            }

            EnterRule FunctionObject = (EnterRule)form.FormCheckCodeObj.GetCommand("level=field&event=click&identifier=" + controlName);
            if (FunctionObject != null && !FunctionObject.IsNull())
            {
                A_JavaScript.Append("function " + controlName.ToLower());
                FunctionObject.ToJavaScript(A_JavaScript);
            }

            return B_JavaScript.ToString() + "  " + A_JavaScript.ToString();
        }

        private static string GetPageLevelJS(int PageNumber, Form form, string PageName, string BeforeOrAfter)
        {
            StringBuilder JavaScript = new StringBuilder();
            if (BeforeOrAfter == "Before")
            {
                Epi.Core.EnterInterpreter.Rules.Rule_Begin_Before_Statement FunctionObject_B = (Epi.Core.EnterInterpreter.Rules.Rule_Begin_Before_Statement)form.FormCheckCodeObj.GetCommand("level=page&event=before&identifier=" + PageName);
                if (FunctionObject_B != null && !FunctionObject_B.IsNull())
                {

                    JavaScript.Append("$(document).ready(function () {  ");
                    JavaScript.Append("page" + PageNumber + "_before();");
                    JavaScript.Append("});");

                    JavaScript.Append("\n\nfunction page" + PageNumber);
                    FunctionObject_B.ToJavaScript(JavaScript);


                }
            }
            if (BeforeOrAfter == "After")
            {
                Epi.Core.EnterInterpreter.Rules.Rule_Begin_After_Statement FunctionObject_A = (Epi.Core.EnterInterpreter.Rules.Rule_Begin_After_Statement)form.FormCheckCodeObj.GetCommand("level=page&event=after&identifier=" + PageName);
                if (FunctionObject_A != null && !FunctionObject_A.IsNull())
                {
                    JavaScript.AppendLine("$(document).ready(function () {");
                    //JavaScript.AppendLine("$('#myform').submit(function () {");
                    //  JavaScript.AppendLine("page" + PageNumber + "_after();})");
                    JavaScript.AppendLine("$(\"[href]\").click(function(e) {");
                    JavaScript.AppendLine("page" + PageNumber + "_after();  e.preventDefault(); })");

                    JavaScript.AppendLine("$(\"#ContinueButton\").click(function(e) {");
                    JavaScript.AppendLine("page" + PageNumber + "_after();  e.preventDefault(); })");

                    JavaScript.AppendLine("$(\"#PreviousButton\").click(function(e) {");
                    JavaScript.AppendLine("page" + PageNumber + "_after();  e.preventDefault(); })");
                    JavaScript.AppendLine("});");

                    JavaScript.Append("\n\nfunction page" + PageNumber);
                    FunctionObject_A.ToJavaScript(JavaScript);

                }
            }

            return JavaScript.ToString();
        }


        private static Dictionary<string, bool> GetChoices(List<string> List)
        {
            Dictionary<string, bool> NewList = new Dictionary<string, bool>();
            foreach (var _List in List)
            {
                NewList.Add(_List, false);
            }
            return NewList;
        }
        private static List<Epi.Web.Enter.Common.Helper.RelatedFormsObj> GetRelateFormObj(List<SurveyAnswerDTO> surveyAnswerList, List<SurveyInfoDTO> surveyInfoList)
        {

            List<Epi.Web.Enter.Common.Helper.RelatedFormsObj> List = new List<Enter.Common.Helper.RelatedFormsObj>();


            for (int i = 0; surveyAnswerList.Count() > i; i++)
            {


                Epi.Web.Enter.Common.Helper.RelatedFormsObj RelatedFormsObj = new Epi.Web.Enter.Common.Helper.RelatedFormsObj();
                XDocument xdocResponse1 = XDocument.Parse(surveyAnswerList[i].XML);
                XDocument xdoc1 = XDocument.Parse(surveyInfoList[i].XML.ToString());
                RelatedFormsObj.MetaData = xdoc1;
                RelatedFormsObj.Response = xdocResponse1;


                List.Add(RelatedFormsObj);
            }

            return List;
        }

    }
}
