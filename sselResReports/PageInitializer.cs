using LNF.Web;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(PageInitializer), "Initialize")]

public static class PageInitializer
{ 
    public static void Initialize()
    { 
        PageInitializerModule.Initialize();
    }
}