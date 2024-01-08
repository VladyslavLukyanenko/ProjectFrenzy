using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ProjectFrenzy.AvaloniaUI.Views;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;
        private static readonly string BaseNamespace = typeof(MainWindowView).Namespace;

        public IControl Build(object data)
        {
            var name = data.GetType().Name.Replace("ViewModel", "View");
            var type = Type.GetType($"{BaseNamespace}.{name}");

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}