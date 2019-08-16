using System.Threading.Tasks;
using ConsoleService;

public class Program
{
    public static Task Main()
    {
        return ServiceStarter.Start<MyService>();
    }
}