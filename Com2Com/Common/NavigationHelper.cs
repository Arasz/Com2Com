using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Page = System.Windows.Controls.Page;

namespace Com2Com.Common
{
    public static class NavigationHelper
    {
        private static Dictionary<string, Page> _registeredPages = new Dictionary<string, Page>();

        public static  Frame NavigationFrame { get; set; }

        public static void RegisterPage<T>(Page page)
        {
            _registeredPages[typeof(T).ToString()] = page;
        }

        public static void NavigateTo<T>()
        {
            string key = typeof(T).ToString();
            if (_registeredPages.ContainsKey(key))
                NavigationFrame.Navigate(_registeredPages[key]);
            else
                throw new Exception("Navigation error");
        }
    }
}
