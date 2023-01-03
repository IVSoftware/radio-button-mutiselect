The nice thing about Winforms is that you can almost always make a custom version of a standard control with the functionality you want. The trick is to try and do it in a way that doesn't upend the idiomatic behavior to the point that your user is frustrated when their expectations are thwarted in terms of how the UI should be behaving.

I would argue that it's "intuitive enough" that when a multiselection is intended that holding down the [Control] modifier key is an accepted way to access that. So let's add multiselct capability that way.

Looking at the default behavior of a RadioButton as documented [here](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-group-windows-forms-radiobutton-controls-to-function-as-a-set).

> You group radio buttons by drawing them inside a container such as a Panel control, a GroupBox control, or a form. All radio buttons that are added directly to a form become one group. 

 This tells us what exactly what we need to do. _We need to temporarily make the clicked butting think that it doesn't belong to any group._  Heres one way that I _tested_ that seems to work.

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
    }




