using System;
// ReSharper disable CheckNamespace

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class LikeSpecification : ISpecification
    {
        private readonly string propertyName;
        private readonly object value;

        public LikeSpecification(string propertyName, object value)
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
            return String.Format("[{0}] LIKE '%{1}%'", this.propertyName, this.value);
        }
    }
}
