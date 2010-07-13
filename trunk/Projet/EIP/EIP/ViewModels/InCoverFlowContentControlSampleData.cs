using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace EIP.Showroom.ViewModels
{
    /// <summary>
    /// Sample data for the InCoverFlowContentControl sample
    /// </summary>
    public class InCoverFlowContentControlSampleData
    {
       
        public class Item
        {
            public string ColorName { get; set; }
            public Brush Brush { get; set; }
            public Brush HeaderBrush { get; set; }
        }

        private readonly List<Item> _items = new List<Item>();
        public List<Item> Items { get { return _items; } }

        public InCoverFlowContentControlSampleData()
        {
            foreach (var property in typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Where(p => p.PropertyType == typeof(Color) && p.Name != "Transparent"))
            {
                var name = property.Name;
                var color = (Color) property.GetValue(null, null);
                var bg = new SolidColorBrush(color);
                var headerBrush = new LinearGradientBrush(new GradientStopCollection
                {
                    new GradientStop{Offset=0, Color = Color.FromArgb(255,(byte) (color.R *0.5),(byte) (color.G *0.5),(byte) (color.B *0.5))},
                    new GradientStop{Offset=1, Color = Color.FromArgb(20,(byte) (color.R *0.5),(byte) (color.G *0.5),(byte) (color.B *0.5))}
                }, 0);

                _items.Add(new Item { Brush = bg, ColorName = name, HeaderBrush = headerBrush });

            }

        }
    }
}
