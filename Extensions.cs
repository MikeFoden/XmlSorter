// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace XmlSorter.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Extensions
    {
        public static string GetBrowserSafeUri(this Uri UriInstance)
        {
            return UriInstance.AbsoluteUri.Replace(":", "$").Replace("file$///", "file://127.0.0.1/").Replace("file$//", "file://");
        }
        public static void NavigateSafely(this WebBrowser WebBrowserInstance, string source)
        {
            WebBrowserInstance.Navigate(new Uri(source).GetBrowserSafeUri());
        }
        public static bool? ShowDialog(this Window WindowToBeShown, Window Owner)
        {
            WindowToBeShown.Owner = Owner;
            return WindowToBeShown.ShowDialog();
        }
        public static void Show(this Window WindowToBeShown, Window Owner)
        {
            WindowToBeShown.Owner = Owner;
            WindowToBeShown.Show();
        }
    }
}
