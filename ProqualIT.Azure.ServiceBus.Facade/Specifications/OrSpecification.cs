// ReSharper disable CheckNamespace

using System;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class OrSpecification : ISpecification
    {
        private readonly ISpecification leftSpecification;
        private readonly ISpecification rightSpecification;

        public OrSpecification(ISpecification left, ISpecification right)
        {
            this.leftSpecification = left;
            this.rightSpecification = right;
        }

        public string Result()
        {
            return String.Format("({0} OR {1})", this.leftSpecification.Result(), this.rightSpecification.Result());
        }
    }
}
