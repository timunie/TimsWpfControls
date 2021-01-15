using System;
using System.Collections.Generic;
using System.Text;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class SelectableProperty : BaseClass
    {
        public static List<SelectableProperty> Test { get; } = new List<SelectableProperty>
        {
            new SelectableProperty("Abc"),
            new SelectableProperty("Def"),
            new SelectableProperty("Ghi"),
            new SelectableProperty("Jkl")
        };

        public SelectableProperty(string content)
        {
            Content = content;
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }


        private string _Content;
        public string Content
        {
            get { return _Content; }
            set { _Content = value; OnPropertyChanged(nameof(Content)); }
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
