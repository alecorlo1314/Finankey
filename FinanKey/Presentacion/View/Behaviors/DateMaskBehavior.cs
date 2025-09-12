
namespace FinanKey.Presentacion.View.Behaviors
{
    public class DateMaskBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;

            var text = e.NewTextValue?.Replace("/", "") ?? "";

            // Solo permitir dígitos
            text = new string(text.Where(char.IsDigit).ToArray());

            // Limitar a 4 dígitos
            if (text.Length > 4)
                text = text.Substring(0, 4);

            // Agregar la barra automáticamente
            if (text.Length >= 2)
            {
          
                text = text.Insert(2, "/");
       
                    entry.CursorPosition = 4;
                
            }

            // Evitar bucle infinito
            if (entry.Text != text)
            {
                entry.Text = text;
            }
        }
    }
}
