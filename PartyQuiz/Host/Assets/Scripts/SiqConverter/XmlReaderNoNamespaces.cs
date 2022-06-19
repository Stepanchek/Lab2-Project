using System.IO;
using System.Xml;

namespace PartyQuiz.Siq
{
    internal sealed class XmlReaderNoNamespaces : XmlTextReader
    {
        public override string Name => LocalName;
        public override string NamespaceURI => string.Empty;
        public override string Prefix => string.Empty;

        internal XmlReaderNoNamespaces(Stream stream) : base(stream)
        {
        }
    }
}