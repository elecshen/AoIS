using System.ServiceModel;

namespace MyCalcService.CalcService
{
    [ServiceContract]
    public interface ISoapService
    {
        [OperationContract]
        int Add(int intA, int intB);
        [OperationContract]
        int Subtract(int intA, int intB);
        [OperationContract]
        int Multiply(int intA, int intB);
        [OperationContract]
        int Divide(int intA, int intB);
    }

    public class SoapService : ISoapService
    {
        public int Add(int intA, int intB)
        {
            return intA + intB;
        }

        public int Divide(int intA, int intB)
        {
            return intA / intB;
        }

        public int Multiply(int intA, int intB)
        {
            return intA * intB;
        }

        public int Subtract(int intA, int intB)
        {
            return intA - intB;
        }
    }
}
