using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using Prism.Regions;

namespace DicomReader.WPF.RegionAdapters
{
    public class TabControlAdapter : RegionAdapterBase<TabControl>
    {
        public TabControlAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (UserControl item in e.NewItems)
                    {
                        regionTarget.Items.Add(new TabItem
                        {
                            Header = item.Name,
                            Content = item
                        });
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        var toDelete = regionTarget.Items.OfType<TabItem>().FirstOrDefault(i => i.Content == item);
                        if (toDelete != null)
                        {
                            regionTarget.Items.Remove(toDelete);
                        }
                    }
                }
            };
        }

        protected override IRegion CreateRegion() => new SingleActiveRegion();
    }
}
