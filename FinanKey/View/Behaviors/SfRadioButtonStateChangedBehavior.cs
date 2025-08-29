using System.Windows.Input;
using Syncfusion.Maui.Buttons;

namespace FinanKey.View.Behaviors
{
    class SfRadioButtonStateChangedBehavior : Behavior<SfRadioButton>
    {
        public static readonly BindableProperty CommandProperty = 
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SfRadioButtonStateChangedBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        protected override void OnAttachedTo(SfRadioButton bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.StateChanged += OnStateChanged;
        }
        protected override void OnDetachingFrom(SfRadioButton bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.StateChanged -= OnStateChanged;
        }
        private void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if(Command?.CanExecute(e) ?? false)
            {
                Command.Execute(e);
            }
        }
    }
}
