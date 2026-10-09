using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.Data;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.ObjectExtending;

public abstract class ExtensionPropertyComponentBase<TEntity, TResourceType> : AbpComponentBase
    where TEntity : IHasExtraProperties
{
    //[Inject]
    //public IStringLocalizerFactory StringLocalizerFactory { get; set; } = default!;

    [Inject]
    public IAbpEnumLocalizer AbpEnumLocalizer { get; set; } = default!;

    //[Inject]
    //public IValidationMessageLocalizerAttributeFinder ValidationMessageLocalizerAttributeFinder { get; set; } = default!;

    [Parameter]
    public TEntity Entity { get; set; } = default!;

    [Parameter]
    public ObjectExtensionPropertyInfo PropertyInfo { get; set; } = default!;

    [Parameter]
    public AbpBlazorMessageLocalizerHelper<TResourceType> LH { get; set; } = default!;

    /// <summary>
    /// Renders the input as read-only and skips required validation.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Bootstrap-style column size (1-12) of the label on medium+ screens.
    /// </summary>
    [Parameter]
    public int LabelSizeMD { get; set; } = 4;

    /// <summary>
    /// Bootstrap-style column size (1-12) of the input on medium+ screens.
    /// </summary>
    [Parameter]
    public int InputSizeMD { get; set; } = 8;

    /// <summary>
    /// Inline style applied to the input control. Defaults to filling the column,
    /// which suits dialogs; pass an empty string to keep Radzen's default width.
    /// </summary>
    [Parameter]
    public string InputStyle { get; set; } = "display: block; width: 100%;";

    /// <summary>
    /// Required mark and validator are only rendered when the field is editable.
    /// </summary>
    protected bool IsRequiredAndEditable => !ReadOnly && PropertyInfo.IsRequired();

    //[Parameter]
    //public ExtensionPropertyModalType? ModalType { get; set; }

    //protected virtual void Validate(ValidatorEventArgs e)
    //{
    //    e.Status = ValidationStatus.Success;

    //    var validationAttributes = PropertyInfo.GetValidationAttributes();
    //    var validationContext = new ValidationContext(Entity)
    //    {
    //        DisplayName = PropertyInfo.Name,
    //        MemberName = PropertyInfo.Name
    //    };

    //    foreach (var validationAttribute in validationAttributes)
    //    {
    //        var result = validationAttribute.GetValidationResult(e.Value, validationContext);
    //        if (result == ValidationResult.Success || result == null)
    //        {
    //            continue;
    //        }

    //        var errorMessage = result.ErrorMessage;
    //        if (LH != null)
    //        {
    //            var formattedErrorMessage = GetDefaultErrorMessage(validationAttribute);
    //            var errorMessageString = ValidationAttributeHelper.RevertErrorMessagePlaceholders(formattedErrorMessage);
    //            var errorMessageArguments = ValidationMessageLocalizerAttributeFinder.FindAll(errorMessage, errorMessageString)
    //                ?.OrderBy(x => x.Index)
    //                ?.Select(x => x.Argument);

    //            errorMessage = LH.Localize(errorMessageString, errorMessageArguments);
    //        }

    //        e.MemberNames = result.MemberNames;
    //        e.Status = ValidationStatus.Error;
    //        e.ErrorText = errorMessage;
    //        break;
    //    }
    //}

    //protected bool IsReadonlyField => ModalType is ExtensionPropertyModalType.EditModal && PropertyInfo.UI.EditModal.IsReadOnly;

    //private static string GetDefaultErrorMessage(ValidationAttribute validationAttribute)
    //{
    //    if (validationAttribute is StringLengthAttribute stringLengthAttribute && stringLengthAttribute.MinimumLength != 0)
    //    {
    //        var nullable = ValidationAttributeHelper.ValidationAttributeCustomErrorMessageSetProperty.GetValue((object)validationAttribute) as bool?;
    //        var flag = true;
    //        if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
    //        {
    //            return ValidationAttributeHelper.SetErrorMessagePlaceholders("The field {0} must be a string with a minimum length of {2} and a maximum length of {1}.");
    //        }
    //    }
    //    return ValidationAttributeHelper.SetErrorMessagePlaceholders(ValidationAttributeHelper.ValidationAttributeErrorMessageStringProperty.GetValue((object)validationAttribute) as string);
    //}
}
