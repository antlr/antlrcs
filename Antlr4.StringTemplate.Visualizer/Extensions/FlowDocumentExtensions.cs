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

namespace Antlr4.StringTemplate.Visualizer.Extensions
{
    using System;
    using System.Windows.Documents;

    internal static class FlowDocumentExtensions
    {
        public static TextPointer GetPointerFromCharOffset(this FlowDocument document, ref int charOffset)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (charOffset == 0)
                return document.ContentStart;

            return GetPointerFromCharOffset(document.ContentEnd, document.Blocks, ref charOffset);
        }

        public static int GetCharOffsetToPosition(this FlowDocument document, TextPointer position)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (position == null)
                throw new ArgumentNullException("position");

            if (position.CompareTo(document.ContentStart) == 0)
                return 0;

            TextPointer result;
            return GetCharOffsetToPosition(document.Blocks, position, out result);
        }

        private static TextPointer GetPointerFromCharOffset(TextPointer elementEnd, BlockCollection blockCollection, ref int charOffset)
        {
            foreach (var block in blockCollection)
            {
                TextPointer pointer = GetPointerFromCharOffset(block, ref charOffset);
                if (charOffset == 0)
                    return pointer;
            }

            return elementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(TextPointer elementEnd, InlineCollection inlineCollection, ref int charOffset)
        {
            foreach (var inline in inlineCollection)
            {
                TextPointer pointer = GetPointerFromCharOffset(inline, ref charOffset);
                if (charOffset == 0)
                    return pointer;
            }

            return elementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(TextPointer elementEnd, ListItemCollection listItemCollection, ref int charOffset)
        {
            foreach (var listItem in listItemCollection)
            {
                TextPointer pointer = GetPointerFromCharOffset(listItem, ref charOffset);
                if (charOffset == 0)
                    return pointer;
            }

            return elementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(Block currentBlock, ref int charOffset)
        {
            Paragraph paragraph = currentBlock as Paragraph;
            if (paragraph != null)
                return GetPointerFromCharOffset(paragraph, ref charOffset);

            List list = currentBlock as List;
            if (list != null)
                return GetPointerFromCharOffset(list, ref charOffset);

            Table table = currentBlock as Table;
            if (table != null)
                return GetPointerFromCharOffset(table, ref charOffset);

            Section section = currentBlock as Section;
            if (section != null)
                return GetPointerFromCharOffset(section, ref charOffset);

            throw new ArgumentException();
        }

        private static TextPointer GetPointerFromCharOffset(List list, ref int charOffset)
        {
            return GetPointerFromCharOffset(list.ElementEnd, list.ListItems, ref charOffset);
        }

        private static TextPointer GetPointerFromCharOffset(ListItem listItem, ref int charOffset)
        {
            return GetPointerFromCharOffset(listItem.ElementEnd, listItem.Blocks, ref charOffset);
        }

        private static TextPointer GetPointerFromCharOffset(Paragraph paragraph, ref int charOffset)
        {
            return GetPointerFromCharOffset(paragraph.ElementEnd, paragraph.Inlines, ref charOffset);
        }

        private static TextPointer GetPointerFromCharOffset(Section section, ref int charOffset)
        {
            return GetPointerFromCharOffset(section.ElementEnd, section.Blocks, ref charOffset);
        }

        private static TextPointer GetPointerFromCharOffset(Table table, ref int charOffset)
        {
            throw new NotImplementedException();
        }

        private static TextPointer GetPointerFromCharOffset(Inline currentInline, ref int charOffset)
        {
            Run run = currentInline as Run;
            if (run != null)
                return GetPointerFromCharOffset(run, ref charOffset);

            LineBreak lineBreak = currentInline as LineBreak;
            if (lineBreak != null)
                return GetPointerFromCharOffset(lineBreak, ref charOffset);

            Span span = currentInline as Span;
            if (span != null)
                return GetPointerFromCharOffset(span, ref charOffset);

            InlineUIContainer inlineUIContainer = currentInline as InlineUIContainer;
            if (inlineUIContainer != null)
                return GetPointerFromCharOffset(inlineUIContainer, ref charOffset);

            AnchoredBlock anchoredBlock = currentInline as AnchoredBlock;
            if (anchoredBlock != null)
                return GetPointerFromCharOffset(anchoredBlock, ref charOffset);

            throw new ArgumentException();
        }

        private static TextPointer GetPointerFromCharOffset(AnchoredBlock anchoredBlock, ref int charOffset)
        {
            return GetPointerFromCharOffset(anchoredBlock.ElementEnd, anchoredBlock.Blocks, ref charOffset);
        }

        private static TextPointer GetPointerFromCharOffset(InlineUIContainer inlineUIContainer, ref int charOffset)
        {
            return inlineUIContainer.ElementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(LineBreak lineBreak, ref int charOffset)
        {
            return lineBreak.ElementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(Run run, ref int charOffset)
        {
            if (run.Text.Length >= charOffset)
            {
                TextPointer pointer = run.ContentStart.GetPositionAtOffset(charOffset);
                charOffset = 0;
                return pointer;
            }

            charOffset -= run.Text.Length;
            return run.ElementEnd;
        }

        private static TextPointer GetPointerFromCharOffset(Span span, ref int charOffset)
        {
            return GetPointerFromCharOffset(span.ElementEnd, span.Inlines, ref charOffset);
        }

        private static int GetCharOffsetToPosition(BlockCollection blockCollection, TextPointer position, out TextPointer result)
        {
            int offset = 0;
            foreach (var block in blockCollection)
            {
                offset += GetCharOffsetToPosition(block, position, out result);
                if (result == null || result.CompareTo(position) >= 0)
                    return offset;
            }

            result = null;
            return offset;
        }

        private static int GetCharOffsetToPosition(InlineCollection inlineCollection, TextPointer position, out TextPointer result)
        {
            int offset = 0;
            foreach (var inline in inlineCollection)
            {
                offset += GetCharOffsetToPosition(inline, position, out result);
                if (result == null || result.CompareTo(position) >= 0)
                    return offset;
            }

            result = null;
            return offset;
        }

        private static int GetCharOffsetToPosition(ListItemCollection listItemCollection, TextPointer position, out TextPointer result)
        {
            int offset = 0;
            foreach (var listItem in listItemCollection)
            {
                offset += GetCharOffsetToPosition(listItem, position, out result);
                if (result == null || result.CompareTo(position) >= 0)
                    return offset;
            }

            result = null;
            return offset;
        }

        private static int GetCharOffsetToPosition(Block currentBlock, TextPointer position, out TextPointer result)
        {
            Paragraph paragraph = currentBlock as Paragraph;
            if (paragraph != null)
                return GetCharOffsetToPosition(paragraph, position, out result);

            List list = currentBlock as List;
            if (list != null)
                return GetCharOffsetToPosition(list, position, out result);

            Table table = currentBlock as Table;
            if (table != null)
                return GetCharOffsetToPosition(table, position, out result);

            Section section = currentBlock as Section;
            if (section != null)
                return GetCharOffsetToPosition(section, position, out result);

            throw new ArgumentException();
        }

        private static int GetCharOffsetToPosition(List list, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(list.ListItems, position, out result);
        }

        private static int GetCharOffsetToPosition(ListItem listItem, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(listItem.Blocks, position, out result);
        }

        private static int GetCharOffsetToPosition(Paragraph paragraph, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(paragraph.Inlines, position, out result);
        }

        private static int GetCharOffsetToPosition(Section section, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(section.Blocks, position, out result);
        }

        private static int GetCharOffsetToPosition(Table table, TextPointer position, out TextPointer result)
        {
            throw new NotImplementedException();
        }

        private static int GetCharOffsetToPosition(Inline currentInline, TextPointer position, out TextPointer result)
        {
            Run run = currentInline as Run;
            if (run != null)
                return GetCharOffsetToPosition(run, position, out result);

            LineBreak lineBreak = currentInline as LineBreak;
            if (lineBreak != null)
                return GetCharOffsetToPosition(lineBreak, position, out result);

            Span span = currentInline as Span;
            if (span != null)
                return GetCharOffsetToPosition(span, position, out result);

            InlineUIContainer inlineUIContainer = currentInline as InlineUIContainer;
            if (inlineUIContainer != null)
                return GetCharOffsetToPosition(inlineUIContainer, position, out result);

            AnchoredBlock anchoredBlock = currentInline as AnchoredBlock;
            if (anchoredBlock != null)
                return GetCharOffsetToPosition(anchoredBlock, position, out result);

            throw new ArgumentException();
        }

        private static int GetCharOffsetToPosition(AnchoredBlock anchoredBlock, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(anchoredBlock.Blocks, position, out result);
        }

        private static int GetCharOffsetToPosition(InlineUIContainer inlineUIContainer, TextPointer position, out TextPointer result)
        {
            result = inlineUIContainer.ElementEnd;
            return 0;
        }

        private static int GetCharOffsetToPosition(LineBreak lineBreak, TextPointer position, out TextPointer result)
        {
            result = lineBreak.ElementEnd;
            return 0;
        }

        private static int GetCharOffsetToPosition(Run run, TextPointer position, out TextPointer result)
        {
            if (run.ContentEnd.CompareTo(position) >= 0)
            {
                result = position;
                return run.ContentStart.GetOffsetToPosition(position);
            }

            result = run.ElementEnd;
            return run.Text.Length;
        }

        private static int GetCharOffsetToPosition(Span span, TextPointer position, out TextPointer result)
        {
            return GetCharOffsetToPosition(span.Inlines, position, out result);
        }
    }
}
