using System;

namespace DicomCrawler.Helpers
{
    public static class ViewModelFactory
    {
        public static T Create<T>() => Activator.CreateInstance<T>();
    }
}
