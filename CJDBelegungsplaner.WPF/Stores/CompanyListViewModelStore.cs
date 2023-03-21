using CJDBelegungsplaner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Stores
{
    public class CompanyListViewModelStore
    {
        private ObservableCollection<Company>? _companiesListParameter = null;
        public ObservableCollection<Company>? CompaniesListParameter
        {
            get 
            {
                if (_companiesListParameter is null)
                {
                    throw new System.Exception("Sollte nicht abgerufen werden können, wenn es nicht zuvor gefüllt wurde.");
                }

                ObservableCollection<Company>? temp = _companiesListParameter;

                _companiesListParameter = null;

                return temp;

            }
            set => _companiesListParameter = value;
        }
        public bool IsCompanyListParameter => _companiesListParameter is not null;

        public string? FilterName { get; set; } = string.Empty;
        public string? FilterStreet { get; set; } = string.Empty;
        public string? FilterCity { get; set; } = string.Empty;

        public void ClearFilter()
        {
            FilterName = string.Empty;
            FilterStreet = string.Empty;
            FilterCity = string.Empty;
        }
    }
}
