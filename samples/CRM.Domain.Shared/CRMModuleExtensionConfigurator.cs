using Abp.RadzenUI.ObjectExtending;
using CRM.Localization;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace CRM;

public static class CRMModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           AbpUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /* You can configure extra properties for the
         * entities defined in the modules used by your application.
         *
         * This class can be used to define these extra properties
         * with a high level, easy to use API.
         *
         * Example: Add a new property to the user entity of the identity module

           ObjectExtensionManager.Instance.Modules()
              .ConfigureIdentity(identity =>
              {
                  identity.ConfigureUser(user =>
                  {
                      user.AddOrUpdateProperty<string>( //property type: string
                          "SocialSecurityNumber", //property name
                          property =>
                          {
                              //validation rules
                              property.Attributes.Add(new RequiredAttribute());
                              property.Attributes.Add(new StringLengthAttribute(64) {MinimumLength = 4});

                              //...other configurations for this property
                          }
                      );
                  });
              });

         * See the documentation for more:
         * https://docs.abp.io/en/abp/latest/Module-Entity-Extensions
         */

        // 身份用户的扩展属性只在这里定义一次。ABP 会把它复制到 IdentityUser 实体以及
        // IdentityUserDto / IdentityUserCreateDto / IdentityUserUpdateDto / ProfileDto / UpdateProfileDto；
        // 没有调用 MapEfCoreProperty，所以值存放在用户表的 ExtraProperties JSON 列里，不需要迁移。
        //
        // ConfigureProfile 只决定该字段在 /account/manage 的 PersonalInfo tab 里的表现
        // （Hidden / ReadOnly / Editable），不写时走 AbpRadzenUIProfileOptions 的默认值 ReadOnly；
        // 管理员的创建/编辑弹窗仍由 UI.OnCreateForm / UI.OnEditForm 控制，
        // 用户列表的扩展列由 UI.OnTable.IsVisible 控制。
        // Required 等校验属性会被一起复制到 UpdateProfileDto，但库在启动时会把 ReadOnly / Hidden
        // 属性在 UpdateProfileDto 上的校验去掉，所以老用户即使部门为空也能正常保存个人资料。
        ObjectExtensionManager.Instance.Modules().ConfigureIdentity(identity =>
        {
            identity.ConfigureUser(user =>
            {
                // 部门：管理员建用户时必填；用户在个人中心只能看，不能改。
                user.AddOrUpdateProperty<string>(
                    "Department",
                    property =>
                    {
                        property.DisplayName = LocalizableString.Create<CRMResource>("DisplayName:Department");
                        property.Attributes.Add(new RequiredAttribute());
                        property.Attributes.Add(new StringLengthAttribute(64));
                        property.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.ReadOnly);
                    }
                );

                // 昵称：管理员可选填；用户在个人中心可以自己改；列表里不单独占一列。
                user.AddOrUpdateProperty<string>(
                    "Nickname",
                    property =>
                    {
                        property.DisplayName = LocalizableString.Create<CRMResource>("DisplayName:Nickname");
                        property.Attributes.Add(new StringLengthAttribute(32));
                        property.UI.OnTable.IsVisible = false;
                        property.ConfigureProfile(profile => profile.Mode = ProfileExtensionPropertyMode.Editable);
                    }
                );
            });
        });
    }
}
