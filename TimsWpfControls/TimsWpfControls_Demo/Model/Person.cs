using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class Person : ObservableValidator
    {

        private Gender _Gender;
        public Gender Gender
        {
            get { return _Gender; }
            set { SetProperty(ref _Gender, value); }
        }


        private string _FirstName;

        [Required (ErrorMessage = "Everybody has a first name")]
        public string FirstName
        {
            get { return _FirstName; }
            set { SetProperty(ref _FirstName, value, true); }
        }

        private string _LastName;

        [Required(ErrorMessage = "Everybody has a last name")]
        public string LastName
        {
            get { return _LastName; }
            set { SetProperty(ref _LastName, value, true); }
        }

        private int _Age;
        [Range(0, 120, ErrorMessage = "This is a quite strange age")]
        public int Age
        {
            get { return _Age; }
            set { SetProperty(ref _Age, value, true); }
        }

    }
   
}
