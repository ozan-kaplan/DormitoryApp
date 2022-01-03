using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.SearchModels
{
    public abstract class BaseSearchModel
    {
        private string _searchText; 
        public string SearchText
        {
            get { return _searchText.ToLower(); }
            set { _searchText = value; }
        }
    }
}