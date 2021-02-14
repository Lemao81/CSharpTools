using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DicomReader.WPF.Interfaces;
using Newtonsoft.Json;

namespace DicomReader.WPF.Extensions
{
    public static class CommonExtensions
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || !enumerable.Any();

        public static string AsJson(this object obj) => JsonConvert.SerializeObject(obj);

        public static T Deserialize<T>(this string str) where T : IDto => JsonConvert.DeserializeObject<T>(str);

        public static string AsIndentedJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

        public static bool GetIsFocused(DependencyObject obj) => (bool) obj.GetValue(IsFocusedProperty);

        public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(CommonExtensions),
            new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var uiElement = (UIElement) depObject;
            if ((bool) eventArgs.NewValue)
            {
                uiElement.Focus();
            }
        }
    }
}
