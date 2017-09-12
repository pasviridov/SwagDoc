using System;
using Microsoft.Office.Interop.Word;

namespace SwagDoc.Generating.MsWord
{
    public class MsWordDocument : IDisposable
    {
        private interface ILocation
        {
            MsWordDocument Owner { get; }

            Paragraph InsertParagraph();
        }

        public abstract class Location : ILocation
        {
            protected Location(MsWordDocument owner)
            {
                Owner = owner;
            }

            public virtual void Delete()
            {
                if (Owner.InsertLocation == this)
                {
                    Owner.InsertLocation = Owner.EndOfDocument;
                }
            }

            protected MsWordDocument Owner { get; }

            protected abstract Paragraph InsertParagraph();

            MsWordDocument ILocation.Owner => Owner;

            Paragraph ILocation.InsertParagraph() => InsertParagraph();
        }

        private class ParagraphLocation : Location
        {
            private readonly Paragraph _paragraph;

            public ParagraphLocation(MsWordDocument owner, Paragraph paragraph)
                : base(owner)
            {
                if (paragraph == null) throw new ArgumentNullException(nameof(paragraph));

                _paragraph = paragraph;
            }

            protected override Paragraph InsertParagraph()
            {
                object placeholder = _paragraph.Range;
                var paragraph = Owner.Document.Content.Paragraphs.Add(ref placeholder);
                paragraph.Range.InsertParagraphAfter();
                return paragraph;
            }

            public override void Delete()
            {
                object missing = System.Reflection.Missing.Value;
                _paragraph.Range.Delete(ref missing, ref missing);
                base.Delete();
            }
        }

        private class EndOfDocumentLocation : Location
        {
            public EndOfDocumentLocation(MsWordDocument owner) : base(owner) { }

            protected override Paragraph InsertParagraph()
            {
                Owner.Document.Content.InsertParagraphAfter();
                object missing = System.Reflection.Missing.Value;
                var paragraph = Owner.Document.Content.Paragraphs.Add(ref missing);
                return paragraph;
            }
        }

        private Document _document;
        private Location _insertLocation;

        internal MsWordDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            _document = document;

            InsertLocation = EndOfDocument = new EndOfDocumentLocation(this);
        }

        private Document Document
        {
            get
            {
                if (_document == null) throw new ObjectDisposedException(GetType().FullName);
                return _document;
            }
        }

        public Location InsertLocation
        {
            get
            {
                return _insertLocation;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (((ILocation)value).Owner != this) throw new ArgumentException("Location is in another document", nameof(value));
                _insertLocation = value;
            }
        }

        public Location EndOfDocument { get; }

        public bool Replace(string source, string target)
        {
            object findText = source;
            object replaceWith = target;
            object replace = WdReplace.wdReplaceAll;
            object missing = System.Reflection.Missing.Value;
            return Document.Content.Find.Execute(ref findText, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref replaceWith, ref replace, ref missing,
                ref missing, ref missing, ref missing);
        }

        public Location LocatePlaceholder(string placeholder)
        {
            if (string.IsNullOrEmpty(placeholder)) throw new ArgumentNullException(nameof(placeholder));

            for (int ii = 1; ii <= Document.Content.Paragraphs.Count; ii++)
            {
                var paragraph = Document.Content.Paragraphs[ii];
                if (paragraph.Range.Text.Contains(placeholder))
                {
                    if (paragraph.Range.Text.Trim() != placeholder)
                        throw new InvalidOperationException($"Placeholder {placeholder} should use an entire paragraph");

                    return new ParagraphLocation(this, paragraph);
                }
            }

            return null;
        }

        public MsWordDocument InsertParagraph(string text, string style = null)
        {
            var paragraph = InsertParagraph();

            paragraph.Range.Text = text.Replace("\r", "\v").Replace("\n", "");

            if (!string.IsNullOrEmpty(style))
            {
                object styleObj = style;
                paragraph.set_Style(ref styleObj);
            }

            return this;
        }

        public MsWordDocument InsertTable(string[,] values)
        {
            var paragraph = InsertParagraph();

            var rowCount = values.GetLength(0);
            var colCount = values.GetLength(1);

            object missing = System.Reflection.Missing.Value;
            Table table = Document.Tables.Add(paragraph.Range, rowCount, colCount, ref missing, ref missing);
            table.Borders.Enable = 1;

            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    cell.Range.Text = values[cell.RowIndex - 1, cell.ColumnIndex - 1];
                }
            }

            return this;
        }

        public void Save() => Document.Save();

        public void SaveAs(string filename) => Document.SaveAs2(filename);

        public void Dispose()
        {
            object missing = System.Reflection.Missing.Value;
            _document?.Close(ref missing, ref missing, ref missing);
            _document = null;
        }

        private Paragraph InsertParagraph() => ((ILocation)InsertLocation).InsertParagraph();
    }
}
