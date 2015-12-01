using System;

// ReSharper disable CheckNamespace
namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class EqualSpecification : ISpecification
    {
        private readonly string propertyName;
        private readonly object value;

        public EqualSpecification(string propertyName, object value)
        {
            this.propertyName = propertyName;
            this.value = value;

            if (propertyName == null || value == null)
            {
                throw new ArgumentNullException();
            }
        }

        public string Result()
        {
            return $"[{this.propertyName}] = '{this.value}'";
        }
    }
}
