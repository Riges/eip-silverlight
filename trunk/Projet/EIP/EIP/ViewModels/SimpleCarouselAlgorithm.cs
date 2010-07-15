using System;
using System.Windows;
//using SStuff.FlowControls;

namespace EIP.Showroom.ViewModels
{
    /// <summary>
    /// Carousel algorithm demonstrating extensibility
    /// </summary>
    /*public class SimpleCarouselAlgorithm : IFlowLayout3DAlgorithm
    {

        #region IFlowLayout3DAlgorithm Members
        Size _availableSize;
        public void SetAvailableSize(Size availableSize)
        {
            _availableSize = availableSize;
        }

        public int GetMaxDisplayedItems()
        {
            return 15;
        }

        public void ComputeLayout(double normalizedOffset, IFlowItem3D item, int consideredItemsCount)
        {
            var piFactor = normalizedOffset * Math.PI;

            item.Transform3D.GlobalOffsetX = _availableSize.Width * .6 * Math.Sin(piFactor);
            item.Transform3D.GlobalOffsetZ = _availableSize.Width * .6 * (Math.Cos(piFactor)-1);
            item.Transform3D.GlobalOffsetY = 0.3 * (Math.Cos(piFactor) - 1) * _availableSize.Height + 0.2 * _availableSize.Height;
        }

        public bool SupportsGestures
        {
            get { return false; }
        }

        public double GetNormalizedOffsetAtPoint(Point contactPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Raised when the algorithm want to invalidate the current layout
        /// </summary>
        public event EventHandler LayoutChanged;
        #endregion
    }*/
}
