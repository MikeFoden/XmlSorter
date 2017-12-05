using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using IOPath = System.IO.Path;
using XmlSorter.DataObjects;
using XmlSorter.Helpers;

namespace XmlSorter
{
    /// <summary>
    /// Startup and main window of the application
    /// </summary>
    public partial class WindowMain : Window
    {
        #region Fields

        string TempSourceFilePath;
        string TempTargetFilePath;
        string OriginalSourceFilePath;
        OpenFileDialog OpenFileDialogInstance;
        SaveFileDialog SaveFileDialogInstance;
        volatile bool IsAttributesBeingRead;
        bool ClearRigthBrowser;
        WindowAttributesSelection WindowAttributesSelectionInstance;
        AttributesBinding AttributesBindingInstance = new AttributesBinding();
        IEnumerable<string> FilteredSortingAttibutes;

        #endregion

        #region Constructors

        public WindowMain()
        {
            InitializeComponent();
        }

        #endregion

        #region Events' Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonSourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            if(ShowOpenDialog())
            {
                TextBlockSouurcePath.Text = OpenFileDialogInstance.FileName;
                OriginalSourceFilePath = OpenFileDialogInstance.FileName;
                TempSourceFilePath = IOPath.Combine(IOPath.GetTempPath(), "Source.xml");
                TempTargetFilePath = IOPath.Combine(IOPath.GetTempPath(), "Target.xml");
                MaintainControlsAvailability();
                ClearRigthBrowser = true;
                WebBrowserAfter.NavigateToString("<HTML></HTML>");
                new Thread(ReadAttributes).Start();
            }
        }

