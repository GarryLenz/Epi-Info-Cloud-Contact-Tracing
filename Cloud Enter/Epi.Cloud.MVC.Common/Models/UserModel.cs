﻿using System.ComponentModel.DataAnnotations;

namespace Epi.Cloud.MVC.Models
{
    public class UserModel
    {


        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "The email address you entered is not in the proper format.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Confirm email is required.")]
        //[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
        //[Compare("AdminEmail", ErrorMessage = "The email and confirmation do not match.")]
        public string ConfirmEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditMode { get; set; }
        public int UserId { get; set; }

    }

}