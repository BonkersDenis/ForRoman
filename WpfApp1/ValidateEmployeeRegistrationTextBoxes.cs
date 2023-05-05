using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Praktik1;

internal class ValidateEmployeeRegistrationTextBoxes : IDataErrorInfo
{
    private Dictionary<string, bool> IsTextBoxCorrectInput = new Dictionary<string, bool> { { "ID", false }, { "Surname", false }, { "Name", false }, { "Patronymic", true }, { "Passport", false }, { "PhoneNumber", true }, { "Email", false } };
    public string? ID { get; set; }
    public string? Surname { get; set; }
    public string? Name { get; set; }
    public string? Patronymic { get; set; }
    public string? Passport { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public bool IsAllInputCorrect()
    {
        return IsTextBoxCorrectInput.Values.All(a => a == true);
    }

    public string this[string columnName]
    {
        get
        {
            string error = string.Empty;
            switch (columnName)
            {
                case "ID":
                    if (ID == null)
                    {
                        error = "ID must be not empty!";
                        IsTextBoxCorrectInput["ID"] = false;
                        break;
                    }
                    if (!ID.All(Char.IsNumber))
                    {
                        error = "ID must contain only numbers!";
                        IsTextBoxCorrectInput["ID"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["ID"] = true;
                    break;

                case "Surname":
                    if (Surname == null)
                    {
                        error = "Surname must be not empty!";
                        IsTextBoxCorrectInput["Surname"] = false;
                        break;
                    }
                    if (!Surname.All(Char.IsLetter))
                    {
                        error = "Surname must contain only letters!";
                        IsTextBoxCorrectInput["Surname"] = false;
                        break;
                    }
                    if (!Surname.Skip(1).All(Char.IsLower))
                    {
                        error = "All surname letters must be lower except first!";
                        IsTextBoxCorrectInput["Surname"] = false;
                        break;
                    }
                    if (Char.IsLower(Surname[0]))
                    {
                        error = "First surname letter must be higher!";
                        IsTextBoxCorrectInput["Surname"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["Surname"] = true;
                    break;

                case "Name":
                    if (Name == null)
                    {
                        error = "Name must be not empty!";
                        IsTextBoxCorrectInput["Name"] = false;
                        break;
                    }
                    if (!Name.All(Char.IsLetter))
                    {
                        error = "Name can contain only letters!";
                        IsTextBoxCorrectInput["Name"] = false;
                        break;
                    }
                    if (!Name.Skip(1).All(Char.IsLower))
                    {
                        error = "All name letters must be lower except first!";
                        IsTextBoxCorrectInput["Name"] = false;
                        break;
                    }
                    if (Char.IsLower(Name[0]))
                    {
                        error = "First name letter must be higher!";
                        IsTextBoxCorrectInput["Name"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["Name"] = true; ;
                    break;

                case "Patronymic":
                    if (Patronymic == null)
                    {
                        IsTextBoxCorrectInput["Patronymic"] = true;
                        break;
                    }
                    if (!Patronymic.All(Char.IsLetter))
                    {
                        error = "Patronymic can contain only letters!";
                        IsTextBoxCorrectInput["Patronymic"] = false;
                        break;
                    }
                    if (!Patronymic.Skip(1).All(Char.IsLower))
                    {
                        error = "All patronymic letters must be lower except first!";
                        IsTextBoxCorrectInput["Patronymic"] = false;
                        break;
                    }
                    if (Char.IsLower(Patronymic[0]))
                    {
                        error = "First patronymic letter must be higher!";
                        IsTextBoxCorrectInput["Patronymic"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["Patronymic"] = true;
                    break;

                case "Passport":
                    if (Passport == null)
                    {
                        error = "Passport must be not empty!";
                        IsTextBoxCorrectInput["Passport"] = false;
                        break;
                    }
                    if (Passport.Length != 10)
                    {
                        error = "Passport must include ten numbers!";
                        IsTextBoxCorrectInput["Passport"] = false;
                        break;
                    }
                    if (!Passport.All(Char.IsNumber))
                    {
                        error = "Passport must include only numbers!";
                        IsTextBoxCorrectInput["Passport"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["Passport"] = true;
                    break;

                case "PhoneNumber":
                    if (PhoneNumber == null)
                    {
                        IsTextBoxCorrectInput["PhoneNumber"] = true;
                        break;
                    }
                    if (((PhoneNumber[0] == '+' && PhoneNumber[1] != '7') || PhoneNumber.Length != 12) && (PhoneNumber[0] != '8' || PhoneNumber.Length != 11))
                    {
                        error = "Phone number must look like +79873691806 or 89873691806!";
                        IsTextBoxCorrectInput["PhoneNumber"] = false;
                        break;
                    }
                    if (!PhoneNumber.Skip(1).All(Char.IsNumber))
                    {
                        error = "Phone number must contain only numbers except '+'!";
                        IsTextBoxCorrectInput["PhoneNumber"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["PhoneNumber"] = true;
                    break;

                case "Email":
                    if (Email == null)
                    {
                        error = "Email must be not empty!";
                        IsTextBoxCorrectInput["Email"] = false;
                        break;
                    }
                    if (Email.Where(x => x == '@').Count() != 1 || Email.Substring(Email.IndexOf('@')).Where(x => x == '.').Count() != 1)
                    {
                        error = "Email must contain one '@' and one '.' in domen!";
                        IsTextBoxCorrectInput["Email"] = false;
                        break;
                    }
                    IsTextBoxCorrectInput["Email"] = true;
                    break;
            }
            return error;
        }
    }

    public string Error
    {
        get => throw new NotImplementedException();
    }
}