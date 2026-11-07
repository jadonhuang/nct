using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nct.Validation
{

    public enum ValidateType
    {
        Email,
        Number,
        Decimal,
        Url,
        Mobile,
        Tel,
        Contact,
        IdCard,
        PostCode,
        ZHCN,
        LetterAndNumber,
        ValidString

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CustomValidateAttribute : ValidationAttribute
    {

        private const string DefaultErrMsg = "The field {0} is not a valid pattern.";
        private string _regStr = "";
        private ValidateType _validateType;
        public string GetRegStr()
        {
            string regStr = "";
            switch (_validateType)
            {
                case ValidateType.Email:
                    regStr = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                    break;
                case ValidateType.Number:
                    regStr = "^[0-9]+$";
                    break;
                case ValidateType.Decimal:
                    regStr = "^[0-9]+[.]?[0-9]+$";
                    break;
                case ValidateType.Mobile:
                    regStr = @"^1[2-9]\d{9}$";
                    break;
                case ValidateType.Tel:
                    regStr = @"^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$";
                    break;
                case ValidateType.Contact:
                    regStr = @"^1[2-9]\d{9}$|^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$";
                    break;

                case ValidateType.IdCard:
                    regStr = @"^\d{17}[\d|X]|^\d{15}$";
                    break;
                case ValidateType.PostCode:
                    regStr = @"^\d{6}$";
                    break;
                case ValidateType.ZHCN:
                    regStr = "^[\u0391-\uFFE5]+$";
                    break;
                case ValidateType.Url:
                    regStr = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
                    break;
                case ValidateType.LetterAndNumber:
                    regStr = @"^[a-zA-Z0-9_\-]+$";
                    break;
                case ValidateType.ValidString:
                    regStr = @"^[\w-]+$";
                    break;
            }

            return regStr;
        }


        public CustomValidateAttribute(ValidateType validateType) : base(DefaultErrMsg)
        {
            _validateType = validateType;
            _regStr = GetRegStr();
        }


        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString)) return true;
            return new Regex(_regStr).IsMatch(valueAsString);
        }





    }

}
