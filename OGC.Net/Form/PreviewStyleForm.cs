namespace Geosite
{
    public partial class PreviewStyleForm : Form
    {
        public bool Ok;
        public static ((Pen pen, int flag) point, Pen line, Pen polygon, Pen mapGrid)? Style;

        public PreviewStyleForm()
        {
            InitializeComponent();
            Style ??=
                (
                    (new Pen(Color.FromArgb(13, 110, 253), 2), 0),
                    new Pen(Color.FromArgb(0, 0, 0), 1),
                    new Pen(Color.FromArgb(255, 63, 34), 1),
                    new Pen(Color.FromArgb(255, 255, 255), 1)
                );
            PointColorPanel.BackColor = Style.Value.point.pen.Color;
            LineColorPanel.BackColor = Style.Value.line.Color;
            PolygonColorPanel.BackColor = Style.Value.polygon.Color;
            MapGridColorPanel.BackColor = Style.Value.mapGrid.Color;
            var flag = Style.Value.point.flag;
            switch (flag)
            {
                case 0:
                    CircleRadioButton.Checked = true;
                    break;
                default:
                    PinRadioButton.Checked = true;
                    break;
            }
            Ok = false;
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            Style =
            (
                (
                    new Pen(PointColorPanel.BackColor, 2),
                    CircleRadioButton.Checked ? 0 : 1
                ),
                new Pen(LineColorPanel.BackColor, 1),
                new Pen(PolygonColorPanel.BackColor, 1),
                new Pen(MapGridColorPanel.BackColor, 1)
            );
            Ok = true;
            Close();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ColorPanel_Click(object sender, EventArgs e)
        {
            var option = (Panel)sender;
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                var color = ColorDialog.Color;
                switch (option.Name)
                {
                    case "PointColorPanel":
                        {
                            PointColorPanel.BackColor = color;
                            break;
                        }
                    case "LineColorPanel":
                        {
                            LineColorPanel.BackColor = color;
                            break;
                        }
                    case "PolygonColorPanel":
                        {
                            PolygonColorPanel.BackColor = color;
                            break;
                        }
                    //case "MapGridColorPanel":
                    default:
                        {
                            MapGridColorPanel.BackColor = color;
                            break;
                        }
                }
            }
        }

        private void PinRadioPicture_Click(object sender, EventArgs e)
        {
            PinRadioButton.Checked = true;
        }
    }
}