        private void OpenFileDialogInstance_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !ValidateXml(OpenFileDialogInstance.FileName);
        }

        private void CheckBoxOverwriteSourceFile_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonTargetBrowse_Click(object sender, RoutedEventArgs e)
        {
            if(ShowSaveDialog())
            {
                TextBlockTargetPath.Text = SaveFileDialogInstance.FileName;
                MaintainControlsAvailability();
            }
        }

        private void CheckBoxSortBySpecificAttributes_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void CheckBoxSortByTagName_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void CheckBoxSortAttributes_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonSelectAttributes_Click(object sender, RoutedEventArgs e)
        {
            SelectAttributes();
        }

        private void ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            FilteredSortingAttibutes = AttributesBindingInstance.GetSelected();
            XElement xe = XElement.Load(TempSourceFilePath);
            SortElement(xe);
            xe.Save(TempTargetFilePath);
            WebBrowserAfter.NavigateSafely(TempTargetFilePath);
        }

        private void WebBrowserBefore_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = e.Uri.GetBrowserSafeUri().ToLower() != new Uri(TempSourceFilePath).GetBrowserSafeUri().ToLower();
        }

        private void WebBrowserAfter_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if(ClearRigthBrowser)
            {
                ClearRigthBrowser = false;
            }
            else
            {
                e.Cancel = e.Uri.GetBrowserSafeUri().ToLower() != new Uri(TempTargetFilePath).GetBrowserSafeUri().ToLower();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Copy(TempTargetFilePath, TextBlockTargetPath.Text, true);
        }
       
        #endregion

        #region Helper Functions

        private void SortElement(XElement xe)
        {
            IEnumerable<XNode> NodesToBePreserved = xe.Nodes().Where(P => P.GetType() != typeof(XElement));
            if(CheckBoxSortAttributes.IsChecked.Value)
            {
                xe.ReplaceAttributes(xe.Attributes().OrderBy(x => x.Name.LocalName));
            }
            if(!CheckBoxSortBySpecificAttributes.IsChecked.Value || FilteredSortingAttibutes.Count() == 0)
            {
                xe.ReplaceNodes((xe.Elements().OrderBy(x => x.Name.LocalName).Union((NodesToBePreserved).OrderBy(P => P.ToString()))).OrderBy(N => N.NodeType.ToString()));
            }
            else
            {
                foreach(string Att in FilteredSortingAttibutes)
                {
                    xe.ReplaceNodes((xe.Elements().OrderBy(x => x.Name.LocalName).ThenBy(x => (string)x.Attribute(Att)).Union((NodesToBePreserved).OrderBy(P => P.ToString()))).OrderBy(N => N.NodeType.ToString()));
                }
            }
            foreach(XElement xc in xe.Elements())
            {
                SortElement(xc);
            }
        }

        private void ReadAttributes()
        {
            IsAttributesBeingRead = true;
            if(AttributesBindingInstance != null)
            {
                Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        AttributesBindingInstance.Clear();
                    }));
            }
            XElement xe = XElement.Load(OriginalSourceFilePath);
            xe.Save(TempSourceFilePath);
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    WebBrowserBefore.NavigateSafely(TempSourceFilePath);
                }));
            XmlDocument doc = new XmlDocument();
            doc.Load(TempSourceFilePath);
            string stringXPath = "//@*";
            foreach(XmlAttribute att in doc.SelectNodes(stringXPath))
            {
                AttributesBindingInstance.Add(att.Name);
            }
            IsAttributesBeingRead = false;
        }

        private void SelectAttributes()
        {
            while(IsAttributesBeingRead)
            {

            }
            if(WindowAttributesSelectionInstance == null)
            {
                WindowAttributesSelectionInstance = new WindowAttributesSelection(AttributesBindingInstance);
                WindowAttributesSelectionInstance.IsVisibleChanged += new DependencyPropertyChangedEventHandler(delegate
                    {
                        MaintainControlsAvailability();
                    });
            }
            WindowAttributesSelectionInstance.Show(this);
            MaintainControlsAvailability();
        }

        private bool ShowOpenDialog()
        {
            if(OpenFileDialogInstance == null)
            {
                OpenFileDialogInstance = new OpenFileDialog();
                OpenFileDialogInstance.FileOk += new System.ComponentModel.CancelEventHandler(OpenFileDialogInstance_FileOk);
            }
            bool? Result = OpenFileDialogInstance.ShowDialog();
            return Result.HasValue && Result.Value;
        }

        private bool ShowSaveDialog()
        {
            if(SaveFileDialogInstance == null)
            {
                SaveFileDialogInstance = new SaveFileDialog();
            }
            bool? Result = SaveFileDialogInstance.ShowDialog();
            return Result.HasValue && Result.Value;
        }

        private void MaintainControlsAvailability()
        {
            if(!IsLoaded)
            {
                return;
            }
            GroupBoxTarget.IsEnabled = TextBlockSouurcePath.Text.Length > 0;

            LabelTargetPath.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            TextBlockTargetPath.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            ButtonTargetBrowse.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            TextBlockTargetPath.Text = (CheckBoxOverwriteSourceFile.IsChecked.Value) ? TextBlockSouurcePath.Text : (SaveFileDialogInstance == null) ? string.Empty : SaveFileDialogInstance.FileName;

            GroupBoxOptions.IsEnabled = GroupBoxTarget.IsEnabled;
            GroupBoxPreview.IsEnabled = GroupBoxTarget.IsEnabled;
            ButtonSelectAttributes.IsEnabled = CheckBoxSortBySpecificAttributes.IsChecked.Value && (WindowAttributesSelectionInstance == null || !WindowAttributesSelectionInstance.IsVisible);
            GroupBoxActions.IsEnabled = TextBlockSouurcePath.Text.Length > 0;
            ButtonSort.IsEnabled = CheckBoxSortByTagName.IsChecked.Value || CheckBoxSortAttributes.IsChecked.Value || CheckBoxSortBySpecificAttributes.IsChecked.Value;
            ButtonSave.IsEnabled = TextBlockTargetPath.Text.Length > 0;
            if(!CheckBoxSortBySpecificAttributes.IsChecked.Value && WindowAttributesSelectionInstance != null && WindowAttributesSelectionInstance.IsVisible)
            {
                WindowAttributesSelectionInstance.Hide();
            }
        }

        private bool ValidateXml(string FilePath)
        {
            try
            {
                new XmlDocument().Load(FilePath);
                return true;
            }
            catch
            {
                MessageBox.Show("Invalid xml file");
                return false;
            }
        }

        #endregion
    }
}
