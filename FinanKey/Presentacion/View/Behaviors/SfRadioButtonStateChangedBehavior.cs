using System.Windows.Input;
using Syncfusion.Maui.Buttons;

namespace FinanKey.View.Behaviors
{
    public class SfRadioButtonStateChangedBehavior : Behavior<SfRadioButton>
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(SfRadioButtonStateChangedBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(SfRadioButton radioButton)
        {
            base.OnAttachedTo(radioButton);
            radioButton.StateChanged += OnStateChanged;

            // importante: vincular el BindingContext
            radioButton.BindingContextChanged += OnBindingContextChanged;
        }

        protected override void OnDetachingFrom(SfRadioButton radioButton)
        {
            base.OnDetachingFrom(radioButton);
            radioButton.StateChanged -= OnStateChanged;
            radioButton.BindingContextChanged -= OnBindingContextChanged;
        }

        private void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (Command?.CanExecute(e.IsChecked) ?? false)
            {
                Command.Execute(e.IsChecked); // le paso solo el bool
            }
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (sender is BindableObject bindable)
            {
                BindingContext = bindable.BindingContext;
            }
        }
    }
}
