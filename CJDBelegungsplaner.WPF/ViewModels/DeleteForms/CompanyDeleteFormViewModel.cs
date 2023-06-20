using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms
{
    internal class CompanyDeleteFormViewModel : DeleteFormBase<Company>
    {
        private readonly ICompanyDataService _companyDataService;
        private readonly IHandleResultService _handleResultService;
        private readonly IAccountService _accountService;

        public CompanyDeleteFormViewModel(
            IHandleResultService handleResultService,
            ICompanyDataService companyDataService,
            IAccountService accountService)
        {
            _companyDataService = companyDataService;
            _handleResultService = handleResultService;
            _accountService = accountService;
        }
        public override async Task<bool> ExecuteDeleteAsync()
        {
            if (DeleteEntity is null)
            {
                throw new NullReferenceException(nameof(DeleteEntity));
            }
            bool isSuccess = false;

            Result result = await Task.Run(() => _companyDataService.DeleteAsync(DeleteEntity.Id));

            bool isFailure = _handleResultService.Handle(result);

            if (isFailure)
            {
                return isFailure;
            }

            _accountService.MakeUserLogEntry($"Firma '{DeleteEntity.Name} gelöscht.");

            return isSuccess;
        }
    }
}
