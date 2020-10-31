using System;

namespace DicomCrawler.Helpers
{
    public static class ViewModelFactory
    {
        public static T CreateEmpty<T>() => Activator.CreateInstance<T>();

        public static T CreateCloned<T>(object parameter) => (T) Activator.CreateInstance(typeof(T), parameter);
    }
}
