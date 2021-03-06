﻿using System;
using System.Text;
using System.Web.Mvc;
using Epi.Cloud.Common.Metadata;
using Epi.Core.EnterInterpreter;

namespace MvcDynamicForms.Fields
{
    /// <summary>
    /// Represents a single html checkbox input field.
    /// </summary>
    [Serializable]
    public class CheckBox : InputField
    {
        public CheckBox()
        {
        }

        public CheckBox(FieldAttributes fieldAttributes, double formWidth, double formHeight)
        {
            InitializeFromMetadata(fieldAttributes, formWidth, formHeight);
        }

        //Renuka
        private string _checkedValue = "Yes";
        private string _uncheckedValue = "No";
        new private string _promptClass = "MvcDynamicCheckboxPrompt";

        /// <summary>
        /// The text to be used as the user's response when they check the checkbox.
        /// </summary>
        public string CheckedValue
        {
            get
            {
                return _checkedValue;
            }
            set
            {
                _checkedValue = value;
            }
        }
        /// <summary>
        /// The text to be used as the user's response when they do not check the checkbox.
        /// </summary>
        public string UncheckedValue
        {
            get
            {
                return _uncheckedValue;
            }
            set
            {
                _uncheckedValue = value;
            }
        }
        /// <summary>
        /// The state of the checkbox.
        /// </summary>
        public bool Checked { get; set; }

        public override string Response
        {
            get
            {
                return Checked ? _checkedValue : _uncheckedValue;
            }
            set
            {
                switch (value.ToLower())
                {
                    case "yes":
                    case "true":
                        Checked = true;
                        break;
                    case "no":
                    case "false":
                    default:
                        Checked = false;
                        break;
                }

            }
        }

        public override bool Validate()
        {
            /*
            if (Required && !Checked)
            {
                // Isn't valid
                Error = _requiredMessage;
                return false;
            }

            // Is Valid
            */
            ClearError();
            return true;
        }

        protected override void InitializeFromMetadata(FieldAttributes fieldAttributes, double formWidth, double formHeight)
        {
            base.InitializeFromMetadata(fieldAttributes, formWidth, formHeight);

            PromptTop = formHeight * fieldAttributes.ControlTopPositionPercentage + 2;
            PromptLeft = formWidth * fieldAttributes.ControlLeftPositionPercentage + 20;
            PromptWidth = formWidth * fieldAttributes.ControlWidthPercentage;
            ControlWidth = 10;
            //ControlHeight = 0;
            //InputFieldfontSize = 0;
            //InputFieldfontfamily = null;
            //InputFieldfontstyle = null;
        }

