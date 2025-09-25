using System;

namespace WBBEntity.Extensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PostgresqlCustomTypeMappingAttribute : Attribute

    {
        private string text;
        public PostgresqlCustomTypeMappingAttribute(string text)
        {
            this.Text = text;

        }
        public string Text

        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }
    }
}