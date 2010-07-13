using System.Collections.Generic;

namespace EIP.Showroom.ViewModels
{
    /// <summary>
    /// Generate many items for the Virtualization sample
    /// </summary>
    public class VirtualizationSampleData
    {
        public VirtualizationSampleData()
        {
            for (int i = 0; i < 20000; i++)
            {
                _items.Add(new SampleDataItem { Value = "Item " + i });
            }
        }
        private readonly List<SampleDataItem> _items = new List<SampleDataItem>();
        public List<SampleDataItem> Items
        {
            get { return _items; }
        }
    }
}
