using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Abp.RadzenUI.Models;

public class UpdateEmailSettingsVm
{
    [MaxLength(256)]
    [Display(Name = "SmtpHost")]
    public string SmtpHost { get; set; } = default!;

    [Range(1, 65535)]
    [Display(Name = "SmtpPort")]
    public int SmtpPort { get; set; }

    [MaxLength(1024)]
    [Display(Name = "SmtpUserName")]
    public string SmtpUserName { get; set; } = default!;

    [MaxLength(1024)]
    [DataType(DataType.Password)]
    [DisableAuditing]
    [Display(Name = "SmtpPassword")]
    public string SmtpPassword { get; set; } = default!;

    [MaxLength(1024)]
    [Display(Name = "SmtpDomain")]
    public string SmtpDomain { get; set; } = default!;

    [Display(Name = "SmtpEnableSsl")]
    public bool SmtpEnableSsl { get; set; }

    [Display(Name = "SmtpUseDefaultCredentials")]
    public bool SmtpUseDefaultCredentials { get; set; }

    [MaxLength(1024)]
    [Required]
    [Display(Name = "DefaultFromAddress")]
    public string DefaultFromAddress { get; set; } = default!;

    [MaxLength(1024)]
    [Required]
    [Display(Name = "DefaultFromDisplayName")]
    public string DefaultFromDisplayName { get; set; } = default!;
}

public class SendTestEmailVM
{
    [Required]
    public string SenderEmailAddress { get; set; } = default!;

    [Required]
    public string TargetEmailAddress { get; set; } = default!;

    [Required]
    public string Subject { get; set; } = default!;

    public string? Body { get; set; }
}
