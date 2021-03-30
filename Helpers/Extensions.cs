using System;
using System.Windows;
using System.Windows.Controls;

namespace XmlSorter.Helpers
{
    public static class Extensions
    {
        public static string GetBrowserSafeUri(this Uri uriInstance)
        {
            return uriInstance.AbsoluteUri.Replace(":", "$").
              Replace("file$///", "file://127.0.0.1/").
              Replace("file$//", "file://");
        }

        public static void NavigateSafely(this WebBrowser webBrowserInstance, string source)
        {
            webBrowserInstance.Navigate(new Uri(source).GetBrowserSafeUri());
        }

        public static bool? ShowDialog(this Window windowToBeShown, Window owner)
        {
            windowToBeShown.Owner = owner;
            return windowToBeShown.ShowDialog();
        }

        public static void Show(this Window windowToBeShown, Window owner)
        {
            windowToBeShown.Owner = owner;
            windowToBeShown.Show();
        }
    }
}
