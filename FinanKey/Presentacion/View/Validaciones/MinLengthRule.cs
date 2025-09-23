
using FinanKey.Presentacion.Intefaces;

namespace FinanKey.Presentacion.View.Validaciones
{
    class MinLengthRule<T> : IValidationRule<T>
    {
        public int MinLength { get; set; }
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value is string str)
            {
                return str.Length >= MinLength;
            }
            return false;
        }
    }
}
