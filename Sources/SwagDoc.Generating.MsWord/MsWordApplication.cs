using System;
using Microsoft.Office.Interop.Word;

namespace SwagDoc.Generating.MsWord
{
    public class MsWordApplication : IDisposable
    {
        private Application _application;

        public MsWordApplication(bool visible)
        {
            _application = new Application
            {
                ShowAnimation = false,
                Visible = visible
            };
        }

        private Application Application
        {
            get
            {
                if (_application == null) throw new ObjectDisposedException(GetType().FullName);
                return _application;
            }
        }

        public MsWordDocument CreateDocument()
        {
            object missing = System.Reflection.Missing.Value;
            return new MsWordDocument(Application.Documents.Add(ref missing, ref missing, ref missing, ref missing));
        }

        public MsWordDocument OpenDocument(string filename)
        {
            object filenameObj = filename;
            object missing = System.Reflection.Missing.Value;
            return new MsWordDocument(Application.Documents.Open(ref filenameObj, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing));
        }

        public void Dispose()
        {
            object saveOptionsObject = WdSaveOptions.wdDoNotSaveChanges;
            object missing = System.Reflection.Missing.Value;
            _application?.Quit(ref saveOptionsObject, ref missing, ref missing);
            _application = null;
        }
    }
}
