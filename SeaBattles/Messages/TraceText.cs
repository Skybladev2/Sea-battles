using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles.Messages
{
    internal class TraceText
    {
        private string text;

        public string Text
        {
            get { return text; }
            //set { text = value; }
        }

        public TraceText(string text)
        {
            this.text = text;
        }
    }
}
