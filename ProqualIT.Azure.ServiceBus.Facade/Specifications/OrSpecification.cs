// ReSharper disable CheckNamespace
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
            return $"({this.leftSpecification.Result()} OR {this.rightSpecification.Result()})";
        }
    }
}