        public override string RenderHtml()
        {
            var inputName = _form.FieldPrefix + _key;
            var html = new StringBuilder();
            string ErrorStyle = string.Empty;

            // error label
            if (!IsValid)
            {
                //Add new Error to the error Obj
                ErrorStyle = ";border-color: red";
            }

            // checkbox input
            var chk = new TagBuilder("input");
            chk.Attributes.Add("id", inputName);
            chk.Attributes.Add("name", inputName);
            chk.Attributes.Add("type", "checkbox");
            if (Checked) chk.Attributes.Add("checked", "checked");
            chk.Attributes.Add("value", bool.TrueString);
            string IsHiddenStyle = "";
            string IsHighlightedStyle = "";
            if (_IsHidden)
            {
                IsHiddenStyle = "display:none";
            }
            if (_IsHighlighted)
            {
                IsHighlightedStyle = "background-color:yellow";
            }

            //if (_IsDisabled)
            //{
            //    chk.Attributes.Add("disabled", "disabled");
            //}
            //Renuka
            chk.Attributes.Add("style", "position:absolute;left:" + _left.ToString() + "px;top:" + _top.ToString() + "px" + ";width:" + _controlWidth.ToString() + "px" + ErrorStyle + ";" + IsHiddenStyle + ";");

            chk.MergeAttributes(_inputHtmlAttributes);
            ////////////Check code start//////////////////
            EnterRule FunctionObjectAfter = (EnterRule)_form.FormCheckCodeObj.GetCommand("level=field&event=after&identifier=" + _key);
            if (FunctionObjectAfter != null && !FunctionObjectAfter.IsNull())
            {
                chk.Attributes.Add("onblur", "return " + _key + "_after();"); //After
            }
            EnterRule FunctionObjectBefore = (EnterRule)_form.FormCheckCodeObj.GetCommand("level=field&event=before&identifier=" + _key);
            if (FunctionObjectBefore != null && !FunctionObjectBefore.IsNull())
            {
                chk.Attributes.Add("onfocus", "return " + _key + "_before();"); //Before
            }


            EnterRule FunctionObjectClick = (EnterRule)_form.FormCheckCodeObj.GetCommand("level=field&event=click&identifier=" + _key);
            if (FunctionObjectClick != null && !FunctionObjectClick.IsNull())
            {
                chk.Attributes.Add("onclick", "return " + _key + "_click();");
            }




            ////////////Check code end//////////////////
            html.Append(chk.ToString(TagRenderMode.SelfClosing));

            // prompt label
            var prompt = new TagBuilder("label");
            prompt.SetInnerText(Prompt);
            prompt.Attributes.Add("for", inputName);
            prompt.Attributes.Add("class", "EpiLabel");
            prompt.Attributes.Add("Id", "label" + inputName);
            StringBuilder StyleValues = new StringBuilder();
            StyleValues.Append(GetContolStyle(_fontstyle.ToString(), _promptTop.ToString(), _promptLeft.ToString(), null, null, IsHidden));
            prompt.Attributes.Add("style", StyleValues.ToString()+ " ; " + IsHighlightedStyle );
            html.Append(prompt.ToString());
            if (ReadOnly || _IsDisabled)
            {
                var scriptReadOnlyText = new TagBuilder("script");
                //scriptReadOnlyText.InnerHtml = "$(function(){$('#" + inputName + "').attr('disabled','disabled')});";
                scriptReadOnlyText.InnerHtml = "$(function(){  var List = new Array();List.push('" + _key + "');CCE_Disable(List, false);});";
                html.Append(scriptReadOnlyText.ToString(TagRenderMode.Normal));
            }

            // hidden input (so that value is posted when checkbox is unchecked)
            var hdn = new TagBuilder("input");
            hdn.Attributes.Add("type", "hidden");
            hdn.Attributes.Add("id", inputName + "_hidden");
            hdn.Attributes.Add("name", inputName);
            hdn.Attributes.Add("value", bool.FalseString);
            ////////////Check code start//////////////////
            //EnterRule FunctionObjectAfter = (EnterRule)_form.FormCheckCodeObj.GetCommand("level=field&event=after&identifier=" + _key);
            //if (FunctionObjectAfter != null && !FunctionObjectAfter.IsNull())
            //{
            //    hdn.Attributes.Add("onblur", "return " + _key + "_after();"); //After
            //}
            //EnterRule FunctionObjectBefore = (EnterRule)_form.FormCheckCodeObj.GetCommand("level=field&event=before&identifier=" + _key);
            //if (FunctionObjectBefore != null && !FunctionObjectBefore.IsNull())
            //{ 
            //    hdn.Attributes.Add("onfocus", "return " + _key + "_before();"); //Before
            //}

            ////////////Check code end//////////////////
            html.Append(hdn.ToString(TagRenderMode.SelfClosing));


            //prevent check box control to submit on enter click
            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.InnerHtml = "$('#" + inputName + "').BlockEnter('" + inputName + "');";
            scriptBuilder.ToString(TagRenderMode.Normal);
            html.Append(scriptBuilder.ToString(TagRenderMode.Normal));

            var wrapper = new TagBuilder(_fieldWrapper);
            wrapper.Attributes["class"] = _fieldWrapperClass;
            if (_IsHidden)
            {
                wrapper.Attributes["style"] = "display:none";

            }
            wrapper.Attributes["id"] = inputName + "_fieldWrapper";
            wrapper.InnerHtml = html.ToString();
            return wrapper.ToString();
        }
    }
}
