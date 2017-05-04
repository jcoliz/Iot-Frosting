using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Common
{
    /// <summary>
    /// IoT doesn't have Windows.UI.Popups, so we need this
    /// </summary>
    public class MessageDialog: ContentDialog
    {
        public MessageDialog(string message, string title)
        {
            Title = title;
            Content = new TextBlock() { Text = message };
            PrimaryButtonText = "OK";
            IsPrimaryButtonEnabled = true;
        }
    }
}
