namespace radio_button_mutiselect
{
    // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-group-windows-forms-radiobutton-controls-to-function-as-a-set
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();
    }
    class RadioButtonMulti : RadioButton
    {
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if(ModifierKeys.Equals(Keys.Control))
            {
                var parentB4 = Parent;
                try
                {
                    parentB4.Controls.Remove(this);
                    Checked = !Checked;
                }
                finally
                {
                    parentB4.Controls.Add(this);
                }
            }
            else
            {
                var others =
                    Parent.Controls
                    .Cast<Control>()
                    .Where(_ => _ is RadioButtonMulti)
                    .Where(_ => !ReferenceEquals(_, this));
                foreach (RadioButtonMulti other in others)
                {
                    other.Checked = false;
                }
                base.OnMouseDown(mevent);
            }
        }
        // Display checked buttons as title of group
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            var group =
                Parent.Controls
                .Cast<Control>()
                .Where(_ => _ is RadioButtonMulti)
                .Where(_ => ((RadioButtonMulti)_).Checked);
            Parent.Text = string.Join(", ", group.Select(_ => _.Text));
        }
    }
}