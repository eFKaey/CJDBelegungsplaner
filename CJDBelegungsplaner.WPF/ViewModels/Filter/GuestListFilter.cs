using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Filter;
using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels;

public partial class GuestListViewModel : ViewModelBase
{

    /// <summary>
    /// Stellt dem DataGrid der View die Attribute, Felder und Logik für die Filterung zur Verfügung.
    /// </summary>
    public class GuestListFilter : FilterBase
    {
        private GuestListViewModelStore _guestListViewModelStore;

        public List<string> ClassNames { get; private set; }
        private string _showGuestsWithAnyClassName = "(Gäste mit Klasse)";
        private string _showGuestsWithoutClassName = "(Gäste ohne Klasse)";

        public List<string> CompanyNames { get; private set; }

        #region Binding Properties

        /// 
        /// Hinweis: Alle Properties, welche Eingabefelder betreffen, müssen vom Type string? (nullable) sein.
        ///          Außerdem darf es keine andern public Properties vom typ string geben, weil momentan anhand
        ///          des Types erkannt wird, dass es Eingabefelder sind, welche entsprechend überprüft werden können.
        /// 

        public string? FirstName
        {
            get => _guestListViewModelStore.FilterFirstName;
            set
            {
                _guestListViewModelStore.FilterFirstName = value;
                OnPropertyChangedAndDelayRefresh(nameof(FirstName));
            }
        }

        public string? LastName
        {
            get => _guestListViewModelStore.FilterLastName;
            set
            {
                _guestListViewModelStore.FilterLastName = value;
                OnPropertyChangedAndDelayRefresh(nameof(LastName));
            }
        }

        public string? BirthdateFrom
        {
            get => _guestListViewModelStore.FilterBirthdateFrom;
            set
            {
                _guestListViewModelStore.FilterBirthdateFrom = value;
                OnPropertyChangedAndDelayRefresh(nameof(BirthdateFrom));
            }
        }
        public string? BirthdateTo
        {
            get => _guestListViewModelStore.FilterBirthdateTo;
            set
            {
                _guestListViewModelStore.FilterBirthdateTo = value;
                OnPropertyChangedAndDelayRefresh(nameof(BirthdateTo));
            }
        }

        public string? ClassName
        {
            get => _guestListViewModelStore.FilterClassName;
            set
            {
                _guestListViewModelStore.FilterClassName = value;
                OnPropertyChangedAndDelayRefresh(nameof(ClassName));
            }
        }

        public string? CompanyName
        {
            get => _guestListViewModelStore.FilterCompanyName;
            set
            {
                _guestListViewModelStore.FilterCompanyName = value;
                OnPropertyChangedAndDelayRefresh(nameof(CompanyName));
            }
        }

        #endregion

        /// <summary>
        /// Instanziiert ein Object der Klasse <see cref="GuestListFilter"/>.
        /// </summary>
        /// <param name="guestListViewModelStore">Ein Store, der den State der Properites speichert.</param>
        /// <param name="refresh">Die 'Refresh' Event-Methode von der '<see cref="CollectionViewSource"/>.View'.</param>
        /// <param name="classNames">Eine Liste der Klassennamen für die ComboBox.</param>
        public GuestListFilter(GuestListViewModelStore guestListViewModelStore, Action refresh, List<string> classNames, List<string> companyNames)
            : base(refresh)
        {
            _guestListViewModelStore = guestListViewModelStore;

            ClassNames = classNames;
            ClassNames.Insert(0, _showGuestsWithoutClassName);
            ClassNames.Insert(0, _showGuestsWithAnyClassName);

            CompanyNames = companyNames;

            InitializeIsActive(); // Muss immer am Ende des Konstruktors sein!
        }

        /// <summary>
        /// Beinhaltet die Filterlogik.
        /// <para>
        /// Funktionsweise: <br/>
        /// Muss mit der Filter-Property der CollectionViewSource registriert werden. <br/>
        /// Und Sollte mit Dispose() wieder ausgetragen werden.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnFilter(object sender, FilterEventArgs e)
        {
            var row = e.Item as Guest;

            if (row is null)
            {
                return;
            }

            DateTime birthdateFrom;
            DateTime birthdateTo;
            bool isFirstName = true;
            bool isLastName = true;
            bool isBirthdateFrom = true;
            bool isBirthdateTo = true;
            bool isClassName = true;
            bool isCompanyName = true;

            if (!string.IsNullOrEmpty(FirstName))
            {
                isFirstName = row.FirstName.Contains(FirstName, StringComparison.OrdinalIgnoreCase);
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                isLastName = row.LastName.Contains(LastName, StringComparison.OrdinalIgnoreCase);
            }
            if (!string.IsNullOrEmpty(BirthdateFrom)
                && DateTime.TryParse(BirthdateFrom, out birthdateFrom))
            {
                isBirthdateFrom = birthdateFrom <= row.Birthdate;
            }
            if (!string.IsNullOrEmpty(BirthdateTo)
                && DateTime.TryParse(BirthdateTo, out birthdateTo))
            {
                isBirthdateTo = row.Birthdate <= birthdateTo;
            }
            if (!string.IsNullOrEmpty(ClassName))
            {

                if (row.Class is null)
                {
                    if (ClassName != _showGuestsWithoutClassName)
                    {
                        isClassName = false;
                    }
                }

                else if (ClassName == _showGuestsWithAnyClassName)
                {
                    if (string.IsNullOrEmpty(row.Class.Name))
                    {
                        isClassName = false;
                    }
                }

                else
                {
                    isClassName = row.Class.Name.Contains(ClassName, StringComparison.OrdinalIgnoreCase);
                }
            }
            if (!string.IsNullOrEmpty(CompanyName))
            {
                if (row.Company is null)
                {
                    isCompanyName = false;
                }
                else
                {
                    isCompanyName = row.Company.Name.Contains(CompanyName, StringComparison.OrdinalIgnoreCase);
                }
            }

            e.Accepted = isFirstName && isLastName && isBirthdateFrom && isBirthdateTo && isClassName && isCompanyName;
        }

    }
}
