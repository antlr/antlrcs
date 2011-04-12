/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.StringTemplate.Visualizer
{
    using System.Windows;
    using System.Windows.Controls;

    public static class TreeViewItemDisplay
    {
        public static readonly DependencyProperty BringSelectionIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "BringSelectionIntoView",
                typeof(bool),
                typeof(TreeViewItemDisplay),
                new UIPropertyMetadata(false, HandleBringSelectionIntoViewChanged));

        public static bool GetBringSelectionIntoView(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(BringSelectionIntoViewProperty);
        }

        public static void SetBringSelectionIntoView(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(BringSelectionIntoViewProperty, value);
        }

        private static void HandleBringSelectionIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = d as TreeViewItem;
            if (item == null)
                return;

            if (!(e.NewValue is bool))
                return;

            if ((bool)e.NewValue)
                item.Selected += HandleTreeViewItemSelected;
            else
                item.Selected -= HandleTreeViewItemSelected;
        }

        private static void HandleTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            // ignore notifications from ancestors
            if (!object.ReferenceEquals(sender, e.OriginalSource))
                return;

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
                item.BringIntoView();
        }
    }
}
