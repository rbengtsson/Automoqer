namespace Automoqer.Test.Model
{
    public class ServiceWithValueTypeInConstructor
    {
        private readonly int _intVal;

        public ServiceWithValueTypeInConstructor(int intVal)
        {
            _intVal = intVal;
        }

        public int GetVal()
        {
            return _intVal;
        }
    }
}
