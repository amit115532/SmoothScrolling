using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace SmoothScrollingExtension
{
    /// <summary>
    /// Used to define our mouse processor overrider
    /// </summary>
    [Export(typeof(IMouseProcessorProvider))]
    [Name("test mouse processor")]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class SmoothScrollMouseProcessorProvider : IMouseProcessorProvider
    {
        /// <summary>
        /// This function returns the mouse processor overrider
        /// </summary>
        IMouseProcessor IMouseProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new SmoothScrollMouseProcessor(wpfTextView);
        }
    }
}