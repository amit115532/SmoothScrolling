using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Smooth_Scrolling
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("test mouse processor")]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class SmoothScrollMouseProcessorProvider : IMouseProcessorProvider
    {
        IMouseProcessor IMouseProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            // זה פשוט אומר לעורך להשתמש במחלקה שיצרתי בתור מה שמנהל את הגלגלת של העכבר
            return new SmoothScrollMouseProcessor(wpfTextView);
        }
    }
}