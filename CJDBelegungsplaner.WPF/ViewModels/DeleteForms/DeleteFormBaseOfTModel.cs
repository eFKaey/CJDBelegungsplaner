using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels.Forms;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public abstract partial class DeleteFormBase<TModel> : DeleteFormBase
    where TModel : EntityObject
{
    public virtual TModel? DeleteEntity { get; set; }
}
