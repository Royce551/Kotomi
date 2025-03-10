using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Kotomi.ViewModels;
using System;

namespace Kotomi
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = $"Could not find page {name} :(" };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}