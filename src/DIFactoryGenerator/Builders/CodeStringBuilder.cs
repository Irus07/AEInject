using System;
using System.Text;

namespace DIFactoryGenerator.Builders
{
    internal class CodeStringBuilder
    {
        private readonly StringBuilder sb;
        private string _tab;
        public int TabSize
        {
            get => TabSize;
            set
            {
                if (value < 0)
                    TabSize = 0;
                else
                    TabSize = value;

                for (int i = 0; i < TabSize; i++)
                    AddTab();
            }
        }

        public CodeStringBuilder()
        {
            sb = new StringBuilder();
        }

        public void AppendLine(string text) => sb.AppendLine(text);

        public void AppendLineWithTab(string text)
        {
            TabSize++;
            sb.AppendLine(text);
        }

        private void AddTab() => _tab += "\t";
    }
}
