The nice thing about Winforms is that you can almost always inherit a standard control to make a custom version with the functionality you want. The trick is to try and do it in a way that doesn't upend the expected behavior to the point that your user is frustrated when the UI doesn't match their **mental model**<sup>1</sup> of how it should work.

That said, I would argue it's "intuitive enough" to hold down the [Control] modifier key as an accepted way to access multiselection when it's available, so that's the approach I'll take for a `RadioButtonMulti` class that can be swapped out in your designer file.

[![multiselect][1]][1] _Control + Click to multiselect._

***
Looking at the default behavior of a RadioButton as documented [here](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-group-windows-forms-radiobutton-controls-to-function-as-a-set).

> You group radio buttons by drawing them inside a container such as a Panel control, a GroupBox control, or a form. All radio buttons that are added directly to a form become one group. 

This tells us what exactly what we need to do! _We need to temporarily make the clicked button "think" that it doesn't belong to any group._  Here's one way that I [tested](https://github.com/IVSoftware/radio-button-mutiselect.git) that removes the control from its Parent.Controls collection while it toggles the `Checked` state.

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

Display selection in main form title bar for demonstration purposes only.

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if(Form.ActiveForm != null)
            {
                var group =
                    Parent.Controls
                    .Cast<Control>()
                    .Where(_ => _ is RadioButtonMulti)
                    .Where(_ => ((RadioButtonMulti)_).Checked);
                Form.ActiveForm.Text = string.Join(", ", group.Select(_ => _.Text));
            }
        }
    }
***
1. (See also [Mental Models and Computer Models](https://www.cs.cornell.edu/courses/cs5150/2019sp/slides/9-usability.pdf) page 11).


  [1]: https://i.stack.imgur.com/Ye5M8.png