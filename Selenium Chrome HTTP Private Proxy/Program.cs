using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium_Chrome_HTTP_Private_Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            Tuple<bool,string> result = ExtensionHelper.CreateExtension("user", "pass", "ip:port");
            if(result.Item1)
            {               
                options.AddExtension(result.Item2);
                var driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl("http://httpbin.org/ip");
            }
            else
            {
                Console.WriteLine(result.Item2);
                Console.ReadKey();
            }
           
        }
    }
    
        
}
