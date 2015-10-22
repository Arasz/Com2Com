﻿using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Page = System.Windows.Controls.Page;

namespace Com2Com.Common
{
    /// <summary>
    /// Helps in between pages navigation ( implements some IoC functionalities )  
    /// </summary>
    public static class NavigationHelper
    {
        private static Dictionary<string, Page> _registeredPages = new Dictionary<string, Page>();

        //private static SimpleIoc _pagesIoc = new SimpleIoc();

        public static  Frame NavigationFrame { get; set; }

        public static void RegisterPage<T>(bool createInstantly = true) where T : Page
        {
            Type navigationType = typeof(T);
            if (createInstantly == true)
                _registeredPages[navigationType.ToString()] = (Page)Activator.CreateInstance(navigationType);
            else
                _registeredPages[navigationType.ToString()] = null;
        }

        public static void NavigateTo<T>() where T : Page
        {
            string key = typeof(T).ToString();
            if (_registeredPages.ContainsKey(key))
            {
                Page navigationPage = _registeredPages[key];
                if(navigationPage != null)
                    NavigationFrame.Navigate(_registeredPages[key]);
                else
                {
                    _registeredPages[key] = (Page)Activator.CreateInstance(Type.GetType(key));
                    NavigationFrame.Navigate(_registeredPages[key]);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Page isn't registered. Register page in MainWindow.cs");
            }
        }
    }
}
