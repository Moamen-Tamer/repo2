using System.Drawing;
using System.Windows.Forms;

namespace HospitalApp
{
    public static class UIHelper
    {
        // textbox settings
        public static TextBox MakeInput(string placeholder = "")
        {
            TextBox txt = new TextBox();
            txt.Font = Theme.FontBody;
            txt.BackColor = Theme.Input;
            txt.ForeColor = Theme.TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Dock = DockStyle.Fill;
            if (placeholder != "") txt.PlaceholderText = placeholder;

            return txt;
        }

        // button settings
        public static Button MakeButton(string text, Color? color = null)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = Theme.FontSubhead;
            btn.BackColor = color ?? Theme.Accent;
            btn.ForeColor = color == null ? Theme.Background : Theme.TextPrimary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = color == null ? Theme.AccentDeep : ControlPaint.Dark(color.Value, 0.1f);
            btn.Dock = DockStyle.Fill;
            btn.Cursor = Cursors.Hand;
            
            return btn;
        }

        // label settings
        public static Label MakeLabel(string text)
        {
            Label lbl = new Label();
            lbl.Text = text.ToUpper();
            lbl.Font = Theme.FontLabel;
            lbl.ForeColor = Theme.Accent;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.BottomLeft;

            return lbl;
        }

        // text settings
        public static Label MakeText(string text, int fontSize = 10)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", fontSize);
            lbl.ForeColor = Theme.TextPrimary;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;

            return lbl;
        }

        // badge settings
        public static Label MakeBadge(string status) {
            Label lbl      = new Label();
            lbl.Text       = status;
            lbl.Font       = Theme.FontSmall;
            lbl.AutoSize   = false;
            lbl.Size       = new Size(90, 22);
            lbl.TextAlign  = ContentAlignment.MiddleCenter;

            lbl.BackColor = status switch {
                "Admitted"    => Theme.AccentGlow,
                "Critical"    => Color.FromArgb(80, 248, 113, 113),
                "Discharged"  => Theme.BorderLight,
                "Confirmed"   => Color.FromArgb(60, 52, 211, 153),
                "Pending"     => Color.FromArgb(70, 251, 191, 36),
                "Cancelled"   => Color.FromArgb(80, 248, 113, 113),
                "Done"        => Theme.Border,
                "Available"   => Color.FromArgb(60, 52, 211, 153),
                "Unavailable" => Color.FromArgb(80, 248, 113, 113),
                _             => Theme.Border
            };

            lbl.ForeColor = status switch {
                "Admitted"    => Theme.Accent,
                "Critical"    => Theme.Danger,
                "Discharged"  => Theme.TextMuted,
                "Confirmed"   => Theme.Success,
                "Pending"     => Theme.Warning,
                "Cancelled"   => Theme.Danger,
                "Done"        => Theme.TextSecondary,
                "Available"   => Theme.Success,
                "Unavailable" => Theme.Danger,
                _             => Theme.TextSecondary
            };

            return lbl;
        }

        // grid setings
        public static DataGridView MakeGrid() {
            DataGridView grid              = new DataGridView();
            grid.Dock                      = DockStyle.Fill;
            grid.BackgroundColor           = Theme.Card;
            grid.BorderStyle               = BorderStyle.None;
            grid.GridColor                 = Theme.Border;
            grid.RowHeadersVisible         = false;
            grid.AllowUserToAddRows        = false;
            grid.AllowUserToDeleteRows     = false;
            grid.ReadOnly                  = true;
            grid.SelectionMode             = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect               = false;
            grid.Font                      = Theme.FontBody;
            grid.ColumnHeadersHeight       = 42;
            grid.RowTemplate.Height        = 40;
            grid.AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal;

            grid.ColumnHeadersDefaultCellStyle.BackColor  = Theme.Sidebar;
            grid.ColumnHeadersDefaultCellStyle.ForeColor  = Theme.TextSecondary;
            grid.ColumnHeadersDefaultCellStyle.Font       = Theme.FontLabel;
            grid.ColumnHeadersDefaultCellStyle.Padding    = new Padding(12, 0, 0, 0);

            grid.DefaultCellStyle.BackColor          = Theme.Card;
            grid.DefaultCellStyle.ForeColor          = Theme.TextPrimary;
            grid.DefaultCellStyle.Padding            = new Padding(12, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Theme.AccentGlow;
            grid.DefaultCellStyle.SelectionForeColor = Theme.Accent;
            grid.DefaultCellStyle.WrapMode           = DataGridViewTriState.False;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Theme.CardHover;

            return grid;
        }

        // card settings
        public static Panel MakeCard() {
            Panel card     = new Panel();
            card.BackColor = Theme.Card;
            card.Padding   = new Padding(16);
            card.Dock      = DockStyle.Fill;
            return card;
        }

        // sidebar nav settings
        public static Button MakeNavButton(string text) {
            Button btn                            = new Button();
            btn.Text                              = text;
            btn.Font                              = Theme.FontBody;
            btn.ForeColor                         = Theme.TextSecondary;
            btn.BackColor                         = Theme.Sidebar;
            btn.FlatStyle                         = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = Theme.CardHover;
            btn.Dock                              = DockStyle.Fill;
            btn.TextAlign                         = ContentAlignment.MiddleLeft;
            btn.Padding                           = new Padding(20, 0, 0, 0);
            btn.Cursor                            = Cursors.Hand;
            return btn;
        }

        // active state nav settings
        public static void SetNavActive(Button active, params Button[] all) {
            foreach (Button b in all) {
                b.ForeColor                         = Theme.TextSecondary;
                b.BackColor                         = Theme.Sidebar;
                b.FlatAppearance.BorderColor        = Theme.Sidebar;
            }
            active.ForeColor                        = Theme.Accent;
            active.BackColor                        = Theme.AccentGlow;
            active.FlatAppearance.BorderColor       = Theme.AccentGlow;
        }

        // divider settings
        public static Panel MakeDivider() {
            Panel d      = new Panel();
            d.Dock       = DockStyle.Fill;
            d.BackColor  = Theme.Border;
            return d;
        }
    }   
}