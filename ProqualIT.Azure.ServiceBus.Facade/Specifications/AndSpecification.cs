// ReSharper disable CheckNamespace

using System;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class AndSpecification : ISpecification
    {
        private readonly ISpecification leftSpecification;
        private readonly ISpecification rightSpecification;

        public AndSpecification(ISpecification left, ISpecification right)
        {
            this.leftSpecification = left;
            this.rightSpecification = right;
        }

        public string Result()
        {
            return String.Format("({0} And {1})", this.leftSpecification.Result(), this.rightSpecification.Result());
        }
    }
}
