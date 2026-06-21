namespace CRM.Blazor.Web.Menus;

public class CRMMenus
{
    private const string Prefix = "CRM";
    public const string Home = Prefix + ".Home";
    public const string Dashboard = Prefix + ".Dashboard";

    //Add your menu items here...
    public const string Product = Prefix + ".Product";
    public const string ProductList = Product + ".List";

    public const string Sales = Prefix + ".Sales";
    public const string SalesWorkspace = Sales + ".Workspace";

    public const string Operations = Prefix + ".Operations";
    public const string OperationsDashboard = Operations + ".Dashboard";
    public const string WorkOrders = Operations + ".WorkOrders";
    public const string WorkOrderBoard = Operations + ".Board";
    public const string OperationShifts = Operations + ".Shifts";
    public const string OperationAssets = Operations + ".Assets";
}
