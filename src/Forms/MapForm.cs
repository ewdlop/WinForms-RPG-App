using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Interfaces;
using Microsoft.Extensions.Logging;

namespace WinFormsApp1
{
    public partial class MapForm : Form
    {
        private readonly ILocationManager _locationManager;
        private readonly ILogger<MapForm> _logger;
        private Panel mapPanel;
        private Dictionary<string, Rectangle> locationRects;
        private Dictionary<string, Point> locationPositions;
        private ToolTip toolTip;

        public MapForm(ILocationManager locationManager, ILogger<MapForm> logger)
        {
            _locationManager = locationManager;
            _logger = logger;
            this.locationRects = new Dictionary<string, Rectangle>();
            this.locationPositions = new Dictionary<string, Point>();
            this.toolTip = new ToolTip();
            InitializeComponent();
            SetupLocationPositions();
        }

        private void InitializeComponent()
        {
            this.Text = "World Map - Realm of Aethermoor";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(600, 450);

            // Create main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Title
            Label titleLabel = new Label
            {
                Text = "World Map",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Height = 40
            };

            // Map panel
            mapPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGreen,
                BorderStyle = BorderStyle.Fixed3D
            };
            mapPanel.Paint += MapPanel_Paint;
            mapPanel.MouseClick += MapPanel_MouseClick;
            mapPanel.MouseMove += MapPanel_MouseMove;

            // Legend panel
            Panel legendPanel = CreateLegendPanel();

            mainLayout.Controls.Add(titleLabel, 0, 0);
            mainLayout.Controls.Add(mapPanel, 0, 1);
            mainLayout.Controls.Add(legendPanel, 0, 2);

            this.Controls.Add(mainLayout);
        }

