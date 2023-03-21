using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using System;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels.Filter
{
    public partial class CompanyListViewModel : ViewModelBase
    {

        public class CompanyListFilter : FilterBase
        {
            private CompanyListViewModelStore _companyListViewModelStore;

            /// <summary>
            /// Binding Properties
            /// </summary>
            public string? Name
            {
                get => _companyListViewModelStore.FilterName;
                set
                {
                    _companyListViewModelStore.FilterName = value;
                    OnPropertyChangedAndDelayRefresh(nameof(Name));
                }
            }
            public string? Street
            {
                get => _companyListViewModelStore.FilterStreet;
                set
                {
                    _companyListViewModelStore.FilterStreet = value;
                    OnPropertyChangedAndDelayRefresh(nameof(Street));
                }
            }
            public string? City
            {
                get => _companyListViewModelStore.FilterCity;
                set
                {
                    _companyListViewModelStore.FilterCity = value;
                    OnPropertyChangedAndDelayRefresh(nameof(City));
                }
            }

            public CompanyListFilter(CompanyListViewModelStore companyListViewModelStore, Action refresh) : base(refresh)
            {
                _companyListViewModelStore= companyListViewModelStore;
                InitializeIsActive();
            }

            ///Filterlogic
            
            public void OnFilter(object sender, FilterEventArgs e)
            {
                var row = e.Item as Company;

                if(row is null)
                {
                    return;
                }

                bool isName = true;
                bool isCity = true;
                bool isStreet = true;

                if (!string.IsNullOrEmpty(Name))
                {
                    isName = row.Name.Contains(Name, StringComparison.OrdinalIgnoreCase);
                }
                if (!string.IsNullOrEmpty(City))
                {
                    isCity = row.Address.City.Contains(Street, StringComparison.OrdinalIgnoreCase);
                }
                if (!string.IsNullOrEmpty(Street))
                {
                    isName = row.Address.Street.Contains(Street, StringComparison.OrdinalIgnoreCase);
                }

                e.Accepted = isName && isCity && isStreet;

            }

        }
    }
}
