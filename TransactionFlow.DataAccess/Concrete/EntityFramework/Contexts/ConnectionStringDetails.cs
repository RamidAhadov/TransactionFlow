namespace TransactionFlow.DataAccess.Concrete.EntityFramework.Contexts;

public class ConnectionStringDetails
{
    public string Host { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}