        private Panel CreateLegendPanel()
        {
            Panel legendPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label legendTitle = new Label
            {
                Text = "Legend:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // Create legend items
            int x = 80;
            int y = 10;

            // Current location
            Panel currentBox = new Panel
            {
                Size = new Size(20, 20),
                Location = new Point(x, y),
                BackColor = Color.Red,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label currentLabel = new Label
            {
                Text = "Current Location",
                Location = new Point(x + 25, y + 2),
                AutoSize = true
            };

            x += 150;

            // Visited location
            Panel visitedBox = new Panel
            {
                Size = new Size(20, 20),
                Location = new Point(x, y),
                BackColor = Color.LightBlue,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label visitedLabel = new Label
            {
                Text = "Visited Location",
                Location = new Point(x + 25, y + 2),
                AutoSize = true
            };

            x += 150;

            // Unvisited location
            Panel unvisitedBox = new Panel
            {
                Size = new Size(20, 20),
                Location = new Point(x, y),
                BackColor = Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label unvisitedLabel = new Label
            {
                Text = "Unvisited Location",
                Location = new Point(x + 25, y + 2),
                AutoSize = true
            };

            y += 30;

            Label instructionLabel = new Label
            {
                Text = "Click on a location to view details. Double-click to travel (if accessible).",
                Location = new Point(10, y),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            legendPanel.Controls.AddRange(new Control[] {
                legendTitle, currentBox, currentLabel, visitedBox, visitedLabel,
                unvisitedBox, unvisitedLabel, instructionLabel
            });

            return legendPanel;
        }

        private void SetupLocationPositions()
        {
            // Define positions for each location on the map
            locationPositions = new Dictionary<string, Point>
            {
                ["village"] = new Point(400, 400),
                ["forest"] = new Point(400, 250),
                ["plains"] = new Point(600, 400),
                ["cave"] = new Point(400, 100),
                ["ruins"] = new Point(600, 250),
                ["dragon_lair"] = new Point(400, 50)
            };

            // Create rectangles for each location
            foreach (var kvp in locationPositions)
            {
                locationRects[kvp.Key] = new Rectangle(kvp.Value.X - 40, kvp.Value.Y - 20, 80, 40);
            }
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw connections between locations
            DrawConnections(g);

            // Draw locations
            foreach (var kvp in locationPositions)
            {
                string locationKey = kvp.Key;
                Point position = kvp.Value;
                Rectangle rect = locationRects[locationKey];

                // Determine color based on location status
                Color locationColor;
                if (locationKey == _locationManager.CurrentLocationKey)
                {
                    locationColor = Color.Red;
                }
                else if (HasVisited(locationKey))
                {
                    locationColor = Color.LightBlue;
                }
                else
                {
                    locationColor = Color.Gray;
                }

                // Draw location rectangle
                using (Brush brush = new SolidBrush(locationColor))
                {
                    g.FillRectangle(brush, rect);
                }
                g.DrawRectangle(Pens.Black, rect);

                // Draw location name
                if (_locationManager.Locations.ContainsKey(locationKey))
                {
                    string locationName = _locationManager.Locations[locationKey].Name;
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        SizeF textSize = g.MeasureString(locationName, font);
                        PointF textPosition = new PointF(
                            position.X - textSize.Width / 2,
                            position.Y - textSize.Height / 2
                        );
                        g.DrawString(locationName, font, Brushes.White, textPosition);
                    }
                }

                // Draw enemies indicator
                if (_locationManager.Locations.ContainsKey(locationKey) && _locationManager.Locations[locationKey].Enemies.Any())
                {
                    g.FillEllipse(Brushes.Red, rect.Right - 10, rect.Top, 8, 8);
                }

                // Draw items indicator
                if (_locationManager.Locations.ContainsKey(locationKey) && _locationManager.Locations[locationKey].Items.Any())
                {
                    g.FillEllipse(Brushes.Gold, rect.Left + 2, rect.Top, 8, 8);
                }
            }
        }

        private void DrawConnections(Graphics g)
        {
            using (Pen connectionPen = new Pen(Color.Brown, 2))
            {
                foreach (var kvp in _locationManager.Locations)
                {
                    string fromKey = kvp.Key;
                    Location fromLocation = kvp.Value;

                    if (!locationPositions.ContainsKey(fromKey)) continue;

                    Point fromPoint = locationPositions[fromKey];

                    foreach (var exit in fromLocation.Exits)
                    {
                        string toKey = exit.Value;
                        if (locationPositions.ContainsKey(toKey))
                        {
                            Point toPoint = locationPositions[toKey];
                            g.DrawLine(connectionPen, fromPoint, toPoint);

                            // Draw arrow
                            DrawArrow(g, connectionPen, fromPoint, toPoint);
                        }
                    }
                }
            }
        }

        private void DrawArrow(Graphics g, Pen pen, Point from, Point to)
        {
            // Calculate arrow direction
            float dx = to.X - from.X;
            float dy = to.Y - from.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length > 0)
            {
                dx /= length;
                dy /= length;

                // Arrow position (closer to destination)
                float arrowX = to.X - dx * 30;
                float arrowY = to.Y - dy * 30;

                // Arrow head points
                float arrowLength = 10;
                float arrowAngle = 0.5f;

                PointF arrowHead1 = new PointF(
                    arrowX - arrowLength * (float)Math.Cos(Math.Atan2(dy, dx) - arrowAngle),
                    arrowY - arrowLength * (float)Math.Sin(Math.Atan2(dy, dx) - arrowAngle)
                );

                PointF arrowHead2 = new PointF(
                    arrowX - arrowLength * (float)Math.Cos(Math.Atan2(dy, dx) + arrowAngle),
                    arrowY - arrowLength * (float)Math.Sin(Math.Atan2(dy, dx) + arrowAngle)
                );

                g.DrawLine(pen, arrowX, arrowY, arrowHead1.X, arrowHead1.Y);
                g.DrawLine(pen, arrowX, arrowY, arrowHead2.X, arrowHead2.Y);
            }
        }

        private bool HasVisited(string locationKey)
        {
            // For now, assume all locations except current are visited
            // In a real implementation, you'd track this in the game state
            return locationKey != _locationManager.CurrentLocationKey;
        }

        private void MapPanel_MouseClick(object sender, MouseEventArgs e)
        {
            string clickedLocation = GetLocationAtPoint(e.Location);
            if (clickedLocation != null)
            {
                if (e.Clicks == 1)
                {
                    ShowLocationInfo(clickedLocation);
                }
                else if (e.Clicks == 2)
                {
                    TravelToLocation(clickedLocation);
                }
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            string hoveredLocation = GetLocationAtPoint(e.Location);
            if (hoveredLocation != null)
            {
                mapPanel.Cursor = Cursors.Hand;
                toolTip.SetToolTip(mapPanel, _locationManager.Locations[hoveredLocation].Name);
            }
            else
            {
                mapPanel.Cursor = Cursors.Default;
                toolTip.SetToolTip(mapPanel, "");
            }
        }

        private string GetLocationAtPoint(Point point)
        {
            foreach (var kvp in locationRects)
            {
                if (kvp.Value.Contains(point))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        private void ShowLocationInfo(string locationKey)
        {
            if (_locationManager.Locations.ContainsKey(locationKey))
            {
                Location location = _locationManager.Locations[locationKey];
                string info = $"Location: {location.Name}\n\n";
                info += $"Description: {location.Description}\n\n";

                if (location.Exits.Any())
                {
                    info += "Exits:\n";
                    foreach (var exit in location.Exits)
                    {
                        info += $"  {exit.Key} - {exit.Value}\n";
                    }
                    info += "\n";
                }

                if (location.Enemies.Any())
                {
                    info += "Enemies:\n";
                    foreach (var enemy in location.Enemies)
                    {
                        info += $"  {enemy.Name} (Level {enemy.Level})\n";
                    }
                    info += "\n";
                }

                if (location.Items.Any())
                {
                    info += "Items:\n";
                    foreach (var item in location.Items)
                    {
                        info += $"  {item.Name}\n";
                    }
                }

                MessageBox.Show(info, $"Location Info - {location.Name}", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TravelToLocation(string locationKey)
        {
            if (locationKey == _locationManager.CurrentLocationKey)
            {
                MessageBox.Show("You are already at this location!", "Travel", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Check if location is accessible from current location
            if (_locationManager.Locations.ContainsKey(_locationManager.CurrentLocationKey))
            {
                var currentLocation = _locationManager.Locations[_locationManager.CurrentLocationKey];
                bool canTravel = currentLocation.Exits.ContainsValue(locationKey);

                if (canTravel)
                {
                    var result = MessageBox.Show($"Travel to {_locationManager.Locations[locationKey].Name}?", 
                        "Confirm Travel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Find the direction to travel
                        string direction = currentLocation.Exits.FirstOrDefault(e => e.Value == locationKey).Key;
                        if (!string.IsNullOrEmpty(direction))
                        {
                            this.Close();
                            // The game engine will handle the actual movement
                            MessageBox.Show($"Use the command 'go {direction}' to travel there.", 
                                "Travel Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You cannot travel directly to this location from your current position.", 
                        "Travel Not Possible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
} 