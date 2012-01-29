using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    internal class ReflectionAttributes : IReflectionAttributes
    {
        private Dictionary<string, string> attributes = new Dictionary<string, string>();

        #region IReflectionAttributes Members

        public bool SetAttribute(string attributeName, string attributeValue)
        {
            bool result = true;

            try
            {
                attributes[attributeName] = attributeValue;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public bool GetAttribute(string attributeName, out string attributeValue)
        {
            if (!attributes.ContainsKey(attributeName))
            {
                attributeValue = null;
                return false;
            }
            else
            {
                attributeValue = attributes[attributeName];
                return true;
            }
        }

        public void RemoveAttribute(string attributeName)
        {
            attributes.Remove(attributeName);
        }

        #endregion
    }
}
