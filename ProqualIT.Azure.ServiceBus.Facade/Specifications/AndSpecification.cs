// ReSharper disable CheckNamespace
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
            return $"({this.leftSpecification.Result()} And {this.rightSpecification.Result()})";
        }
    }
}
